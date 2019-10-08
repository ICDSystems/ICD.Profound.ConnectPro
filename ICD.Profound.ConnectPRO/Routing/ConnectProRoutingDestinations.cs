using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Groups.Endpoints.Destinations;
using ICD.Connect.Settings.Originators;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRoutingDestinations
	{
		private readonly List<IDestinationBase> m_VideoDestinations;
		private readonly List<IDestinationBase> m_AudioDestinations;
		private readonly ConnectProRouting m_Routing;
		private readonly SafeCriticalSection m_CacheSection;

		#region Properties

		/// <summary>
		/// Returns true if the room contains more than 1 display.
		/// </summary>
		public bool IsMultiDisplayRoom { get { return DisplayDestinationsCount > 1; } }

		/// <summary>
		/// Gets the number of display destinations.
		/// </summary>
		public int DisplayDestinationsCount { get { return GetVideoDestinations().Count(); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="routing"></param>
		public ConnectProRoutingDestinations(ConnectProRouting routing)
		{
			m_VideoDestinations = new List<IDestinationBase>();
			m_AudioDestinations = new List<IDestinationBase>();

			m_Routing = routing;
			m_CacheSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Returns the first two ordered display destinations for the room.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IDestinationBase> GetVideoDestinations()
		{
			m_CacheSection.Enter();

			try
			{
				if (m_VideoDestinations.Count == 0)
				{
					IEnumerable<IDestinationBase> videoDestinations = GetDestinations(eConnectionType.Video);
					m_VideoDestinations.AddRange(videoDestinations);
				}

				return m_VideoDestinations.ToArray();
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
		public IEnumerable<IDestinationBase> GetAudioDestinations()
		{
			m_CacheSection.Enter();

			try
			{
				if (m_AudioDestinations.Count == 0)
				{
					IEnumerable<IDestinationBase> audioDestinations = GetDestinations(eConnectionType.Audio);
					m_AudioDestinations.AddRange(audioDestinations);
				}

				return m_AudioDestinations.ToArray();
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Gets the room for the given destination.
		/// </summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		[NotNull]
		public IRoom GetRoomForDestination(IDestinationBase destination)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			// Is the destination immediately in this room?
			if (m_Routing.Room.Originators.Contains(destination.Id))
				return m_Routing.Room;

			// Is the destination in one of our child rooms?
			foreach (IRoom room in m_Routing.Room.GetRooms())
				if (room.Originators.Contains(destination.Id))
					return room;

			// This probably shouldn't happen - Get the first room in the core containing the destination
			return m_Routing.Room
			                .Core
			                .Originators
			                .GetChildren<IRoom>()
			                .FirstOrDefault(r => r.Originators.Contains(destination.Id), m_Routing.Room);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns an ordered sequence of destination groups and any loose destinations.
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		private IEnumerable<IDestinationBase> GetDestinations(eConnectionType flag)
		{
			bool combine = m_Routing.Room.IsCombineRoom();

			IDestinationGroup[] groups =
				m_Routing.Room
				         .Originators
						 .GetInstancesRecursive<IDestinationGroup>()
				         .Where(d => !d.Hide)
				         .Where(d => d.ConnectionType.HasFlag(flag))
						 .ToArray();

			IEnumerable<IDestination> destinations =
				m_Routing.Room
						 .Originators
						 .GetInstancesRecursive<IDestination>()
						 .Where(d => !d.Hide)
						 .Where(d => d.ConnectionType.HasFlag(flag))
						 .Where(d => !groups.Any(g => g.Contains(d)));

			return groups.Concat(destinations.Cast<IDestinationBase>())
			             .OrderBy(d => d.Order)
			             .ThenBy(d => d.GetName(combine));
		}

		#endregion
	}
}
