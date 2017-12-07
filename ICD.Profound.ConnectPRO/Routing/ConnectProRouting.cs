using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Displays;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRouting
	{
		private readonly IConnectProRoom m_Room;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ConnectProRouting(IConnectProRoom room)
		{
			m_Room = room;
		}

		/// <summary>
		/// Returns the first two ordered display destinations for the room.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IDestination> GetDisplayDestinations()
		{
			bool combine = m_Room.IsCombineRoom();

			return m_Room.Originators
			             .GetInstancesRecursive<IDestination>()
			             .Where(d => d.ConnectionType.HasFlag(eConnectionType.Video))
						 .Where(d => m_Room.Core.Originators.GetChild(d.Endpoint.Device) is IDisplay)
			             .OrderBy(d => d.Order)
			             .ThenBy(d => d.GetNameOrDeviceName(combine))
						 .Take(2);
		}
	}
}
