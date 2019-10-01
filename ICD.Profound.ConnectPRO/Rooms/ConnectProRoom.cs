using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Audio.Controls.Mute;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.VolumePoints;
using ICD.Connect.Calendaring.CalendarControl;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Favorites.SqLite;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Settings.Core;
using ICD.Profound.ConnectPRO.Routing;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public sealed class ConnectProRoom : AbstractCommercialRoom<ConnectProRoomSettings>, IConnectProRoom
	{
		/// <summary>
		/// Raised when the room starts/stops a meeting.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnIsInMeetingChanged;

		private readonly ConnectProRouting m_Routing;

		private bool m_IsInMeeting;

		#region Properties

		/// <summary>
		/// Gets/sets the current meeting status.
		/// </summary>
		public bool IsInMeeting
		{
			get { return m_IsInMeeting; }
			private set
			{
				if (value == m_IsInMeeting)
					return;

				m_IsInMeeting = value;

				Log(eSeverity.Informational, "IsInMeeting changed to {0}", m_IsInMeeting);

				OnIsInMeetingChanged.Raise(this, new BoolEventArgs(m_IsInMeeting));
			}
		}

		/// <summary>
		/// Gets the routing features for this room.
		/// </summary>
		public ConnectProRouting Routing { get { return m_Routing; } }

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		public string Passcode { get; set; }

		/// <summary>
		/// Gets/sets the ATC number for dialing into the room.
		/// </summary>
		public string AtcNumber { get; set; }

		/// <summary>
		/// Gets/sets the calendar control for the room.
		/// </summary>
		public ICalendarControl CalendarControl { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProRoom()
		{
			m_Routing = new ConnectProRouting(this);
			ConferenceManager = new ConferenceManager();
		}

		/// <summary>
		/// Release resources
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			OnIsInMeetingChanged = null;

			base.DisposeFinal(disposing);

			m_Routing.Dispose();
		}

		#region Methods

		/// <summary>
		/// Gets the volume control matching the configured volume point.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public IVolumeDeviceControl GetVolumeControl()
		{
			IVolumePoint volumePoint = Originators.GetInstance<IVolumePoint>();
			if (volumePoint == null)
				return null;

			try
			{
				return Core.GetControl<IVolumeDeviceControl>(volumePoint.DeviceId, volumePoint.ControlId);
			}
			catch (Exception)
			{
				Log(eSeverity.Error, "Failed to find volume control for {0}", volumePoint);
			}

			return null;
		}

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		public void StartMeeting()
		{
			StartMeeting(true);
		}

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		/// <param name="resetRouting"></param>
		public void StartMeeting(bool resetRouting)
		{
			// Change meeting state before any routing for UX
			IsInMeeting = true;

			if (resetRouting)
				Routing.RouteOsd();

			// Reset mute state
			Mute(false);
		}

		/// <summary>
		/// Ends the meeting state. If shutdown is true fully powers down the room.
		/// </summary>
		/// <param name="shutdown"></param>
		public void EndMeeting(bool shutdown)
		{
			// Hangup
			if (ConferenceManager != null && ConferenceManager.ActiveConference != null)
				ConferenceManager.ActiveConference.Hangup();

			// Change meeting state before any routing for UX
			IsInMeeting = false;

			// Reset all routing
			Routing.RouteOsd();

			// Reset mute state
			Mute(false);

			if (shutdown)
				Sleep();
		}

		/// <summary>
		/// Returns true if a source is actively routed to a display or we are in a conference.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsInActiveMeeting()
		{
			// TODO
			return false;
		}

		/// <summary>
		/// Wakes up the room.
		/// </summary>
		public override void Wake()
		{
			// Change meeting state before any routing for UX
			IsInMeeting = false;

			// Reset all routing
			Routing.RouteOsd();

			// Power the panels
			Originators.GetInstancesRecursive<IPanelDevice>()
					   .SelectMany(panel => panel.Controls.GetControls<IPowerDeviceControl>())
					   .ForEach(c => c.PowerOn());
		}

		/// <summary>
		/// Shuts down the room.
		/// </summary>
		public override void Sleep()
		{
			// Hangup
			if (ConferenceManager != null && ConferenceManager.ActiveConference != null)
				ConferenceManager.ActiveConference.Hangup();

			// Change meeting state before any routing for UX
			IsInMeeting = false;

			// Power off displays
			foreach (IDestination destination in Routing.GetDisplayDestinations())
			{
				IDisplay display = Core.Originators.GetChild(destination.Device) as IDisplay;
				if (display != null)
					display.PowerOff();
			}

			// Power off the panels
			Originators.GetInstancesRecursive<IPanelDevice>()
			           .SelectMany(panel => panel.Controls.GetControls<IPowerDeviceControl>())
			           .ForEach(c => c.PowerOff());
		}

		/// <summary>
		/// Sets the mute state on the room volume point.
		/// </summary>
		/// <param name="mute"></param>
		private void Mute(bool mute)
		{
			IVolumeMuteDeviceControl muteControl = GetVolumeControl() as IVolumeMuteDeviceControl;
			if (muteControl != null)
				muteControl.SetVolumeMute(mute);
		}

		#endregion

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
			CalendarControl = null;
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
			string path = PathUtils.GetProgramConfigPath("favorites");
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
					CalendarControl = calendarDevice.Controls.GetControl<ICalendarControl>();
			}
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Is In Meeting", IsInMeeting);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("Start Meeting", "Starts the meeting", () => StartMeeting());
			yield return new ConsoleCommand("End Meeting", "Ends the meeting", () => EndMeeting(false));
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
