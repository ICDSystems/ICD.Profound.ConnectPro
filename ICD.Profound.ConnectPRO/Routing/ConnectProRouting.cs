using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Displays;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Settings;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRouting
	{
		private readonly IConnectProRoom m_Room;

		/// <summary>
		/// Gets the routing graph from the core.
		/// </summary>
		private IRoutingGraph RoutingGraph { get { return m_Room.Core.GetRoutingGraph(); } }

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
			             .Where(d => m_Room.Core.Originators.GetChild(d.Endpoint.Device) is IDisplay)
			             .OrderBy(d => d.Order)
			             .ThenBy(d => d.GetNameOrDeviceName(combine))
			             .Take(2);
		}

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

		/// <summary>
		/// Routes the source to the displays.
		/// </summary>
		/// <param name="source"></param>
		public void Route(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			foreach (IDestination destination in GetDisplayDestinations())
				Route(source, destination);
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

			RoutingGraph.Route(source.Endpoint, destination.Endpoint, eConnectionType.Audio | eConnectionType.Video, m_Room.Id);

			IOriginator device = m_Room.Core.Originators.GetChild(destination.Endpoint.Device);
			IDisplay display = device as IDisplay;

			if (display != null)
			{
				display.PowerOn();
				display.SetHdmiInput(destination.Endpoint.Address);
			}
		}

		/// <summary>
		/// Gets the displays that the given source is actively routed to.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public IEnumerable<IDestination> GetActiveDisplayDestinations(ISource source)
		{
			IRouteSourceControl sourceControl =
				m_Room.Core.GetControl<IRouteSourceControl>(source.Endpoint.Device, source.Endpoint.Control);

			IEnumerable<EndpointInfo> destinations =
				RoutingGraph.GetActiveDestinationEndpoints(sourceControl, source.Endpoint.Address, eConnectionType.Video, false,
				                                           false);

			IDestination[] displayDestinations = GetDisplayDestinations().ToArray();

			return destinations.Where(d => displayDestinations.Any(disp => disp.Endpoint == d))
			                   .Select(d => displayDestinations.First(disp => disp.Endpoint == d));
		}
	}
}
