using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Conferencing.Devices;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Routing.RoutingGraphs;
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

		private readonly IConnectProRoom m_Room;
		private readonly List<IDestination> m_Displays;
		private readonly List<IDestination> m_AudioDestinations;
		private readonly Dictionary<IDestination, IcdHashSet<ISource>> m_VideoRoutingCache;
		private readonly IcdHashSet<ISource> m_AudioRoutingCache;
		private readonly SafeCriticalSection m_CacheSection;

		private IRoutingGraph m_SubscribedRoutingGraph;
		private bool m_Routing;

		/// <summary>
		/// Gets the routing graph from the core.
		/// </summary>
		private IRoutingGraph RoutingGraph { get { return m_SubscribedRoutingGraph; } }

		/// <summary>
		/// Returns true if the room contains more than 1 display.
		/// </summary>
		public bool IsDualDisplayRoom { get { return GetDisplayDestinations().Count() > 1; } }

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

		private void UpdateRoutingCache(eConnectionType type)
		{
			bool videoChange = false;
			bool audioChange = false;

			m_CacheSection.Enter();

			try
			{
				// Build a map of video destination => source
				if (type.HasFlag(eConnectionType.Video))
				{
					Dictionary<IDestination, IcdHashSet<ISource>> routing =
						GetDisplayDestinations()
							.ToDictionary(destination => destination,
							              destination => GetActiveVideoSources(destination).ToIcdHashSet());

					videoChange = UpdateVideoRoutingCache(routing);
				}

				// Get a list of sources going to audio destinations
				if (type.HasFlag(eConnectionType.Audio))
				{
					IcdHashSet<ISource> activeAudio = GetActiveAudioSources().ToIcdHashSet();
					audioChange = !activeAudio.SetEquals(m_AudioRoutingCache);

					m_AudioRoutingCache.Clear();
					m_AudioRoutingCache.AddRange(activeAudio);
				}

				// Make sure there are no audio routes without video routes
				IEnumerable<ISource> unrouteAudioSources =
					m_AudioRoutingCache.Where(s => !m_VideoRoutingCache.Any(kvp => kvp.Value.Contains(s)))
					                   .ToArray();
				foreach (ISource source in unrouteAudioSources)
					UnrouteAudio(source);
			}
			finally
			{
				m_CacheSection.Leave();
			}

			if (videoChange)
				OnDisplaySourceChanged.Raise(this);

			if (audioChange)
				OnAudioSourceChanged.Raise(this);
		}

		/// <summary>
		/// Updates the current video routing cache to match the given dictionary.
		/// Returns true if there are any changes.
		/// </summary>
		/// <param name="routing"></param>
		/// <returns></returns>
		private bool UpdateVideoRoutingCache(Dictionary<IDestination, IcdHashSet<ISource>> routing)
		{
			bool output = false;

			m_CacheSection.Enter();

			try
			{
				foreach (KeyValuePair<IDestination, IcdHashSet<ISource>> kvp in routing)
				{
					if (!m_VideoRoutingCache.ContainsKey(kvp.Key))
						m_VideoRoutingCache.Add(kvp.Key, new IcdHashSet<ISource>());

					output |= !m_VideoRoutingCache[kvp.Key].SetEquals(kvp.Value);

					m_VideoRoutingCache[kvp.Key].Clear();
					m_VideoRoutingCache[kvp.Key].AddRange(kvp.Value);
				}
			}
			finally
			{
				m_CacheSection.Leave();
			}

			return output;
		}

		[CanBeNull]
		private IVideoConferenceDevice GetCodec()
		{
			IDialingDeviceControl dialer = m_Room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			return dialer == null ? null : dialer.Parent as IVideoConferenceDevice;
		}

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
		/// Gets the room for the given source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		[CanBeNull]
		public IRoom GetRoomForSource(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			// This room takes precendence
			if (m_Room.Originators.ContainsRecursive(source.Id))
				return m_Room;

			return m_Room.Core
			             .Originators
			             .GetChildren<IRoom>()
			             .Where(r => r.Originators.ContainsRecursive(source.Id))
			             .OrderBy(r => r.IsCombineRoom())
			             .FirstOrDefault();
		}

		public IEnumerable<KeyValuePair<IDestination, IcdHashSet<ISource>>> GetCachedActiveVideoSources()
		{
			return m_CacheSection.Execute(() => m_VideoRoutingCache.ToArray(m_VideoRoutingCache.Count));
		}

		public IEnumerable<ISource> GetCachedActiveAudioSources()
		{
			return m_CacheSection.Execute(() => m_AudioRoutingCache.ToArray(m_AudioRoutingCache.Count));
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
						      .ThenBy(d => d.GetNameOrDeviceName(combine))
						      .Take(2);

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
		private IEnumerable<IDestination> GetAudioDestinations()
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
		/// Gets all of the source endpoints actively routed to the given destination for video.
		/// </summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		public IEnumerable<EndpointInfo> GetActiveVideoEndpoints(IDestination destination)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			return GetActiveEndpoints(destination, eConnectionType.Video);
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

			return RoutingGraph.GetActiveSourceEndpoints(destination, flag, false, false);
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
		/// Routes the source to the displays and room audio.
		/// </summary>
		/// <param name="source"></param>
		public void Route(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			foreach (IDestination destination in GetDisplayDestinations())
				Route(source, destination, eConnectionType.Video);

			if (source.ConnectionType.HasFlag(eConnectionType.Audio))
				RouteAudio(source);
		}

		/// <summary>
		/// Routes the source to the destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public void Route(ISource source, IDestination destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			eConnectionType intersection = EnumUtils.GetFlagsIntersection(source.ConnectionType, destination.ConnectionType);

			foreach (eConnectionType flag in EnumUtils.GetFlagsExceptNone(intersection))
				Route(source, destination, flag);
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

			Connection firstOutput = outputs.FirstOrDefault();
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

			Connection firstOutput = RoutingGraph.Connections
			                                     .GetOutputConnections(sourceControl.Parent.Id,
			                                                           sourceControl.Id)
			                                     .Where(c => c.ConnectionType.HasFlag(eConnectionType.Audio))
			                                     .OrderBy(o => o.Source.Address)
			                                     .FirstOrDefault();

			if (firstOutput == null)
			{
				m_Room.Logger.AddEntry(eSeverity.Error, "Failed to find {0} output connection for {1}",
									   eConnectionType.Audio, sourceControl);
				return;
			}

			foreach (IDestination audioDestination in GetAudioDestinations())
			{
				Route(sourceControl.GetOutputEndpointInfo(firstOutput.Source.Address),
					  audioDestination, eConnectionType.Audio);
			}
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

			ConnectionPath path = IcdStopwatch.Profile(() =>

			                                           RoutingGraph.FindPath(source, destination, flag, m_Room.Id), "Pathfinding");
			if (path == null)
			{
				m_Room.Logger.AddEntry(eSeverity.Error, "Failed to find {0} path from {1} to {2}", flag, source, destination);
				return;
			}

			Route(path);
		}

		private void Route(EndpointInfo sourceEndpoint, IDestination destination, eConnectionType flag)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			ConnectionPath path = RoutingGraph.FindPath(sourceEndpoint, destination, flag, m_Room.Id);
			if (path == null)
			{
				m_Room.Logger.AddEntry(eSeverity.Error, "Failed to find {0} path from {1} to {2}", flag, sourceEndpoint, destination);
				return;
			}

			Route(path);
		}

		private void Route(ISource source, EndpointInfo destinationEndpoint, eConnectionType flag)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			ConnectionPath path = RoutingGraph.FindPath(source, destinationEndpoint, flag, m_Room.Id);
			if (path == null)
			{
				m_Room.Logger.AddEntry(eSeverity.Error, "Failed to find {0} path from {1} to {2}", flag, source, destinationEndpoint);
				return;
			}

			Route(path);
		}

		public void RouteAudio(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			foreach (IDestination destination in GetAudioDestinations())
				RouteAudio(source, destination);
		}

		/// <summary>
		/// Route the source audio only if there is currently no audio routed.
		/// </summary>
		/// <param name="source"></param>
		public void RouteAudioIfUnrouted(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (!GetActiveAudioSources().Any())
				RouteAudio(source);
		}

		/// <summary>
		/// Routes audio from the source to the given destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public void RouteAudio(ISource source, IDestination destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			Route(source, destination, eConnectionType.Audio);
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

		private void Route(ConnectionPath path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			bool oldRouting = m_Routing;
			m_Routing = true;

			IcdStopwatch.Profile(() => RoutingGraph.RoutePath(path, m_Room.Id),
			                     string.Format("Route - {0} {1}", path, path.ConnectionType));

			if (!oldRouting)
				m_Routing = false;

			IDeviceBase destinationDevice =
				m_Room.Core.Originators.GetChild<IDeviceBase>(path.DestinationEndpoint.Device);

			// Power on the destination
			IPowerDeviceControl powerControl = destinationDevice.Controls.GetControl<IPowerDeviceControl>();
			if (powerControl != null && !powerControl.IsPowered)
				powerControl.PowerOn();

			// Set the destination to the correct input
			int input = path.DestinationEndpoint.Address;
			IRouteInputSelectControl inputSelectControl =
				destinationDevice.Controls.GetControl<IRouteInputSelectControl>();

			if (inputSelectControl != null && inputSelectControl.ActiveInput != input)
				inputSelectControl.SetActiveInput(input);
		}

		/// <summary>
		/// Routes the given source to the VTC and starts the presentation.
		/// </summary>
		/// <param name="source"></param>
		public void RouteVtcPresentation(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IVideoConferenceDevice codec = GetCodec();
			if (codec == null)
				throw new InvalidOperationException("No codec available.");

			IPresentationControl presentation = codec.Controls.GetControl<IPresentationControl>();
			if (presentation == null)
				throw new InvalidOperationException("No presentation control available.");

			IVideoConferenceRouteControl control = codec.Controls.GetControl<IVideoConferenceRouteControl>();
			if (control == null)
				throw new InvalidOperationException("No routing control available.");

			// Get the content inputs
			int[] inputs = control.GetCodecInputs(eCodecInputType.Content).ToArray();
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
				EndpointInfo endpoint = control.GetInputEndpointInfo(input);

				// Is there a path?
				if (RoutingGraph.FindPath(source, endpoint, eConnectionType.Video, m_Room.Id) == null)
					continue;

				// Route the source video and audio to the codec
				Route(source, endpoint, eConnectionType.Video);
				Route(source, endpoint, eConnectionType.Audio);

				// Start the presentation
				presentation.StartPresentation(input);
				return;
			}

			m_Room.Logger.AddEntry(eSeverity.Error,
			                       "Failed to start presentation for {0} - Could not find a path to a Codec input configured for content.",
			                       source);
		}

		public ISource GetVtcPresentationSource()
		{
			IVideoConferenceDevice codec = GetCodec();
			if (codec == null)
				return null;

			IVideoConferenceRouteControl control = codec.Controls.GetControl<IVideoConferenceRouteControl>();
			if (control == null)
				return null;

			IPresentationControl presentation = codec.Controls.GetControl<IPresentationControl>();
			if (presentation == null)
				return null;

			int? activeInput = presentation.PresentationActiveInput;
			if (activeInput == null)
				return null;

			EndpointInfo? endpoint =
				RoutingGraph.GetActiveSourceEndpoint(control, (int)activeInput, eConnectionType.Video, false, false);

			return endpoint.HasValue
				       ? RoutingGraph.Sources.GetChildren(endpoint.Value, eConnectionType.Video).FirstOrDefault()
				       : null;
		}

		/// <summary>
		/// Unroutes the given audio source from all audio destinations.
		/// </summary>
		/// <param name="source"></param>
		public void UnrouteAudio(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			foreach (IDestination destination in GetAudioDestinations())
				RoutingGraph.Unroute(source, destination, eConnectionType.Audio, m_Room.Id);
		}

		/// <summary>
		/// Unroutes the given video source from all display destinations.
		/// </summary>
		/// <param name="source"></param>
		public void UnrouteVideo(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			foreach (IDestination destination in GetDisplayDestinations())
				RoutingGraph.Unroute(source, destination, eConnectionType.Video, m_Room.Id);
		}

		/// <summary>
		/// Fully unroutes the VTC from all displays.
		/// </summary>
		public void UnrouteVtc()
		{
			IVideoConferenceDevice codec = m_Room.Originators.GetInstanceRecursive<IVideoConferenceDevice>();
			if (codec == null)
				return;

			IRouteSourceControl sourceControl = codec.Controls.GetControl<IRouteSourceControl>();
			RoutingGraph.Unroute(sourceControl, EnumUtils.GetFlagsAllValue<eConnectionType>(), m_Room.Id);
		}

		/// <summary>
		/// Unroute all sources from all destinations.
		/// </summary>
		public void UnrouteAll()
		{
			UnrouteVtc();

			foreach (IDestination display in GetDisplayDestinations())
				RoutingGraph.Unroute(display, EnumUtils.GetFlagsAllValue<eConnectionType>(), m_Room.Id);

			foreach (IDestination audio in GetAudioDestinations())
				RoutingGraph.Unroute(audio, EnumUtils.GetFlagsAllValue<eConnectionType>(), m_Room.Id);
		}

		/// <summary>
		/// Unroute all sources except OSD from all destinations.
		/// </summary>
		public void UnrouteAllExceptOsd()
		{
			UnrouteVtc();

			foreach (IDestination display in GetDisplayDestinations())
			{
				IEnumerable<int> endpointIds =
					GetActiveVideoEndpoints(display).Select(e => e.Device)
					                                .Distinct()
													.ToArray();

				foreach (int endpointId in endpointIds)
				{
					OsdPanelDevice osd = m_Room.Core.Originators.GetChild(endpointId) as OsdPanelDevice;
					if (osd != null)
						continue;

					RoutingGraph.Unroute(display, EnumUtils.GetFlagsAllValue<eConnectionType>(), m_Room.Id);
				}
			}

			foreach (IDestination audio in GetAudioDestinations())
				RoutingGraph.Unroute(audio, EnumUtils.GetFlagsAllValue<eConnectionType>(), m_Room.Id);
		}

		#endregion

		#region Controls

		/// <summary>
		/// Gets the device for the given source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public IDeviceBase GetDevice(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return m_Room.Core.Originators.GetChild<IDeviceBase>(source.Device);
		}

		public bool CanControl(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			eControlOverride controlOverride = GetControlOverride(source);

			// If the control type is NOT default we return true.
			// This means we can show UI items for sources that don't necessarily have an IDeviceControl.
			return controlOverride != eControlOverride.Default || GetDeviceControl(source, eControlOverride.Default) != null;
		}

		public IDeviceControl GetDeviceControl(ISource source, eControlOverride controlOverride)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IDeviceBase device = GetDevice(source);
			return device == null ? null : GetDeviceControl(device, controlOverride);
		}

		public IDeviceControl GetDeviceControl(IDeviceBase device, eControlOverride controlOverride)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			switch (controlOverride)
			{
				case eControlOverride.Default:

					IDeviceControl dialer = GetDeviceControl(device, eControlOverride.Vtc) ??
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
					return device.Controls.GetControls<IDialingDeviceControl>().FirstOrDefault();
					
				case eControlOverride.WebConference:
					return null;
				
				default:
					throw new ArgumentOutOfRangeException("controlOverride");
			}

			return null;
		}

		public eControlOverride GetControlOverride(ISource source)
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

			routingGraph.OnRouteChanged += RoutingGraphOnRouteChanged;
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

			routingGraph.OnRouteChanged -= RoutingGraphOnRouteChanged;
		}

		/// <summary>
		/// Called when a switcher changes routing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RoutingGraphOnRouteChanged(object sender, SwitcherRouteChangeEventArgs args)
		{
			UpdateRoutingCache(args.Type);

			if (m_Routing)
				return;

			// If nothing is routed to a display we route the OSD
			OsdPanelDevice osd = m_Room.Core.Originators.GetChildren<OsdPanelDevice>().FirstOrDefault();
			if (osd == null)
				return;

			IRouteSourceControl sourceControl = osd.Controls.GetControl<IRouteSourceControl>();
			EndpointInfo sourceEndpoint = sourceControl.GetOutputEndpointInfo(1);

			IEnumerable<IDestination> emptyDestinations =
				GetCachedActiveVideoSources().Where(kvp => kvp.Value == null).Select(kvp => kvp.Key);

			foreach (IDestination destination in emptyDestinations)
				Route(sourceEndpoint, destination, eConnectionType.Video);
		}

		#endregion
	}
}
