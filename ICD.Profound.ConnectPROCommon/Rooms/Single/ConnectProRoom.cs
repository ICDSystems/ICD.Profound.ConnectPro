using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Calendaring.CalendarManagers;
using ICD.Connect.Calendaring.CalendarPoints;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.ConferencePoints;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Devices;
using ICD.Connect.Partitioning;
using ICD.Connect.Partitioning.Commercial;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Utils;

namespace ICD.Profound.ConnectPROCommon.Rooms.Single
{
	public sealed class ConnectProRoom : AbstractConnectProRoom<ConnectProRoomSettings>
	{
		#region Properties

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		public override string Passcode { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProRoom()
		{
			ConferenceManager = new ConferenceManager();
			CalendarManager = new CalendarManager();
			WakeSchedule = new WakeSchedule();
			TouchFree = new TouchFree();
		}

		/// <summary>
		/// Release resources
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			ConferenceManager = null;
			CalendarManager = null;
			WakeSchedule = null;
			TouchFree = null;
		}

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(ConnectProRoomSettings settings)
		{
			base.CopySettingsFinal(settings);

			ICalendarControl calendarControl = CalendarManager == null ? null : CalendarManager.GetProviders().FirstOrDefault();

			settings.Passcode = Passcode;
			settings.CalendarDevice = calendarControl == null ? (int?)null : calendarControl.Parent.Id;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Passcode = null;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(ConnectProRoomSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			// Passcode
			Passcode = settings.Passcode;

			// Generate calendar points
			GenerateCalendarPoints(settings, factory);

			// Generate conference points
			GenerateConferencePoints(factory);
		}

		/// <summary>
		/// Generates a calendar point for the configured calendar control.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		private void GenerateCalendarPoints(ConnectProRoomSettings settings, IDeviceFactory factory)
		{
			if (settings.CalendarDevice == null)
				return;

			IDevice calendarDevice = factory.GetOriginatorById<IDevice>(settings.CalendarDevice.Value);
			ICalendarControl calendarControl = calendarDevice.Controls.GetControl<ICalendarControl>();
			if (calendarControl == null)
				return;

			// Is there already a calendar point for the calendar control?
			if (Originators.GetInstances<ICalendarPoint>()
			               .Any(p => p.DeviceId == calendarDevice.Id && p.ControlId == calendarControl.Id))
				return;

			int id = IdUtils.GetNewId(Core.Originators.GetChildrenIds().Concat(factory.GetOriginatorIds()), eSubsystem.CalendarPoints);
			eCombineMode combineMode = Originators.GetCombineMode(calendarDevice.Id);

			CalendarPoint point = new CalendarPoint
			{
				Id = id,
				Uuid = OriginatorUtils.GenerateUuid(Core, id),
				Name = calendarControl.Name,
				Features = EnumUtils.GetFlagsAllValue<eCalendarFeatures>()
			};
			point.SetControl(calendarControl);

			Core.Originators.AddChild(point);
			Originators.Add(id, combineMode);

			if (CalendarManager != null)
				CalendarManager.RegisterCalendarProvider(point);
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
				IDevice device = Core.Originators.GetChild<IDevice>(source.Device);
				
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

					int id = IdUtils.GetNewId(Core.Originators.GetChildrenIds().Concat(factory.GetOriginatorIds()), eSubsystem.ConferencePoints);
					eCombineMode combineMode = Originators.GetCombineMode(source.Id);

					ConferencePoint point = new ConferencePoint
					{
						Id = id,
						Uuid = OriginatorUtils.GenerateUuid(Core, id),
						Name = control.Name,
						Type = control.Supports
					};
					point.SetControl(control);

					Core.Originators.AddChild(point);
					Originators.Add(id, combineMode);

					if (ConferenceManager != null)
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
