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
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Profound.ConnectPRO.Routing;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public abstract class AbstractConnectProRoom<TSettings> : AbstractRoom<TSettings>, IConnectProRoom
		where TSettings : IConnectProRoomSettings, new()
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
		/// Gets the conference manager.
		/// </summary>
		public abstract IConferenceManager ConferenceManager { get; }

		/// <summary>
		/// Gets the wake/sleep schedule.
		/// </summary>
		public abstract WakeSchedule WakeSchedule { get; }

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		public abstract string Passcode { get; set; }

		/// <summary>
		/// Gets/sets the ATC number for dialing into the room.
		/// </summary>
		public abstract string AtcNumber { get; set; }

		/// <summary>
		/// Gets the CalendarControl for the room.
		/// </summary>
		public abstract ICalendarControl CalendarControl { get; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractConnectProRoom()
		{
			m_Routing = new ConnectProRouting(this);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnIsInMeetingChanged = null;

			base.DisposeFinal(disposing);
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
			EndAllConferences();

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
		/// Wakes up the room.
		/// </summary>
		public void Wake()
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
		public void Sleep()
		{
			EndAllConferences();

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

		#endregion

		#region Private Methods

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

		private void EndAllConferences()
		{
			var activeConferences = ConferenceManager == null ? Enumerable.Empty<IConference>() : ConferenceManager.OnlineConferences;
			// Hangup
			foreach (var activeConference in activeConferences)
			{
				// TODO - Actually use polymorphism like a good developer
				var traditional = activeConference as ITraditionalConference;
				if (traditional != null)
					traditional.Hangup();

				var web = activeConference as IWebConference;
				if (web != null)
					web.LeaveConference();
			}
		}

		#endregion

		#region WakeSchedule Callbacks

		protected void Subscribe(WakeSchedule schedule)
		{
			schedule.OnWakeActionRequested += ScheduleOnWakeActionRequested;
			schedule.OnSleepActionRequested += ScheduleOnSleepActionRequested;
		}

		protected void Unsubscribe(WakeSchedule schedule)
		{
			schedule.OnWakeActionRequested -= ScheduleOnWakeActionRequested;
			schedule.OnSleepActionRequested -= ScheduleOnSleepActionRequested;
		}

		private void ScheduleOnSleepActionRequested(object sender, EventArgs eventArgs)
		{
			if (CombineState)
				return;

			Log(eSeverity.Informational, "Scheduled sleep occurring at {0}", IcdEnvironment.GetLocalTime().ToShortTimeString());
			Sleep();
		}

		private void ScheduleOnWakeActionRequested(object sender, EventArgs eventArgs)
		{
			if (CombineState)
				return;

			Log(eSeverity.Informational, "Scheduled wake occurring at {0}", IcdEnvironment.GetLocalTime().ToShortTimeString());
			Wake();
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

			ConnectProRoomConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in ConnectProRoomConsole.GetConsoleCommands(this))
				yield return command;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			foreach (IConsoleNodeBase node in ConnectProRoomConsole.GetConsoleNodes(this))
				yield return node;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		#endregion
	}
}
