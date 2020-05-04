using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Conferencing.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.PathFinding;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Profound.ConnectPROCommon.Rooms.Combine;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPROCommon.Routing
{
	public sealed class ConnectProRoutingSources
	{
		[NotNull]
		private readonly ConnectProRouting m_Routing;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="routing"></param>
		public ConnectProRoutingSources([NotNull] ConnectProRouting routing)
		{
			if (routing == null)
				throw new ArgumentNullException("routing");

			m_Routing = routing;
		}

		#region Sources

		/// <summary>
		/// Returns all of the sources available in the room.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public IEnumerable<ISource> GetRoomSources()
		{
			return m_Routing.Room
			                .Originators
			                .GetInstancesRecursive<ISource>()
			                .OrderBy(s => s.Order);
		}

		/// <summary>
		/// Returns all of the sources available in the room unless:
		///		- The source is hidden
		///		- The source appearance does not intersect the given mask
		///		- The source is not an audio or video source
		///		- The source is a video source and:
		///			- In a room with a single display or combined simple mode the source can not be routed to all displays.
		///			- In a room with multiple displays or combined advanced mode the source can not be routed to any destination.
		/// </summary>
		/// <param name="mask"></param>
		/// <returns></returns>
		[NotNull]
		public IEnumerable<ISource> GetRoomSourcesForUi(eSourceAppearance mask)
		{
			IDestinationBase[] videoDestinations = m_Routing.Destinations.GetVideoDestinations().ToArray();
			
			bool multipleDisplays = m_Routing.Destinations.IsMultiDisplayRoom;
				
			// If this is a combined room in simple mode treat it like a single display destination
			ConnectProCombineRoom combineRoom = m_Routing.Room as ConnectProCombineRoom;
			if (combineRoom != null && combineRoom.CombinedAdvancedMode == eCombineAdvancedMode.Simple)
				multipleDisplays = false;

			return GetRoomSources().Where(s =>
			{
				// Hidden
				if (s.Hide)
					return false;

				// Appearance
				ConnectProSource connectProSource = s as ConnectProSource;
				if (connectProSource != null &&
				    EnumUtils.GetFlagsIntersection(connectProSource.Appearance, mask) == eSourceAppearance.None)
					return false;

				// Not audio or video
				if (EnumUtils.GetFlagsIntersection(s.ConnectionType, eConnectionType.Audio | eConnectionType.Video) ==
				    eConnectionType.None)
					return false;

				// Not a video source so we're done with the extra validation
				if (!s.ConnectionType.HasFlag(eConnectionType.Video))
					return true;

				return multipleDisplays
					? videoDestinations.Any(d => m_Routing.HasPath(s, d, eConnectionType.Video))
					: videoDestinations.All(d => m_Routing.HasPath(s, d, eConnectionType.Video));
			});
		}

		/// <summary>
		/// Returns all of the sources available in the room that are tagged for sharing and can be
		/// routed to the given VTC routing control.
		/// </summary>
		/// <param name="routeControl"></param>
		/// <returns></returns>
		[NotNull]
		public IEnumerable<ISource> GetRoomSourcesForPresentation([NotNull] IVideoConferenceRouteControl routeControl)
		{
			if (routeControl == null)
				throw new ArgumentNullException("routeControl");

			// Get the content inputs
			int[] inputs = routeControl.GetCodecInputs(eCodecInputType.Content).ToArray();
			if (inputs.Length == 0)
				return Enumerable.Empty<ISource>();

			return GetRoomSources().Where(s =>
			{
				// Hidden
				if (s.Hide)
					return false;

				// Not video
				if (!s.ConnectionType.HasFlag(eConnectionType.Video))
					return false;

				// Not sharable
				ConnectProSource cpSource = s as ConnectProSource;
				if (cpSource != null && !cpSource.Appearance.HasFlag(eSourceAppearance.Presentation))
					return false;

				// Can it be routed to a content input on the route control?
				foreach (int input in inputs)
				{
					EndpointInfo endpoint = routeControl.GetInputEndpointInfo(input);

					// Is there a path?
					bool hasPath =
						PathBuilder.FindPaths()
						           .From(s)
						           .To(endpoint)
						           .OfType(eConnectionType.Video)
						           .HasPaths(m_Routing.PathFinder);
					if (hasPath)
						return true;
				}

				return false;
			});
		}

		/// <summary>
		/// Gets the room for the given source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		[NotNull]
		public IRoom GetRoomForSource([NotNull] ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			// Is the source immediately in this room?
			if (m_Routing.Room.Originators.Contains(source.Id))
				return m_Routing.Room;

			// Is the source in one of our child rooms?
			foreach (IRoom room in m_Routing.Room.GetRooms())
				if (room.Originators.Contains(source.Id))
					return room;

			// This probably shouldn't happen - Get the first room in the core containing the source
			return m_Routing.Room
			                .Core
			                .Originators
			                .GetChildren<IRoom>()
			                .FirstOrDefault(r => r.Originators.Contains(source.Id), m_Routing.Room);
		}

		/// <summary>
		/// Gets the sources currently being presented.
		/// </summary>
		/// <param name="presentationControl"></param>
		/// <returns></returns>
		[NotNull]
		public IEnumerable<ISource> GetVtcPresentationSources([NotNull] IPresentationControl presentationControl)
		{
			if (presentationControl == null)
				throw new ArgumentNullException("presentationControl");

			IVideoConferenceRouteControl control =
				presentationControl.Parent.Controls.GetControl<IVideoConferenceRouteControl>();
			if (control == null)
				throw new InvalidOperationException("No routing control available.");

			int? activeInput = presentationControl.PresentationActiveInput;
			if (activeInput == null)
				return Enumerable.Empty<ISource>();

			return m_Routing.RoutingGraph
			                .RoutingCache
			                .GetSourcesForDestinationEndpoint(control.GetInputEndpointInfo((int)activeInput),
			                                                  eConnectionType.Video);
		}

		#endregion

		#region Controls

		/// <summary>
		/// Gets the sources for the given device control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		[NotNull]
		public IEnumerable<ISource> GetSources([NotNull] IDeviceControl control)
		{
			if (control == null)
				throw new ArgumentNullException("control");

			return m_Routing.RoutingGraph
				.Sources
				.GetChildrenForDevice(control.Parent.Id)
				.Where(s => GetDeviceControl(s) == control);
		}

		/// <summary>
		/// Gets the device for the given source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		[NotNull]
		private static IDevice GetDevice([NotNull] ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return ServiceProvider.GetService<ICore>()
			                      .Originators
			                      .GetChild<IDevice>(source.Device);
		}

		public static bool CanControl([NotNull] ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			eControlOverride controlOverride = GetControlOverride(source);

			// If the control type is NOT default we return true.
			// This means we can show UI items for sources that don't necessarily have an IDeviceControl.
			return controlOverride != eControlOverride.Default || GetDeviceControl(source, eControlOverride.Default) != null;
		}

		[CanBeNull]
		public static IDeviceControl GetDeviceControl([NotNull] ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			eControlOverride controlOverride = GetControlOverride(source);
			return GetDeviceControl(source, controlOverride);
		}

		[CanBeNull]
		public static IDeviceControl GetDeviceControl([NotNull] ISource source, eControlOverride controlOverride)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IDevice device = GetDevice(source);
			return GetDeviceControl(device, controlOverride);
		}

		[CanBeNull]
		private static IDeviceControl GetDeviceControl([NotNull] IDevice device, eControlOverride controlOverride)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			switch (controlOverride)
			{
				case eControlOverride.Default:

					IDeviceControl dialer =
						GetDeviceControl(device, eControlOverride.WebConference) ??
						GetDeviceControl(device, eControlOverride.Vtc) ??
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
					return device.Controls.GetControls<ITraditionalConferenceDeviceControl>().FirstOrDefault();

				case eControlOverride.WebConference:
					return device.Controls.GetControls<IWebConferenceDeviceControl>().FirstOrDefault();

				default:
					throw new ArgumentOutOfRangeException("controlOverride");
			}

			return null;
		}

		public static eControlOverride GetControlOverride([NotNull] ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			ConnectProSource connectProSource = source as ConnectProSource;
			return connectProSource == null ? eControlOverride.Default : connectProSource.ControlOverride;
		}

		#endregion
	}
}
