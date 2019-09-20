using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing.Masking;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRoutingState : IDisposable
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

		// Keeps track of the active routing states
		private readonly Dictionary<IDestinationBase, IcdHashSet<ISource>> m_VideoRoutingCache;
		private readonly IcdHashSet<ISource> m_AudioRoutingCache;

		// Keeps track of the processing state for each source.
		private readonly Dictionary<ISource, eSourceState> m_SourceRoutedStates;
		private readonly Dictionary<IDestinationBase, ProcessingSourceInfo> m_ProcessingSources;
		private readonly Dictionary<IDestinationBase, IMaskedSourceInfo> m_MaskedSources;

		private readonly SafeCriticalSection m_CacheSection;

		private readonly ConnectProRouting m_Routing;
		private readonly IRoutingGraph m_RoutingGraph;

		/// <summary>
		/// Returns true if an audio source is currently routed to one or more audio destination.
		/// </summary>
		public bool IsAudioRouted { get { return m_CacheSection.Execute(() => m_AudioRoutingCache.Count) > 0; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProRoutingState(ConnectProRouting routing)
		{
			m_VideoRoutingCache = new Dictionary<IDestinationBase, IcdHashSet<ISource>>();
			m_AudioRoutingCache = new IcdHashSet<ISource>();
			m_SourceRoutedStates = new Dictionary<ISource, eSourceState>();
			m_ProcessingSources = new Dictionary<IDestinationBase, ProcessingSourceInfo>();
			m_MaskedSources = new Dictionary<IDestinationBase, IMaskedSourceInfo>();

			m_Routing = routing;
			m_RoutingGraph = m_Routing.RoutingGraph;

			m_CacheSection = new SafeCriticalSection();

			Subscribe(m_Routing.Room);
			Subscribe(m_RoutingGraph);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			OnDisplaySourceChanged = null;
			OnAudioSourceChanged = null;
			OnSourceRoutedChanged = null;

			Unsubscribe(m_Routing.Room);
			Unsubscribe(m_RoutingGraph);
		}

		/// <summary>
		/// Returns true if we can override active audio when routing the given source to the given destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public bool CanOverrideAudio(ISource source, IDestinationBase destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			m_CacheSection.Enter();

			try
			{
				// Nothing is currently routed for audio so we can override
				IcdHashSet<ISource> activeAudio = m_AudioRoutingCache.ToIcdHashSet();
				if (activeAudio.Count == 0)
					return true;

				// Is there another source routed for audio going to another display?
				foreach (KeyValuePair<IDestinationBase, IcdHashSet<ISource>> kvp in m_VideoRoutingCache)
				{
					// Skip the display we are currently routing to
					IDestinationBase display = kvp.Key;
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

		#region Sources

		[NotNull]
		public IEnumerable<KeyValuePair<IDestinationBase, IcdHashSet<ISource>>> GetRealActiveVideoSources()
		{
			return
				m_CacheSection.Execute(() =>
									   m_VideoRoutingCache.ToDictionary(kvp => kvp.Key, kvp => new IcdHashSet<ISource>(kvp.Value)));
		}

		[NotNull]
		public IEnumerable<KeyValuePair<IDestinationBase, IcdHashSet<ISource>>> GetFakeActiveVideoSources()
		{
			m_CacheSection.Enter();
			try
			{
				Dictionary<IDestinationBase, IcdHashSet<ISource>> cache =
					m_VideoRoutingCache.ToDictionary(kvp => kvp.Key, kvp => new IcdHashSet<ISource>(kvp.Value));

				foreach (KeyValuePair<IDestinationBase, ProcessingSourceInfo> kvp in m_ProcessingSources.ToList())
				{
					IcdHashSet<ISource> destinationCache = cache.GetOrAddNew(kvp.Key);
					
					if (kvp.Value.Source != null)
						destinationCache.Add(kvp.Value.Source);
				}

				foreach (KeyValuePair<IDestinationBase, IMaskedSourceInfo> kvp in m_MaskedSources.ToList())
				{
					IcdHashSet<ISource> destinationCache = cache.GetOrAddNew(kvp.Key);

					if (kvp.Value.Source != null)
						destinationCache.Add(kvp.Value.Source);
				}

				return cache;
			}
			finally
			{
				m_CacheSection.Leave();
			}
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
		private void ClearProcessingSource([NotNull] IDestinationBase destination, [NotNull] ISource source)
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
			IDestinationBase destination = m_Routing.Room == null ? null : m_Routing.Destinations.GetVideoDestinations().FirstOrDefault();
			if (destination == null)
				return;

			SetProcessingSource(destination, source);
		}

		/// <summary>
		/// Sets the processing source for the given destination.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="source"></param>
		public void SetProcessingSource([NotNull] IDestinationBase destination, [NotNull] ISource source)
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
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		#endregion

		#region Masked Sources

		public void SetMaskedSource(IDestinationBase destination, IMaskedSourceInfo mask)
		{
			m_CacheSection.Enter();
			try
			{
				IMaskedSourceInfo currentMask;
				if (m_MaskedSources.TryGetValue(destination, out currentMask))
				{
					if (currentMask == mask)
						return;

					ClearMaskedSource(destination);
				}

				if (mask == null)
					return;

				m_MaskedSources[destination] = mask;
				UpdateSourceRoutedStates();
			}
			finally
			{
				m_CacheSection.Leave();
			}

			OnDisplaySourceChanged.Raise(this);
		}

		public void ClearMaskedSource(IDestinationBase destination)
		{
			m_CacheSection.Enter();
			try
			{
				IMaskedSourceInfo maskToRemove;
				if (!m_MaskedSources.TryGetValue(destination, out maskToRemove))
					return;

				m_MaskedSources.Remove(destination);
				maskToRemove.Dispose();

				UpdateSourceRoutedStates();
			}
			finally
			{
				m_CacheSection.Leave();
			}

			OnDisplaySourceChanged.Raise(this);
		}

		/// <summary>
		/// Clears the processing source for each display destination.
		/// </summary>
		public void ClearMaskedSources()
		{
			m_CacheSection.Enter();

			try
			{
				IMaskedSourceInfo[] clear = m_MaskedSources.Values.Where(p => p != null).ToArray();
				if (clear.Length == 0)
					return;

				foreach (IMaskedSourceInfo mask in clear)
					mask.Dispose();

				m_MaskedSources.Clear();

				UpdateSourceRoutedStates();
			}
			finally
			{
				m_CacheSection.Leave();
			}

			OnDisplaySourceChanged.Raise(this);
		}

		#endregion

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
				IcdHashSet<ISource> activeAudio =
					m_Routing.Destinations
					         .GetAudioDestinations()
							 .SelectMany(d => d.GetDestinations())
					         .SelectMany(d => m_Routing.RoutingGraph
					                                   .RoutingCache
					                                   .GetSourcesForDestination(d, eConnectionType.Audio))
					         .ToIcdHashSet();

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
				Dictionary<IDestinationBase, IcdHashSet<ISource>> routing =
					m_Routing.Destinations
					         .GetVideoDestinations()
					         .ToDictionary(d => d,
					                       d => d.GetDestinations()
					                             .SelectMany(d2 => m_Routing.RoutingGraph
					                                                        .RoutingCache
					                                                        .GetSourcesForDestination(d2, eConnectionType.Video, false,
					                                                                                  true))
					                             .ToIcdHashSet());
				
				bool change = false;
				foreach (KeyValuePair<IDestinationBase, IcdHashSet<ISource>> kvp in routing)
				{
					IcdHashSet<ISource> cache = m_VideoRoutingCache.GetOrAddNew(kvp.Key);
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
				foreach (KeyValuePair<IDestinationBase, IcdHashSet<ISource>> kvp in m_VideoRoutingCache)
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

				foreach (ISource source in m_MaskedSources.Values.Where(m => m != null && m.Source != null).Select(m => m.Source))
					if (!routedSources.ContainsKey(source) || routedSources[source] != eSourceState.Active)
						routedSources[source] = eSourceState.Masked;

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

		#region RoutingGraph Callbacks

		/// <summary>
		/// Subscribe to the routing events.
		/// </summary>
		/// <param name="routingGraph"></param>
		private void Subscribe(IRoutingGraph routingGraph)
		{
			if (routingGraph == null)
				return;

			routingGraph.RoutingCache.OnSourceDestinationRouteChanged += RoutingCacheOnRouteToDestinationChanged;
			routingGraph.RoutingCache.OnDestinationEndpointActiveChanged += RoutingCacheOnDestinationEndpointActiveChanged;
		}

		/// <summary>
		/// Unsubscribe from the routing events.
		/// </summary>
		/// <param name="routingGraph"></param>
		private void Unsubscribe(IRoutingGraph routingGraph)
		{
			if (routingGraph == null)
				return;

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
	}
}