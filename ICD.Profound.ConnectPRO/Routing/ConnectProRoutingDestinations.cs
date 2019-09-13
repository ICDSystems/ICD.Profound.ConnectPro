using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;

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

		#region Methods

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
						m_Routing.Room
						         .Originators
						         .GetInstancesRecursive<IDestination>()
						         .Where(d => !d.Hide)
						         .Where(d => d.ConnectionType.HasFlag(eConnectionType.Video))
						         .Where(d => m_Routing.Room.Core.Originators.GetChild(d.Device) is IDisplay)
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
						m_Routing.Room
						         .Originators
						         .GetInstancesRecursive<IDestination>()
						         .Where(d => !d.Hide)
						         .Where(d => d.ConnectionType.HasFlag(eConnectionType.Audio))
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

		#endregion
	}
}
