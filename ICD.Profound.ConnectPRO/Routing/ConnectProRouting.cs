using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Conferencing.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Routing.PathFinding;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing.Masking;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRouting : IDisposable
	{
		private readonly IConnectProRoom m_Room;
		private readonly IRoutingGraph m_RoutingGraph;

		private readonly IPathFinder m_PathFinder;

		private readonly MaskedSourceInfoFactory m_MaskFactory;

		private readonly ConnectProRoutingSources m_Sources;
		private readonly ConnectProRoutingDestinations m_Destinations;
		private readonly ConnectProRoutingState m_State;

		#region Properties

		/// <summary>
		/// Gets the parent room.
		/// </summary>
		public IConnectProRoom Room { get { return m_Room; } }

		/// <summary>
		/// Gets the core routing graph.
		/// </summary>
		public IRoutingGraph RoutingGraph { get { return m_RoutingGraph; } }

		/// <summary>
		/// Gets source features for the current room.
		/// </summary>
		public ConnectProRoutingSources Sources { get { return m_Sources; } }

		/// <summary>
		/// Gets destination features for the current room.
		/// </summary>
		public ConnectProRoutingDestinations Destinations { get { return m_Destinations; } }

		/// <summary>
		/// Gets the routing state for the current room.
		/// </summary>
		public ConnectProRoutingState State { get { return m_State; } }

		/// <summary>
		/// Gets the pathfinder used for ConnectPro routing.
		/// </summary>
		public IPathFinder PathFinder { get { return m_PathFinder; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ConnectProRouting(IConnectProRoom room)
		{
			m_Room = room;
			m_RoutingGraph = m_Room.Core.GetRoutingGraph();

			m_PathFinder = new DefaultPathFinder(m_RoutingGraph, m_Room.Id);

			m_MaskFactory = new MaskedSourceInfoFactory(Room);

			m_Sources = new ConnectProRoutingSources(this);
			m_Destinations = new ConnectProRoutingDestinations(this);
			m_State = new ConnectProRoutingState(this);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_State.Dispose();
		}

		#region Methods

		/// <summary>
		/// Routes the source to all displays and room audio.
		/// </summary>
		/// <param name="source"></param>
		public void RouteToAllDisplays(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			var mask = m_MaskFactory.GetMaskedSourceInfo(source);
			if (mask == null)
			{
				RouteToAllDisplays(source, null);
				return;
			}

			IDestinationBase[] destinations = Destinations.GetVideoDestinations().ToArray();
			foreach (var destination in destinations)
				State.SetMaskedSource(destination, mask);
		}

		/// <summary>
		/// Routes the source to all displays and room audio.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="mask"></param>
		public void RouteToAllDisplays(ISource source, IMaskedSourceInfo mask)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (mask == null)
				State.ClearMaskedSources();

			IDestinationBase[] destinations = m_Destinations.GetVideoDestinations().ToArray();

			foreach (IDestinationBase destination in destinations)
				Route(source, destination, eConnectionType.Video);

			if (source.ConnectionType.HasFlag(eConnectionType.Audio))
				RouteToRoomAudio(source);
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
		public void RouteToDisplay(ISource source, IDestinationBase destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			var mask = m_MaskFactory.GetMaskedSourceInfo(source);
			if (mask == null)
				RouteToDisplay(source, destination, null);
			else
				State.SetMaskedSource(destination, mask);
		}

		/// <summary>
		/// Routes the source to the destination.
		/// Routes to room audio if there is no other audio source currently routed.
		/// Unroutes routed audio if associated video is unrouted.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <param name="mask"></param>
		public void RouteToDisplay(ISource source, IDestinationBase destination, IMaskedSourceInfo mask)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			if (mask == null)
				State.ClearMaskedSource(destination);

			Route(source, destination, eConnectionType.Video);

			if (!source.ConnectionType.HasFlag(eConnectionType.Audio))
				return;

			if (m_State.CanOverrideAudio(source, destination))
				RouteToRoomAudio(source);
			else
				RouteAudioIfNoAudioRouted(source);
		}

		/// <summary>
		/// Routes the codec to all available displays.
		/// </summary>
		/// <param name="source"></param>
		public void RouteVtc(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			var mask = m_MaskFactory.GetMaskedSourceInfo(source);
			if (mask != null)
			{
				var destinations = Destinations.GetVideoDestinations().ToList();
				foreach (var destination in destinations)
					State.SetMaskedSource(destination, mask);
			}
			else
			{
				RouteVtc(source, null);
			}
		}

		/// <summary>
		/// Routes the codec to all available displays.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="mask"></param>
		public void RouteVtc(ISource source, IMaskedSourceInfo mask)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			Connection[] outputs = RoutingGraph.Connections
			                                   .GetOutputConnections(source.Device,
			                                                         source.Control)
			                                   .Where(c => c.ConnectionType.HasFlag(eConnectionType.Video))
			                                   .OrderBy(o => o.Source.Address)
			                                   .ToArray();
			
			IDestinationBase[] destinations = m_Destinations.GetVideoDestinations().ToArray();

			Connection lastOutput = outputs.LastOrDefault();
			if (lastOutput == null)
			{
				m_Room.Logger.AddEntry(eSeverity.Error, "Failed to find {0} output connection for {1}",
				                       eConnectionType.Video, source);
				return;
			}
			
			IDeviceBase device = m_Room.Core.Originators.GetChild<IDeviceBase>(source.Device);
			IPresentationControl presentationControl = device.Controls.GetControl<IPresentationControl>();
			bool presenting = presentationControl != null && presentationControl.PresentationActive;

			for (int index = 0; index < destinations.Length; index++)
			{
				IDestinationBase destination = destinations[index];

				if (mask == null)
					State.ClearMaskedSource(destination);

				Connection output;
				if (m_Room.IsCombineRoom())
				{
					if (outputs.Length >= 2 && presenting)
						output = outputs[1];
					else
						output = outputs[0];
				}
				else if (!outputs.TryElementAt(index, out output))
					output = lastOutput;
				
				if (output == null)
					break;
				
				Route(output.Source, destination, eConnectionType.Video);
			}

			RouteAtc(source);
		}

		/// <summary>
		/// Routes the audio dialer to the audio destination.
		/// </summary>
		/// <param name="source"></param>
		public void RouteAtc(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IEnumerable<IDestination> singleAudioDestinations =
				m_Destinations.GetAudioDestinations()
				              .SelectMany(d => d.GetDestinations(eConnectionType.Audio));

			foreach (IDestination audioDestination in singleAudioDestinations)
			{
				// Edge case - Often the DSP is also the ATC, in which case we don't need to do any routing
				if (audioDestination.Device == source.Device)
					continue;

				Route(source, audioDestination, eConnectionType.Audio);
			}
		}

		/// <summary>
		/// Routes the given source for audio to all destinations with audio in the room.
		/// Unroutes any audio destinations with no path to the source.
		/// </summary>
		/// <param name="source"></param>
		public void RouteToRoomAudio(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			foreach (IDestinationBase destination in m_Destinations.GetAudioDestinations())
				RouteToRoomAudio(source, destination);
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
			RouteOsd(null);
		}

		/// <summary>
		/// Unroutes every destination that isn't currently showing the OSD.
		/// 
		/// Routes the OSD to the displays.
		/// 
		/// Powers off displays that have no OSD routed.
		/// </summary>
		public void RouteOsd(IMaskedSourceInfo mask)
		{
			// First unroute everything that isn't OSD
			UnrouteAllExceptOsd();

			OsdPanelDevice osd = m_Room.Originators.GetInstanceRecursive<OsdPanelDevice>();

			if (mask == null)
				State.ClearMaskedSources();
			
			IDestinationBase[] destinations = m_Destinations.GetVideoDestinations().ToArray();

			// Route the OSD or power off displays
			if (osd == null)
			{
				foreach (IDestinationBase destination in destinations)
				{
					// Power off the destination
					foreach (IDeviceBase device in destination.GetDevices())
						PowerDevice(device, false);
				}
			}
			else
			{
				IRouteSourceControl sourceControl = osd.Controls.GetControl<IRouteSourceControl>();
				EndpointInfo sourceEndpoint = sourceControl.GetOutputEndpointInfo(1);

				foreach (IDestinationBase destination in destinations)
				{
					Route(sourceEndpoint, destination, eConnectionType.Video);
				}
			}
		}

		/// <summary>
		/// Routes the given camera to the VTC route control and sets the active camera input.
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="routingControl"></param>
		public void RouteCameraToVtc(ICameraDevice camera, IVideoConferenceRouteControl routingControl)
		{
			if (camera == null)
				throw new ArgumentNullException("camera");

			if (routingControl == null)
				throw new ArgumentException("routeControl");

			IRouteSourceControl sourceControl = camera.Controls.GetControl<IRouteSourceControl>();
			if (sourceControl == null)
				throw new InvalidOperationException("Camera has no routing control");

			// Get the content inputs
			int[] inputs = routingControl.GetCodecInputs(eCodecInputType.Camera).ToArray();
			if (inputs.Length == 0)
			{
				m_Room.Logger.AddEntry(eSeverity.Error,
									   "Failed to route camera {0} - VTC has no inputs configured for camera.",
									   camera);
				return;
			}

			// Find the first input available
			foreach (int input in inputs)
			{
				EndpointInfo endpoint = routingControl.GetInputEndpointInfo(input);

				// Is there a path?
				bool hasPath =
					PathBuilder.FindPaths()
					           .From(sourceControl)
					           .To(endpoint)
					           .OfType(eConnectionType.Video)
					           .HasPaths(m_PathFinder);
				if (!hasPath)
					continue;

				// Route the source video and audio to the codec
				Route(sourceControl, endpoint, eConnectionType.Video);
				Route(sourceControl, endpoint, eConnectionType.Audio);

				// Start the presentation
				routingControl.SetCameraInput(input);
				return;
			}

			m_Room.Logger.AddEntry(eSeverity.Error,
								   "Failed to route camera {0} - Could not find a path to a VTC input configured for camera.",
								   camera);
		}

		/// <summary>
		/// Routes the given source to the VTC and starts the presentation.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="presentationControl"></param>
		public void RouteToVtcPresentation(ISource source, IPresentationControl presentationControl)
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
				                       "Failed to start presentation for {0} - VTC has no inputs configured for content.",
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
					           .OfType(source.ConnectionType)
					           .HasPaths(m_PathFinder);
				if (!hasPath)
					continue;

				// Route the source video and audio to the codec
				Route(source, endpoint, eConnectionType.Video);
				Route(source, endpoint, eConnectionType.Audio);

				// Start the presentation
				presentationControl.StartPresentation(input);
				return;
			}

			m_Room.Logger.AddEntry(eSeverity.Error,
			                       "Failed to start presentation for {0} - Could not find a path to a VTC input configured for content.",
			                       source);
		}

		/// <summary>
		/// Unroutes the active VTC presentation source and ends the presentation.
		/// </summary>
		/// <param name="presentationControl"></param>
		public void UnrouteAllFromVtcPresentation(IPresentationControl presentationControl)
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
		/// Returns true if there is a path from the source to the destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool HasPath(ISource source, IDestinationBase destination, eConnectionType type)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			return PathBuilder.FindPaths()
			                  .From(source)
			                  .To(destination.GetDestinations(type))
			                  .OfType(type)
			                  .HasPaths(m_PathFinder);
		}

		/// <summary>
		/// Returns true if the given source has a path to any of the room audio destinations.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public bool HasPathToRoomAudio(ISource source)
		{
			return Destinations.GetAudioDestinations().Any(d => HasPath(source, d, eConnectionType.Audio));
		}

		/// <summary>
		/// Returns true if any sources can be routed to all display destinations.
		/// </summary>
		/// <returns></returns>
		public bool SupportsSimpleMode()
		{
			return Sources
			       .GetRoomSourcesForUi()
			       .Any(s => Room.Routing
			                     .Destinations
			                     .GetVideoDestinations()
			                     .All(d => HasPath(s, d, eConnectionType.Video)));
		}

		/// <summary>
		/// Helper for turning a device on/off.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="power"></param>
		public void PowerDevice(IDeviceBase device, bool power)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			IPowerDeviceControl powerControl = device.Controls.GetControl<IPowerDeviceControl>();
			if (powerControl == null)
				return;

			switch (powerControl.PowerState)
			{
				case ePowerState.Unknown:
					break;

				case ePowerState.PowerOn:
				case ePowerState.Warming:
					if (power)
						return;
					break;

				case ePowerState.PowerOff:
				case ePowerState.Cooling:
					if (!power)
						return;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			if (power)
				powerControl.PowerOn();
			else
				powerControl.PowerOff();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Routes the source to the destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <param name="flag"></param>
		private void Route(ISource source, IDestinationBase destination, eConnectionType flag)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			IEnumerable<ConnectionPath> paths =
				PathBuilder.FindPaths()
				           .From(source)
				           .To(destination.GetDestinations(flag))
				           .OfType(flag)
				           .With(m_PathFinder);

			Route(paths);
		}

		private void Route(EndpointInfo sourceEndpoint, IDestinationBase destination, eConnectionType flag)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			IEnumerable<ConnectionPath> paths =
				PathBuilder.FindPaths()
				           .From(sourceEndpoint)
				           .To(destination.GetDestinations(flag))
				           .OfType(flag)
				           .With(m_PathFinder);

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
				           .With(m_PathFinder);

			Route(paths);
		}

		private void Route(IRouteSourceControl sourceControl, EndpointInfo destinationEndpoint, eConnectionType flag)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			IEnumerable<ConnectionPath> paths =
				PathBuilder.FindPaths()
				           .From(sourceControl)
				           .To(destinationEndpoint)
				           .OfType(flag)
				           .With(m_PathFinder);

			Route(paths);
		}

		/// <summary>
		/// Routes the given source for audio to all destinations with audio in the room.
		/// Unroutes any audio destinations with no path from the source.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		private void RouteToRoomAudio(ISource source, IDestinationBase destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			foreach (IDestination singleDestination in destination.GetDestinations(eConnectionType.Audio))
			{
				bool hasPath = HasPath(source, singleDestination, eConnectionType.Audio);

				if (hasPath)
					Route(source, singleDestination, eConnectionType.Audio);
				else
					RoutingGraph.Unroute(singleDestination, eConnectionType.Audio, m_Room.Id);
			}
		}

		/// <summary>
		/// Route the source audio only if there is currently no audio routed.
		/// </summary>
		/// <param name="source"></param>
		private void RouteAudioIfNoAudioRouted(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (!m_State.IsAudioRouted)
				RouteToRoomAudio(source);
		}

		private void Route(IEnumerable<ConnectionPath> paths)
		{
			if (paths == null)
				throw new ArgumentNullException("paths");

			IList<ConnectionPath> pathsList = paths as IList<ConnectionPath> ?? paths.ToArray();
			foreach (ConnectionPath path in pathsList)
				Route(path);
		}

		private void Route(ConnectionPath path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			IcdStopwatch.Profile(() => RoutingGraph.RoutePath(path, m_Room.Id), string.Format("Route - {0}", path));

			EndpointInfo destination = path.DestinationEndpoint;
			IDeviceBase destinationDevice =
				m_Room.Core.Originators.GetChild<IDeviceBase>(destination.Device);

			// Power on the destination
			PowerDevice(destinationDevice, true);

			// Set the destination to the correct input
			int input = destination.Address;
			IRouteInputSelectControl inputSelectControl =
				destinationDevice.Controls.GetControl<IRouteInputSelectControl>();

			if (inputSelectControl != null)
				inputSelectControl.SetActiveInput(input, path.ConnectionType);
		}

		/// <summary>
		/// Unroutes all audio.
		/// </summary>
		private void UnrouteAudio()
		{
			IDestination[] audioDestinations =
				m_Destinations.GetAudioDestinations()
				              .SelectMany(d => d.GetDestinations(eConnectionType.Audio))
				              .ToArray();

			ISource[] audioSources = m_State.GetCachedActiveAudioSources().ToArray();

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
			UnrouteAllExceptOsd(Destinations.GetVideoDestinations(), eConnectionType.Video);
			UnrouteAllExceptOsd(Destinations.GetAudioDestinations(), eConnectionType.Audio);
		}

		private void UnrouteAllExceptOsd(IEnumerable<IDestinationBase> destinations, eConnectionType flag)
		{
			Dictionary<EndpointInfo, IcdHashSet<EndpointInfo>> unrouteVideoEndpoints =
				destinations.SelectMany(d => d.GetEndpoints())
				            .Distinct()
				            .ToDictionary(e => e, e => GetNonOsdSourceEndpoints(e, flag).ToIcdHashSet());

			foreach (KeyValuePair<EndpointInfo, IcdHashSet<EndpointInfo>> pair in unrouteVideoEndpoints)
			{
				foreach (EndpointInfo sourceEndpoint in pair.Value)
					RoutingGraph.Unroute(sourceEndpoint, pair.Key, flag, m_Room.Id);
			}
		}

		private IEnumerable<EndpointInfo> GetNonOsdSourceEndpoints(EndpointInfo destinationEndpoint,
		                                                           eConnectionType flag)
		{
			return RoutingGraph.RoutingCache
			                   .GetSourceEndpointsForDestinationEndpoint(destinationEndpoint, flag)
			                   .Where(s =>
				                          !(m_Room.Core.Originators.GetChild(s.Device) is OsdPanelDevice) ||
				                          !m_Room.Originators.ContainsRecursive(s.Device))
			                   .ToIcdHashSet();
		}

		#endregion
	}
}
