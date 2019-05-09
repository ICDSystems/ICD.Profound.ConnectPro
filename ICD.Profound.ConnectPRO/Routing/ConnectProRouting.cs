using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Conferencing.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Routing.PathFinding;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRouting : IDisposable
	{
		private readonly IConnectProRoom m_Room;
		private readonly IRoutingGraph m_RoutingGraph;

		private readonly IPathFinder m_PathFinder;

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
		/// Routes the source to the display and room audio.
		/// </summary>
		/// <param name="source"></param>
		public void RouteSingleDisplay(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			// Functionally the same thing if there is only one display
			RouteAllDisplays(source);
		}

		/// <summary>
		/// Routes the source to all displays and room audio.
		/// </summary>
		/// <param name="source"></param>
		public void RouteAllDisplays(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IDestination[] destinations = m_Destinations.GetDisplayDestinations().ToArray();

			foreach (IDestination destination in destinations)
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

			if (m_State.CanOverrideAudio(source, destination))
				RouteAudio(source);
			else
				RouteAudioIfNoAudioRouted(source);
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

			IDestination[] destinations = m_Destinations.GetDisplayDestinations().ToArray();

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

			foreach (IDestination audioDestination in m_Destinations.GetAudioDestinations())
			{
				// Edge case - Often the DSP is also the ATC, in which case we don't need to do any routing
				if (audioDestination.Device == sourceControl.Parent.Id)
					continue;

				Route(sourceControl, audioDestination, eConnectionType.Audio);
			}
		}

		public void RouteAudio(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			foreach (IDestination destination in m_Destinations.GetAudioDestinations())
				RouteAudio(source, destination);
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
				foreach (IDestination destination in m_Destinations.GetDisplayDestinations())
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

				foreach (IDestination destination in m_Destinations.GetDisplayDestinations())
					Route(sourceEndpoint, destination, eConnectionType.Video);
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
					           .With(m_PathFinder)
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

		#endregion

		#region Private Methods

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
				           .With(m_PathFinder);

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
				           .With(m_PathFinder);

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

			if (!m_State.IsAudioRouted)
				RouteAudio(source);
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
		/// Unroutes all audio.
		/// </summary>
		private void UnrouteAudio()
		{
			IDestination[] audioDestinations = m_Destinations.GetAudioDestinations().ToArray();
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
			UnrouteAllVideoExceptOsd();
			UnrouteAllAudioExceptOsd();
		}

		private void UnrouteAllVideoExceptOsd()
		{
			// Create a copy of the video routing cache, removing any OSD sources.
			Dictionary<IDestination, IcdHashSet<ISource>> activeVideoSources =
				m_State.GetCachedActiveVideoSources()
				       .ToDictionary(kvp => kvp.Key,
				                     kvp =>
				                     kvp.Value
				                        .Where(source => !(m_Room.Core.Originators.GetChild(source.Device) is OsdPanelDevice))
				                        .ToIcdHashSet());

			// Keep routing out of the critical section above
			foreach (KeyValuePair<IDestination, IcdHashSet<ISource>> pair in activeVideoSources)
				foreach (ISource videoSource in pair.Value)
					RoutingGraph.Unroute(videoSource, pair.Key, eConnectionType.Video, m_Room.Id);
		}

		private void UnrouteAllAudioExceptOsd()
		{
			// Create a copy of the audio routing cache, removing any OSD sources.
			IcdHashSet<ISource> activeAudioSources =
				m_State.GetCachedActiveAudioSources()
				       .Where(source => !(m_Room.Core.Originators.GetChild(source.Device) is
				                          OsdPanelDevice))
				       .ToIcdHashSet();

			// Keep routing out of the critical section above
			foreach (IDestination destination in m_Destinations.GetAudioDestinations())
				foreach (ISource source in activeAudioSources)
					RoutingGraph.Unroute(source, destination, eConnectionType.Audio, m_Room.Id);
		}

		#endregion
	}
}
