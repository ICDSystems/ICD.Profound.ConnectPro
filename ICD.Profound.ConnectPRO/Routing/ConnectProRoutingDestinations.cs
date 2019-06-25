using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRoutingDestinations
	{
		private readonly List<IDestination> m_Displays;
		private readonly List<IDestination> m_AudioDestinations;
		private readonly ConnectProRouting m_Routing;
		private readonly SafeCriticalSection m_CacheSection;

		#region Properties

		/// <summary>
		/// Returns true if the room contains more than 1 display.
		/// </summary>
		public bool IsMultiDisplayRoom { get { return DisplayDestinationsCount > 1; } }

		/// <summary>
		/// Returns true if the room has an audio destination.
		/// </summary>
		public bool RoomHasAudio { get { return GetAudioDestinations().Any(); } }

		/// <summary>
		/// Gets the number of display destinations.
		/// </summary>
		public int DisplayDestinationsCount { get { return GetDisplayDestinations().Count(); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="routing"></param>
		public ConnectProRoutingDestinations(ConnectProRouting routing)
		{
			m_Displays = new List<IDestination>();
			m_AudioDestinations = new List<IDestination>();

			m_Routing = routing;
			m_CacheSection = new SafeCriticalSection();
		}

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
					bool combine = m_Routing.Room.IsCombineRoom();

					IEnumerable<IDestination> displays =
						m_Routing.Room.Originators
							  .GetInstancesRecursive<IDestination>(d =>
																	   m_Routing.Room.Core.Originators.GetChild(d.Device)
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
					bool combine = m_Routing.Room.IsCombineRoom();

					IEnumerable<IDestination> audioDestinations =
						m_Routing.Room.Originators
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

			return m_Routing.RoutingGraph.RoutingCache.GetSourceEndpointsForDestination(destination, flag, false, true);
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

			return GetActiveEndpoints(destination, flag).SelectMany(e => m_Routing.RoutingGraph.Sources.GetChildren(e, flag))
														.Distinct();
		}

		#endregion
	}
}
