using System;
using System.Linq;
using ICD.Common.Utils.Services;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Routing
{
	public sealed class ConnectProRoutingSources
	{
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
