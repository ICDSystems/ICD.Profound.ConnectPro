using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Conferencing.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Routing.PathFinding;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRouting : IDisposable
	{
		/// <summary>
		/// Raised when a source becomes routed/unrouted to a display.
		/// </summary>
		public event EventHandler OnDisplaySourceChanged;

		/// <summary>
		/// Raised when a source becomes routed/unrouted to room audio.
		/// </summary>
		public event EventHandler OnAudioSourceChanged;

		/// <summary>
		/// Raised when a source becomes routed for the first time or completely unrouted from all displays.
		/// </summary>
		public event EventHandler OnSourceRoutedChanged;

		private readonly IConnectProRoom m_Room;
		private readonly List<IDestination> m_Displays;
		private readonly List<IDestination> m_AudioDestinations;
		private readonly SafeCriticalSection m_CacheSection;

		// Keeps track of the active routing states
		private readonly Dictionary<IDestination, IcdHashSet<ISource>> m_VideoRoutingCache;
		private readonly IcdHashSet<ISource> m_AudioRoutingCache;

		// Keeps track of the processing state for each source.
		private readonly Dictionary<ISource, eSourceState> m_SourceRoutedStates;
		private readonly Dictionary<IDestination, ProcessingSourceInfo> m_ProcessingSources;

		private IRoutingGraph m_SubscribedRoutingGraph;
		private IPathFinder m_PathFinder;

		#region Properties

		/// <summary>
		/// Gets the routing graph from the core.
		/// </summary>
		private IRoutingGraph RoutingGraph { get { return m_SubscribedRoutingGraph; } }

		private IPathFinder PathFinder { get { return m_PathFinder = m_PathFinder ?? new DefaultPathFinder(RoutingGraph, m_Room.Id); } }

		/// <summary>
		/// Returns true if the room contains more than 1 display.
		/// </summary>
		public bool IsDualDisplayRoom { get { return GetDisplayDestinations().Count() > 1; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ConnectProRouting(IConnectProRoom room)
		{
			m_Room = room;
			m_Displays = new List<IDestination>();
			m_AudioDestinations = new List<IDestination>();
			m_VideoRoutingCache = new Dictionary<IDestination, IcdHashSet<ISource>>();
			m_AudioRoutingCache = new IcdHashSet<ISource>();
			m_SourceRoutedStates = new Dictionary<ISource, eSourceState>();
			m_ProcessingSources = new Dictionary<IDestination, ProcessingSourceInfo>();
			m_CacheSection = new SafeCriticalSection();

			Subscribe(room);
			Subscribe(room.Core.GetRoutingGraph());
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			OnDisplaySourceChanged = null;
			OnAudioSourceChanged = null;

			Unsubscribe(m_Room);
			Unsubscribe(m_SubscribedRoutingGraph);
		}

		#region Caching

		private void UpdateRoutingCache(eConnectionType type)
		{
			// Build a map of video destination => source
			if (type.HasFlag(eConnectionType.Video))
				UpdateVideoRoutingCache();

			// Get a list of sources going to audio destinations
			if (type.HasFlag(eConnectionType.Audio))
				UpdateAudioRoutingCache();
		}

		/// <summary>
		/// Updates the current audio routing cache to match the routed sources.
		/// </summary>
		/// <returns></returns>
		private void UpdateAudioRoutingCache()
		{
			m_CacheSection.Enter();

			try
			{
				IcdHashSet<ISource> activeAudio = GetActiveAudioSources().ToIcdHashSet();
				if (activeAudio.SetEquals(m_AudioRoutingCache))
					return;

				m_AudioRoutingCache.Clear();
				m_AudioRoutingCache.AddRange(activeAudio);
			}
			finally
			{
				m_CacheSection.Leave();
			}

			OnAudioSourceChanged.Raise(this);
		}

		/// <summary>
		/// Updates the current video routing cache to match the routed sources.
		/// </summary>
		/// <returns></returns>
		private void UpdateVideoRoutingCache()
		{
			m_CacheSection.Enter();

			try
			{
				Dictionary<IDestination, IcdHashSet<ISource>> routing =
				GetDisplayDestinations()
					.ToDictionary(destination => destination,
								  destination => GetActiveVideoSources(destination).ToIcdHashSet());

				bool change = false;
				foreach (KeyValuePair<IDestination, IcdHashSet<ISource>> kvp in routing)
				{
					IcdHashSet<ISource> cache;
					if (!m_VideoRoutingCache.TryGetValue(kvp.Key, out cache))
					{
						cache = new IcdHashSet<ISource>();
						m_VideoRoutingCache.Add(kvp.Key, cache);
					}

					if (cache.SetEquals(kvp.Value))
						continue;

					change = true;

					cache.Clear();
					cache.AddRange(kvp.Value);
				}

				// No change, break early
				if (!change)
					return;

				// Remove routed items from the processing sources collection
				foreach (KeyValuePair<IDestination, IcdHashSet<ISource>> kvp in m_VideoRoutingCache)
				{
					ProcessingSourceInfo processing = m_ProcessingSources.GetDefault(kvp.Key);
					if (processing == null || processing.Source == null)
						continue;

					if (!kvp.Value.Contains(processing.Source))
						continue;

					processing.StopTimer();
					processing.Source = null;
				}

				UpdateSourceRoutedStates();

				// Now update the "faked" routing states for UX
				foreach (KeyValuePair<IDestination, ProcessingSourceInfo> item in m_ProcessingSources.Where(kvp => kvp.Value != null))
				{
					IcdHashSet<ISource> sources;
					if (!m_VideoRoutingCache.TryGetValue(item.Key, out sources))
					{
						sources = new IcdHashSet<ISource>();
						m_VideoRoutingCache.Add(item.Key, sources);
					}

					if (item.Value.Source != null)
						sources.Add(item.Value.Source);
				}
			}
			finally
			{
				m_CacheSection.Leave();
			}

			OnDisplaySourceChanged.Raise(this);
		}

		private void UpdateSourceRoutedStates()
		{
			m_CacheSection.Enter();

			try
			{
				// Build a map of video sources to their routed state
				Dictionary<ISource, eSourceState> routedSources =
					m_VideoRoutingCache.Values
					                   .SelectMany(v => v)
					                   .Distinct()
					                   .ToDictionary(s => s, s => eSourceState.Active);

				// A source may be processing for another display, so we override
				foreach (ISource source in m_ProcessingSources.Values.Where(p => p != null && p.Source != null).Select(s => s.Source))
					routedSources[source] = eSourceState.Processing;

				if (routedSources.DictionaryEqual(m_SourceRoutedStates))
					return;

				m_SourceRoutedStates.Clear();
				m_SourceRoutedStates.AddRange(routedSources);
			}
			finally
			{
				m_CacheSection.Leave();
			}

			OnSourceRoutedChanged.Raise(this);
		}

		#endregion

		#region Processing Sources

		/// <summary>
		/// Clears the processing source for each display destination.
		/// </summary>
		public void ClearProcessingSources()
		{
			m_CacheSection.Enter();

			try
			{
				foreach (ProcessingSourceInfo processing in m_ProcessingSources.Values.Where(p => p != null))
					processing.Dispose();
				m_ProcessingSources.Clear();

				UpdateSourceRoutedStates();
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Clears the processing source for the display destination.
		/// </summary>
		private void ClearProcessingSource([NotNull] IDestination destination, [NotNull] ISource source)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			if (source == null)
				throw new ArgumentNullException("source");

			m_CacheSection.Enter();

			try
			{
				ProcessingSourceInfo processing = m_ProcessingSources.GetDefault(destination);
				if (processing == null || source != processing.Source)
					return;

				processing.Source = null;

				UpdateSourceRoutedStates();
				UpdateRoutingCache(eConnectionType.Video);
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Sets the processing source for the single display destination.
		/// </summary>
		/// <param name="source"></param>
		public void SetProcessingSource([NotNull] ISource source)
		{
			IDestination destination = m_Room == null ? null : m_Room.Routing.GetDisplayDestinations().FirstOrDefault();
			if (destination == null)
				return;

			SetProcessingSource(destination, source);
		}

		/// <summary>
		/// Sets the processing source for the given destination.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="source"></param>
		public void SetProcessingSource([NotNull] IDestination destination, [NotNull] ISource source)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			if (source == null)
				throw new ArgumentNullException("source");

			m_CacheSection.Enter();

			try
			{
				ProcessingSourceInfo processing;
				if (!m_ProcessingSources.TryGetValue(destination, out processing))
				{
					processing = new ProcessingSourceInfo(destination, ClearProcessingSource);
					m_ProcessingSources.Add(destination, processing);
				}

				// No change
				if (source == processing.Source)
					return;

				// Is the source already routed to the destination?
				IcdHashSet<ISource> routed;
				if (m_VideoRoutingCache.TryGetValue(destination, out routed) && routed.Contains(source))
					return;

				processing.ResetTimer();
				processing.Source = source;

				UpdateSourceRoutedStates();
				UpdateRoutingCache(eConnectionType.Video);
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		#endregion

		#region Sources

		/// <summary>
		/// Returns all of the sources available in the core.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ISource> GetCoreSources()
		{
			return m_Room.Core
			             .Originators
			             .GetChildren<ISource>()
			             .OrderBy(s => s.Order);
		}

		/// <summary>
		/// Returns all of the sources available in the core.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ISource> GetSources()
		{
			return m_Room.Originators
			             .GetInstancesRecursive<ISource>()
			             .OrderBy(s => s.Order);
		}

		/// <summary>
		/// Gets the room for the given source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		[CanBeNull]
		public IRoom GetRoomForSource(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			// This room takes precedence
			if (m_Room.Originators.ContainsRecursive(source.Id))
				return m_Room;

			return m_Room.Core
			             .Originators
			             .GetChildren<IRoom>()
			             .Where(r => r.Originators.ContainsRecursive(source.Id))
			             .OrderBy(r => r.IsCombineRoom())
			             .FirstOrDefault();
		}

		[NotNull]
		public IEnumerable<KeyValuePair<IDestination, IcdHashSet<ISource>>> GetCachedActiveVideoSources()
		{
			return
				m_CacheSection.Execute(() =>
				                       m_VideoRoutingCache.ToDictionary(kvp => kvp.Key, kvp => new IcdHashSet<ISource>(kvp.Value)));
		}

		[NotNull]
		public IEnumerable<ISource> GetCachedActiveAudioSources()
		{
			return m_CacheSection.Execute(() => m_AudioRoutingCache.ToArray(m_AudioRoutingCache.Count));
		}

		/// <summary>
		/// Returns true if the given source is routed to any of the room destinations for the given connection flag.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public bool GetIsRoutedCached(ISource source, eConnectionType flag)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (!EnumUtils.HasSingleFlag(flag))
				throw new ArgumentException("Connection type must have a single flag", "flag");

			m_CacheSection.Enter();

			try
			{
				switch (flag)
				{
					case eConnectionType.Audio:
						return m_AudioRoutingCache.Contains(source);
					case eConnectionType.Video:
						return m_VideoRoutingCache.Any(kvp => kvp.Value.Contains(source));

					default:
						throw new ArgumentOutOfRangeException("flag");
				}
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Returns the routed state for each source.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<ISource, eSourceState>> GetSourceRoutedStates()
		{
			return m_CacheSection.Execute(() => m_SourceRoutedStates.ToArray(m_SourceRoutedStates.Count));
		}

		#endregion

		#region Destinations

		/// <summary>
		/// Returns the first two ordered display destinations for the room.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IDestination> GetDisplayDestinations()
		{
			m_CacheSection.Enter();

			try
			{
				if (m_Displays.Count == 0)
				{
					bool combine = m_Room.IsCombineRoom();

					IEnumerable<IDestination> displays =
						m_Room.Originators
						      .GetInstancesRecursive<IDestination>(d =>
							                                           m_Room.Core.Originators.GetChild(d.Device)
								                                           is IDisplay)
						      .Where(d => d.ConnectionType.HasFlag(eConnectionType.Video))
						      .OrderBy(d => d.Order)
						      .ThenBy(d => d.GetNameOrDeviceName(combine));

					m_Displays.AddRange(displays);
				}

				return m_Displays.ToArray(m_Displays.Count);
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Returns the ordered audio only destinations for the room.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IDestination> GetAudioDestinations()
		{
			m_CacheSection.Enter();

			try
			{
				if (m_AudioDestinations.Count == 0)
				{
					bool combine = m_Room.IsCombineRoom();

					IEnumerable<IDestination> audioDestinations =
						m_Room.Originators
						      .GetInstancesRecursive<IDestination>(d => d.ConnectionType.HasFlag(eConnectionType.Audio))
						      .OrderBy(d => d.Order)
						      .ThenBy(d => d.GetNameOrDeviceName(combine));

					m_AudioDestinations.AddRange(audioDestinations);
				}

				return m_AudioDestinations.ToArray(m_AudioDestinations.Count);
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		private IEnumerable<ISource> GetActiveAudioSources()
		{
			return GetAudioDestinations().SelectMany(destination => GetActiveSources(destination, eConnectionType.Audio));
		}

		/// <summary>
		/// Gets all of the source endpoints actively routed to the given destination.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		private IEnumerable<EndpointInfo> GetActiveEndpoints(IDestination destination, eConnectionType flag)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			return RoutingGraph.RoutingCache.GetSourceEndpointsForDestination(destination, flag, false, true);
		}

		private IEnumerable<ISource> GetActiveVideoSources(IDestination destination)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			return GetActiveSources(destination, eConnectionType.Video);
		}

		private IEnumerable<ISource> GetActiveSources(IDestination destination, eConnectionType flag)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			return GetActiveEndpoints(destination, flag).SelectMany(e => RoutingGraph.Sources.GetChildren(e, flag))
														.Distinct();
		}

		#endregion

		#region Routing

		/// <summary>
		/// Routes the source to the display and room audio.
		/// </summary>
		/// <param name="source"></param>
		public void RouteSingleDisplay(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IDestination destination = GetDisplayDestinations().First();
			
			Route(source, destination, eConnectionType.Video);

			if (source.ConnectionType.HasFlag(eConnectionType.Audio))
				RouteAudio(source);
			else
				UnrouteAudio();
		}

		/// <summary>
		/// Routes the source to the display and room audio.
		/// </summary>
		/// <param name="source"></param>
		public void RouteAllDisplays(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IDestination[] destinations = GetDisplayDestinations().ToArray();
			
			foreach (var destination in destinations)
				Route(source, destination, eConnectionType.Video);

			if (source.ConnectionType.HasFlag(eConnectionType.Audio))
				RouteAudio(source);
			else
				UnrouteAudio();
		}

		/// <summary>
		/// Routes the source to the destination.
		/// Routes to room audio if there is no other audio source currently routed.
		/// Unroutes routed audio if associated video is unrouted.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public void RouteDualDisplay(ISource source, IDestination destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			Route(source, destination, eConnectionType.Video);

			if (!source.ConnectionType.HasFlag(eConnectionType.Audio))
				return;

			if (CanOverrideAudio(source, destination))
				RouteAudio(source);
			else
				RouteAudioIfNoAudioRouted(source);
		}

		/// <summary>
		/// Returns true if we can override active audio when routing the given source to the given destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		private bool CanOverrideAudio(ISource source, IDestination destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			if (m_Room == null)
				return false;

			m_CacheSection.Enter();

			try
			{
				// Nothing is currently routed for audio
				IcdHashSet<ISource> activeAudio = m_AudioRoutingCache.ToIcdHashSet();
				if (activeAudio.Count == 0)
					return false;

				// If there are no sources routed to the display then there is nothing to override
				IcdHashSet<ISource> activeVideo = m_VideoRoutingCache.GetDefault(destination);
				if (activeVideo == null || activeVideo.Count == 0)
					return false;

				// No change
				if (activeVideo.Contains(source))
					return false;

				// Old source is not current active audio
				if (!activeAudio.Intersect(activeVideo).Any())
					return false;

				// Is there another source routed for audio going to another display?
				foreach (KeyValuePair<IDestination, IcdHashSet<ISource>> kvp in m_VideoRoutingCache)
				{
					// Skip the display we are currently routing to
					IDestination display = kvp.Key;
					if (display == destination)
						continue;

					// Are we in the middle of routing a new source to the display?
					ProcessingSourceInfo processing = m_ProcessingSources.GetDefault(display);
					if (processing != null && processing.Source != null)
						continue;

					// The display has a source that is being listened to
					if (kvp.Value.Intersect(activeAudio).Any())
						return false;
				}

				return true;
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Routes the codec to all available displays.
		/// </summary>
		/// <param name="sourceControl"></param>
		public void RouteVtc(IRouteSourceControl sourceControl)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			Connection[] outputs = RoutingGraph.Connections
			                                   .GetOutputConnections(sourceControl.Parent.Id,
			                                                         sourceControl.Id)
			                                   .Where(c => c.ConnectionType.HasFlag(eConnectionType.Video))
			                                   .OrderBy(o => o.Source.Address)
			                                   .ToArray();

			IDestination[] destinations = GetDisplayDestinations().ToArray();

			Connection firstOutput = outputs.LastOrDefault();
			if (firstOutput == null)
			{
				m_Room.Logger.AddEntry(eSeverity.Error, "Failed to find {0} output connection for {1}",
									   eConnectionType.Video, sourceControl);
				return;
			}

			for (int index = 0; index < destinations.Length; index++)
			{
				IDestination destination = destinations[index];

				Connection output;
				if (!outputs.TryElementAt(index, out output))
					output = firstOutput;

				if (output == null)
					break;

				Route(output.Source, destination, eConnectionType.Video);
			}

			RouteAtc(sourceControl);
		}

		/// <summary>
		/// Routes the audio dialer to the audio destination.
		/// </summary>
		/// <param name="sourceControl"></param>
		public void RouteAtc(IRouteSourceControl sourceControl)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			foreach (IDestination audioDestination in GetAudioDestinations())
			{
				// Edge case - Often the DSP is also the ATC, in which case we don't need to do any routing
				if (audioDestination.Device == sourceControl.Parent.Id)
					continue;

				Route(sourceControl, audioDestination, eConnectionType.Audio);
			}
		}

		private void Route(IRouteSourceControl source, IDestination destination, eConnectionType flag)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			IEnumerable<ConnectionPath> paths =
				PathBuilder.FindPaths()
						   .From(source)
						   .To(destination)
						   .OfType(flag)
						   .With(PathFinder);

			Route(paths);
		}

		/// <summary>
		/// Routes the source to the destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <param name="flag"></param>
		private void Route(ISource source, IDestination destination, eConnectionType flag)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			IEnumerable<ConnectionPath> paths =
				PathBuilder.FindPaths()
				           .From(source)
				           .To(destination)
				           .OfType(flag)
				           .With(PathFinder);

			Route(paths);
		}

		private void Route(EndpointInfo sourceEndpoint, IDestination destination, eConnectionType flag)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			IEnumerable<ConnectionPath> paths =
				PathBuilder.FindPaths()
						   .From(sourceEndpoint)
						   .To(destination)
						   .OfType(flag)
						   .With(PathFinder);

			Route(paths);
		}

		private void Route(ISource source, EndpointInfo destinationEndpoint, eConnectionType flag)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IEnumerable<ConnectionPath> paths =
				PathBuilder.FindPaths()
						   .From(source)
						   .To(destinationEndpoint)
						   .OfType(flag)
						   .With(PathFinder);

			Route(paths);
		}

		public void RouteAudio(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			foreach (IDestination destination in GetAudioDestinations())
				RouteAudio(source, destination);
		}

		/// <summary>
		/// Routes audio from the source to the given destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		private void RouteAudio(ISource source, IDestination destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			Route(source, destination, eConnectionType.Audio);
		}

		/// <summary>
		/// Route the source audio only if there is currently no audio routed.
		/// </summary>
		/// <param name="source"></param>
		private void RouteAudioIfNoAudioRouted(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (m_CacheSection.Execute(() => m_AudioRoutingCache.Count) == 0)
				RouteAudio(source);
		}

		/// <summary>
		/// Unroutes every destination that isn't currently showing the OSD.
		/// 
		/// Routes the OSD to the displays.
		/// 
		/// Powers off displays that have no OSD routed.
		/// </summary>
		public void RouteOsd()
		{
			// First unroute everything that isn't OSD
			UnrouteAllExceptOsd();

			OsdPanelDevice osd = m_Room.Core.Originators.GetChildren<OsdPanelDevice>().FirstOrDefault();

			// Route the OSD or power off displays
			if (osd == null)
			{
				foreach (IDestination destination in GetDisplayDestinations())
				{
					IDeviceBase destinationDevice =
						m_Room.Core.Originators.GetChild<IDeviceBase>(destination.Device);

					// Power off the destination
					IPowerDeviceControl powerControl = destinationDevice.Controls.GetControl<IPowerDeviceControl>();
					if (powerControl != null)
						powerControl.PowerOff();
				}
			}
			else
			{
				IRouteSourceControl sourceControl = osd.Controls.GetControl<IRouteSourceControl>();
				EndpointInfo sourceEndpoint = sourceControl.GetOutputEndpointInfo(1);

				foreach (IDestination destination in GetDisplayDestinations())
					Route(sourceEndpoint, destination, eConnectionType.Video);
			}
		}

		private void Route(IEnumerable<ConnectionPath> paths)
		{
			if (paths == null)
				throw new ArgumentNullException("paths");

			IList<ConnectionPath> pathsList = paths as IList<ConnectionPath> ?? paths.ToArray();

			IcdStopwatch.Profile(() => RoutingGraph.RoutePaths(pathsList, m_Room.Id),
			                     string.Format("Route - {0}", StringUtils.ArrayFormat(pathsList)));

			foreach (ConnectionPath path in pathsList)
			{
				EndpointInfo destination = path.DestinationEndpoint;
				IDeviceBase destinationDevice =
					m_Room.Core.Originators.GetChild<IDeviceBase>(destination.Device);

				// Power on the destination
				IPowerDeviceControl powerControl = destinationDevice.Controls.GetControl<IPowerDeviceControl>();
				if (powerControl != null && !powerControl.IsPowered)
					powerControl.PowerOn();

				// Set the destination to the correct input
				int input = destination.Address;
				IRouteInputSelectControl inputSelectControl =
					destinationDevice.Controls.GetControl<IRouteInputSelectControl>();

				if (inputSelectControl != null)
					inputSelectControl.SetActiveInput(input, path.ConnectionType);
			}
		}

		/// <summary>
		/// Routes the given source to the VTC and starts the presentation.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="presentationControl"></param>
		public void RouteVtcPresentation(ISource source, IPresentationControl presentationControl)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (presentationControl == null)
				throw new ArgumentNullException("presentationControl");

			IVideoConferenceRouteControl routingControl =
				presentationControl.Parent.Controls.GetControl<IVideoConferenceRouteControl>();
			if (routingControl == null)
				throw new InvalidOperationException("No routing control available.");

			// Get the content inputs
			int[] inputs = routingControl.GetCodecInputs(eCodecInputType.Content).ToArray();
			if (inputs.Length == 0)
			{
				m_Room.Logger.AddEntry(eSeverity.Error,
				                       "Failed to start presentation for {0} - Codec has no inputs configured for content.",
									   source);
				return;
			}

			// Find the first input available
			foreach (int input in inputs)
			{
				EndpointInfo endpoint = routingControl.GetInputEndpointInfo(input);

				// Is there a path?
				bool hasPath =
					PathBuilder.FindPaths()
					           .From(source)
							   .To(endpoint)
							   .OfType(eConnectionType.Video)
					           .With(PathFinder)
					           .Any();
				if (!hasPath)
					return;

				// Route the source video and audio to the codec
				Route(source, endpoint, eConnectionType.Video);
				Route(source, endpoint, eConnectionType.Audio);

				// Start the presentation
				presentationControl.StartPresentation(input);
				return;
			}

			m_Room.Logger.AddEntry(eSeverity.Error,
			                       "Failed to start presentation for {0} - Could not find a path to a Codec input configured for content.",
			                       source);
		}

		/// <summary>
		/// Unroutes the active VTC presentation source and ends the presentation.
		/// </summary>
		/// <param name="presentationControl"></param>
		public void UnrouteVtcPresentation(IPresentationControl presentationControl)
		{
			if (presentationControl == null)
				throw new ArgumentNullException("presentationControl");

			IVideoConferenceRouteControl control =
				presentationControl.Parent.Controls.GetControl<IVideoConferenceRouteControl>();
			if (control == null)
				throw new InvalidOperationException("No routing control available.");

			// Get the content inputs
			int[] inputs = control.GetCodecInputs(eCodecInputType.Content).ToArray();
			if (inputs.Length == 0)
			{
				m_Room.Logger.AddEntry(eSeverity.Error,
									   "Failed to end presentation - Codec has no inputs configured for content.");
				return;
			}

			// Find the first input available
			foreach (int input in inputs)
			{
				EndpointInfo endpoint = control.GetInputEndpointInfo(input);
				RoutingGraph.UnrouteDestination(endpoint, eConnectionType.Audio | eConnectionType.Video, m_Room.Id);
			}

			presentationControl.StopPresentation();
		}

		/// <summary>
		/// Gets the sources currently being presented.
		/// </summary>
		/// <param name="presentationControl"></param>
		/// <returns></returns>
		public IEnumerable<ISource> GetVtcPresentationSources(IPresentationControl presentationControl)
		{
			if (presentationControl == null)
				throw new ArgumentNullException("presentationControl");

			IVideoConferenceRouteControl control =
				presentationControl.Parent.Controls.GetControl<IVideoConferenceRouteControl>();
			if (control == null)
				throw new InvalidOperationException("No routing control available.");

			int? activeInput = presentationControl.PresentationActiveInput;
			if (activeInput == null)
				return Enumerable.Empty<ISource>();

			return RoutingGraph.RoutingCache
			                   .GetSourcesForDestinationEndpoint(control.GetInputEndpointInfo((int)activeInput),
			                                                     eConnectionType.Video);
		}

		/// <summary>
		/// Unroutes all audio.
		/// </summary>
		private void UnrouteAudio()
		{
			IDestination[] audioDestinations = GetAudioDestinations().ToArray();
			ISource[] audioSources = m_CacheSection.Execute(() => m_AudioRoutingCache.ToArray(m_AudioRoutingCache.Count));

			foreach (ISource audioSource in audioSources)
			{
				foreach (IDestination audioDestination in audioDestinations)
					RoutingGraph.Unroute(audioSource, audioDestination, eConnectionType.Audio, m_Room.Id);
			}
		}

		/// <summary>
		/// Unroute all sources except OSD from all destinations.
		/// </summary>
		private void UnrouteAllExceptOsd()
		{
			UnrouteAllVideoExceptOsd();
			UnrouteAllAudioExceptOsd();
		}

		private void UnrouteAllVideoExceptOsd()
		{
			Dictionary<IDestination, IcdHashSet<ISource>> activeVideoSources = new Dictionary<IDestination, IcdHashSet<ISource>>();

			m_CacheSection.Enter();

			try
			{
				foreach (KeyValuePair<IDestination, IcdHashSet<ISource>> kvp in GetCachedActiveVideoSources())
				{
					IcdHashSet<ISource> sources = new IcdHashSet<ISource>();
					activeVideoSources.Add(kvp.Key, sources);

					foreach (ISource source in kvp.Value)
					{
						if (m_Room.Core.Originators.GetChild(source.Device) is OsdPanelDevice)
							continue;

						sources.Add(source);
					}
				}
			}
			finally
			{
				m_CacheSection.Leave();
			}

			// Keep routing out of the critical section above
			foreach (KeyValuePair<IDestination, IcdHashSet<ISource>> pair in activeVideoSources)
				foreach (ISource videoSource in pair.Value)
					RoutingGraph.Unroute(videoSource, pair.Key, eConnectionType.Video, m_Room.Id);
		}

		private void UnrouteAllAudioExceptOsd()
		{
			IcdHashSet<IDestination> audioDestinations = new IcdHashSet<IDestination>();
			IcdHashSet<ISource> activeAudioSources = new IcdHashSet<ISource>();

			m_CacheSection.Enter();

			try
			{
				audioDestinations.AddRange(GetAudioDestinations());

				foreach (ISource source in GetCachedActiveAudioSources())
				{
					if (m_Room.Core.Originators.GetChild(source.Device) is OsdPanelDevice)
						continue;

					activeAudioSources.Add(source);
				}
			}
			finally
			{
				m_CacheSection.Leave();
			}

			// Keep routing out of the critical section above
			foreach (IDestination destination in audioDestinations)
				foreach (ISource source in activeAudioSources)
					RoutingGraph.Unroute(source, destination, eConnectionType.Audio, m_Room.Id);
		}

		#endregion

		#region Controls

		/// <summary>
		/// Gets the device for the given source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private static IDeviceBase GetDevice(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return ServiceProvider.GetService<ICore>().Originators.GetChild<IDeviceBase>(source.Device);
		}

		public static bool CanControl(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			eControlOverride controlOverride = GetControlOverride(source);

			// If the control type is NOT default we return true.
			// This means we can show UI items for sources that don't necessarily have an IDeviceControl.
			return controlOverride != eControlOverride.Default || GetDeviceControl(source, eControlOverride.Default) != null;
		}

		public static IDeviceControl GetDeviceControl(ISource source, eControlOverride controlOverride)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IDeviceBase device = GetDevice(source);
			return device == null ? null : GetDeviceControl(device, controlOverride);
		}

		private static IDeviceControl GetDeviceControl(IDeviceBase device, eControlOverride controlOverride)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			switch (controlOverride)
			{
				case eControlOverride.Default:

					IDeviceControl dialer =
						GetDeviceControl(device, eControlOverride.Vtc) ??
						GetDeviceControl(device, eControlOverride.WebConference) ??
						GetDeviceControl(device, eControlOverride.Atc);

					if (dialer != null)
						return dialer;

					IDeviceControl tuner = GetDeviceControl(device, eControlOverride.CableTv);
					if (tuner != null)
						return tuner;

					break;

				case eControlOverride.CableTv:
					return device.Controls.GetControls<ITvTunerControl>().FirstOrDefault();

				case eControlOverride.Atc:
				case eControlOverride.Vtc:
					return device.Controls.GetControls<ITraditionalConferenceDeviceControl>().FirstOrDefault();
					
				case eControlOverride.WebConference:
					return device.Controls.GetControls<IWebConferenceDeviceControl>().FirstOrDefault();
				
				default:
					throw new ArgumentOutOfRangeException("controlOverride");
			}

			return null;
		}

		public static eControlOverride GetControlOverride(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			ConnectProSource connectProSource = source as ConnectProSource;
			return connectProSource == null ? eControlOverride.Default : connectProSource.ControlOverride;
		}

		#endregion

		#region Room Callbacks

		private void Subscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnSettingsApplied += RoomOnSettingsApplied;
		}

		private void Unsubscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnSettingsApplied -= RoomOnSettingsApplied;
		}

		private void RoomOnSettingsApplied(object sender, EventArgs eventArgs)
		{
			UpdateRoutingCache(EnumUtils.GetFlagsAllValue<eConnectionType>());
		}

		#endregion

		#region RoutingGraph Callbacks

		/// <summary>
		/// Subscribe to the routing graph events.
		/// </summary>
		/// <param name="routingGraph"></param>
		private void Subscribe(IRoutingGraph routingGraph)
		{
			if (routingGraph == null)
				return;

			m_SubscribedRoutingGraph = routingGraph;

			routingGraph.RoutingCache.OnSourceDestinationRouteChanged += RoutingCacheOnRouteToDestinationChanged;
			routingGraph.RoutingCache.OnDestinationEndpointActiveChanged += RoutingCacheOnDestinationEndpointActiveChanged;
		}

		/// <summary>
		/// Unsubscribe from the routing graph events.
		/// </summary>
		/// <param name="routingGraph"></param>
		private void Unsubscribe(IRoutingGraph routingGraph)
		{
			if (routingGraph == null)
				return;

			m_SubscribedRoutingGraph = null;

			routingGraph.RoutingCache.OnSourceDestinationRouteChanged -= RoutingCacheOnRouteToDestinationChanged;
			routingGraph.RoutingCache.OnDestinationEndpointActiveChanged -= RoutingCacheOnDestinationEndpointActiveChanged;
		}

		/// <summary>
		/// Called when a switcher changes routing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoutingCacheOnRouteToDestinationChanged(object sender, SourceDestinationRouteChangedEventArgs eventArgs)
		{
			HandleRoutingChange(eventArgs.Type);
		}

		/// <summary>
		/// Called when a destinations active inputs change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoutingCacheOnDestinationEndpointActiveChanged(object sender, CacheStateChangedEventArgs eventArgs)
		{
			HandleRoutingChange(eventArgs.Type);
		}

		private void HandleRoutingChange(eConnectionType type)
		{
			UpdateRoutingCache(type);
		}

		#endregion
	}
}
