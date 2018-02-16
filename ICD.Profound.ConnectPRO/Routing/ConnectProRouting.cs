using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Controls;
using ICD.Connect.Conferencing.Controls;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Displays;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRouting
	{
		private readonly IConnectProRoom m_Room;
		private readonly List<IDestination> m_Displays;
		private readonly List<IDestination> m_AudioDestinations;
		private readonly SafeCriticalSection m_CacheSection;

		/// <summary>
		/// Gets the routing graph from the core.
		/// </summary>
		private IRoutingGraph RoutingGraph { get { return m_Room.Core.GetRoutingGraph(); } }

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
			m_CacheSection = new SafeCriticalSection();
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
						                                           m_Room.Core.Originators.GetChild(d.Endpoint.Device)
						                                           is IDisplay)
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
							  .GetInstancesRecursive<IDestination>(d => d.ConnectionType == eConnectionType.Audio)
							  .OrderBy(d => d.Order)
							  .ThenBy(d => d.GetNameOrDeviceName(combine))
							  .Take(2);

					m_AudioDestinations.AddRange(audioDestinations);
				}

				return m_AudioDestinations.ToArray(m_AudioDestinations.Count);
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		public IEnumerable<ISource> GetActiveAudioSources()
		{
			return GetAudioDestinations().Select(destination => GetActiveSource(destination, eConnectionType.Audio))
			                             .Where(source => source != null);
		}

		[CanBeNull]
		public ISource GetActiveVideoSource(IDestination destination)
		{
			return GetActiveSource(destination, eConnectionType.Video);
		}

		[CanBeNull]
		public ISource GetActiveSource(IDestination destination, eConnectionType flag)
		{
			IRouteDestinationControl destinationControl =
				m_Room.Core.GetControl<IRouteDestinationControl>(destination.Endpoint.Device, destination.Endpoint.Control);

			EndpointInfo? info =
				RoutingGraph.GetActiveSourceEndpoint(destinationControl, destination.Endpoint.Address, flag,
													 false, false);
			if (info == null)
				return null;

			ISource source;
			return RoutingGraph.Sources.TryGetChild(info.Value, flag, out source) ? source : null;
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
				Route(source, destination);

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
			if (intersection == eConnectionType.None)
				return;

			Route(source.Endpoint, destination.Endpoint, intersection);
		}

		/// <summary>
		/// Routes the codec to all available displays.
		/// </summary>
		/// <param name="codecControl"></param>
		public void Route(CiscoCodecRoutingControl codecControl)
		{
			if (codecControl == null)
				throw new ArgumentNullException("codecControl");

			Connection[] outputs = RoutingGraph.Connections
			                                   .GetOutputConnections(codecControl.Parent.Id,
			                                                         codecControl.Id)
			                                   .OrderBy(o => o.Source.Address)
			                                   .ToArray();

			IDestination[] destinations = GetDisplayDestinations().ToArray();

			Connection firstOutput = outputs.FirstOrDefault();

			for (int index = 0; index < destinations.Length; index++)
			{
				IDestination destination = destinations[index];

				Connection output;
				if (!outputs.TryElementAt(index, out output))
					output = firstOutput;

				if (output == null)
					break;

				Route(output.Source, destination.Endpoint, eConnectionType.Video);
			}

			foreach (IDestination audioDestination in GetAudioDestinations())
			{
				Route(codecControl.GetOutputEndpointInfo(firstOutput.Source.Address),
				      audioDestination.Endpoint, eConnectionType.Audio);
			}
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

		public void RouteAudio(ISource source, IDestination destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			Route(source.Endpoint, destination.Endpoint, eConnectionType.Audio);
		}

		/// <summary>
		/// Routes the OSD to the displays.
		/// </summary>
		public void RouteOsd()
		{
			OsdPanelDevice osd = m_Room.Core.Originators.GetChildren<OsdPanelDevice>().FirstOrDefault();
			if (osd == null)
				return;

			IRouteSourceControl sourceControl = osd.Controls.GetControl<IRouteSourceControl>();
			EndpointInfo sourceEndpoint = sourceControl.GetOutputEndpointInfo(1);

			foreach (IDestination destination in GetDisplayDestinations())
			{
				RoutingGraph.Route(sourceEndpoint, destination.Endpoint, eConnectionType.Video, m_Room.Id);
			}
		}

		private void Route(EndpointInfo source, EndpointInfo destination, eConnectionType connectionType)
		{
			RoutingGraph.Route(source, destination, connectionType, m_Room.Id);

			IDisplay display = m_Room.Core.Originators.GetChild(destination.Device) as IDisplay;
			if (display == null)
				return;

			display.PowerOn();
			display.SetHdmiInput(destination.Address);
		}

		/// <summary>
		/// Fully unroutes the VTC from all displays.
		/// </summary>
		public void UnrouteVtc()
		{
			CiscoCodec codec = m_Room.Originators.GetInstanceRecursive<CiscoCodec>();
			if (codec == null)
				return;

			IRouteSourceControl sourceControl = codec.Controls.GetControl<IRouteSourceControl>();
			RoutingGraph.Unroute(sourceControl, EnumUtils.GetFlagsAllValue<eConnectionType>(), m_Room.Id);
		}

		/// <summary>
		/// Unroute all sources except OSD from all destinations.
		/// </summary>
		public void UnrouteAllExceptOsd()
		{
			UnrouteVtc();

			foreach (IDestination display in GetDisplayDestinations())
			{
				ISource source = GetActiveVideoSource(display);
				if (source == null)
					continue;

				OsdPanelDevice osd = m_Room.Core.Originators.GetChild(source.Endpoint.Device) as OsdPanelDevice;
				if (osd != null)
					continue;

				RoutingGraph.UnrouteDestination(display.Endpoint, EnumUtils.GetFlagsAllValue<eConnectionType>(), m_Room.Id);
			}

			foreach (IDestination audio in GetAudioDestinations())
				RoutingGraph.UnrouteDestination(audio.Endpoint, EnumUtils.GetFlagsAllValue<eConnectionType>(), m_Room.Id);
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

			return m_Room.Core.Originators.GetChild<IDeviceBase>(source.Endpoint.Device);
		}

		/// <summary>
		/// Returns the control that can be manipulated by the user, e.g. dialing control, tv tuner, etc.
		/// </summary>
		/// <returns></returns>
		public IDeviceControl GetDeviceControl(IDeviceBase device)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			// Dialer
			IDialingDeviceControl dialer = device.Controls.GetControls<IDialingDeviceControl>().FirstOrDefault();
			if (dialer != null)
				return dialer;

			// TV Tuner
			ITvTunerControl tvTuner = device.Controls.GetControls<ITvTunerControl>().FirstOrDefault();
			return tvTuner;
		}

		#endregion
	}
}
