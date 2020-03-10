using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.ConferencePoints;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Favorites.SqLite;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Devices;
using ICD.Connect.Partitioning;
using ICD.Connect.Partitioning.Commercial;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Utils;

namespace ICD.Profound.ConnectPRO.Rooms.Single
{
	public sealed class ConnectProRoom : AbstractConnectProRoom<ConnectProRoomSettings>
	{
		private ICalendarControl m_CalendarControl;

		#region Properties

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		public override string Passcode { get; set; }

		/// <summary>
		/// Gets/sets the ATC number for dialing into the room.
		/// </summary>
		public override string AtcNumber { get; set; }

		/// <summary>
		/// Gets/sets the calendar control for the room.
		/// </summary>
		public override ICalendarControl CalendarControl { get { return m_CalendarControl; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProRoom()
		{
			ConferenceManager = new ConferenceManager();
			WakeSchedule = new WakeSchedule();
		}

		/// <summary>
		/// Release resources
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			ConferenceManager = null;
			WakeSchedule = null;
		}

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(ConnectProRoomSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Passcode = Passcode;

			if (CalendarControl != null && CalendarControl.Parent != null)
				settings.CalendarDevice = CalendarControl.Parent.Id;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			AtcNumber = null;
			Passcode = null;
			m_CalendarControl = null;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(ConnectProRoomSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			// Favorites
			string path = PathUtils.GetRoomDataPath(Id, "favorites");
			if (ConferenceManager != null)
				ConferenceManager.Favorites = new SqLiteFavorites(path);

			// ATC Number
			AtcNumber = settings.AtcNumber;

			// Passcode
			Passcode = settings.Passcode;

			// Calendar Device
			if (settings.CalendarDevice != null)
			{
				var calendarDevice = factory.GetOriginatorById<IDevice>(settings.CalendarDevice.Value);
				if (calendarDevice != null)
					m_CalendarControl = calendarDevice.Controls.GetControl<ICalendarControl>();
			}

			// Generate conference points
			GenerateConferencePoints(factory);
		}

		/// <summary>
		/// We need to automatically generate conference points for sources with conferencing controls
		/// that do not already have conference points.
		/// </summary>
		/// <param name="factory"></param>
		private void GenerateConferencePoints(IDeviceFactory factory)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			foreach (ISource source in Originators.GetInstances<ISource>())
			{
				// Does the source have a conference control?
				IDeviceBase device = Core.Originators.GetChild<IDeviceBase>(source.Device);
				
				// Skip devices with multiple conference controls that already have conference points.
				// DSPs should be configured properly in DeployAV
				bool alreadyConfigured =
					Originators.GetInstances<IConferencePoint>().Any(p => p.Control != null && p.Control.Parent == device);

				// DeployAV doesn't support controls on non-DSP devices, so there's no way
				// for users to make a conference point for the Zoom call out feature
				bool isZoom = device is ZoomRoom;
				if (alreadyConfigured && !isZoom)
					continue;

				IEnumerable<IConferenceDeviceControl>
					controls = device.Controls.GetControls<IConferenceDeviceControl>();
				
				foreach (IConferenceDeviceControl control in controls)
				{
					// Does the control already have a conference point?
					if (ControlHasConferencePoint(control))
						continue;

					int id = IdUtils.GetNewId(Core.Originators.GetChildrenIds().Concat(factory.GetOriginatorIds()), eSubsystems.ConferencePoints);
					eCombineMode combineMode = Originators.GetCombineMode(source.Id);

					ConferencePoint point = new ConferencePoint
					{
						Id = id,
						Name = control.Name,
						Type = control.Supports
					};
					point.SetControl(control);

					Core.Originators.AddChild(point);
					Originators.Add(id, combineMode);

					ConferenceManager.Dialers.RegisterDialingProvider(point);
				}
			}
		}

		private bool ControlHasConferencePoint(IConferenceDeviceControl control)
		{
			if (control == null)
				throw new ArgumentNullException("control");

			return Originators.GetInstances<IConferencePoint>()
			                  .Any(p => p.Control == control);
		}

		#endregion
	}
}
