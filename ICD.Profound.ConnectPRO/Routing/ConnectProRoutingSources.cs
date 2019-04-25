using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Services;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRoutingSources
	{
		private readonly ConnectProRouting m_Routing;
		private readonly SafeCriticalSection m_CacheSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="routing"></param>
		public ConnectProRoutingSources(ConnectProRouting routing)
		{
			m_Routing = routing;
			m_CacheSection = new SafeCriticalSection();
		}

		#region Sources

		/// <summary>
		/// Returns all of the sources available in the core.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ISource> GetCoreSources()
		{
			return m_Routing.Room
			                .Core
			                .Originators
			                .GetChildren<ISource>()
			                .OrderBy(s => s.Order);
		}

		/// <summary>
		/// Returns all of the sources available in the core.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ISource> GetSources()
		{
			return m_Routing.Room
			                .Originators
			                .GetInstancesRecursive<ISource>()
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

			// This room takes precedence
			if (m_Routing.Room.Originators.ContainsRecursive(source.Id))
				return m_Routing.Room;

			return m_Routing.Room
			                .Core
			                .Originators
			                .GetChildren<IRoom>()
			                .Where(r => r.Originators.ContainsRecursive(source.Id))
			                .OrderBy(r => r.IsCombineRoom())
			                .FirstOrDefault();
		}

		/// <summary>
		/// Gets the sources currently being presented.
		/// </summary>
		/// <param name="presentationControl"></param>
		/// <returns></returns>
		public IEnumerable<ISource> GetVtcPresentationSources(IPresentationControl presentationControl)
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
		/// Gets the device for the given source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private static IDeviceBase GetDevice(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return ServiceProvider.GetService<ICore>().Originators.GetChild<IDeviceBase>(source.Device);
		}

		public static bool CanControl(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			eControlOverride controlOverride = GetControlOverride(source);

			// If the control type is NOT default we return true.
			// This means we can show UI items for sources that don't necessarily have an IDeviceControl.
			return controlOverride != eControlOverride.Default || GetDeviceControl(source, eControlOverride.Default) != null;
		}

		public static IDeviceControl GetDeviceControl(ISource source, eControlOverride controlOverride)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IDeviceBase device = GetDevice(source);
			return device == null ? null : GetDeviceControl(device, controlOverride);
		}

		private static IDeviceControl GetDeviceControl(IDeviceBase device, eControlOverride controlOverride)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			switch (controlOverride)
			{
				case eControlOverride.Default:

					IDeviceControl dialer =
						GetDeviceControl(device, eControlOverride.Vtc) ??
						GetDeviceControl(device, eControlOverride.WebConference) ??
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

		public static eControlOverride GetControlOverride(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			ConnectProSource connectProSource = source as ConnectProSource;
			return connectProSource == null ? eControlOverride.Default : connectProSource.ControlOverride;
		}

		#endregion
	}
}
