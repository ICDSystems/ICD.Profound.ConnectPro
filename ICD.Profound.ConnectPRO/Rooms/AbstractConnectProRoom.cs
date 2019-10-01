using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Audio.Controls.Mute;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.VolumePoints;
using ICD.Connect.Calendaring;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Profound.ConnectPRO.Routing;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public abstract class AbstractConnectProRoom<TSettings> : AbstractCommercialRoom<TSettings>, IConnectProRoom
		where TSettings : IConnectProRoomSettings, new()
	{
		/// <summary>
		/// Raised when the room starts/stops a meeting.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnIsInMeetingChanged;

		/// <summary>
		/// Raised when the source that is currently the primary focus of the room (i.e. VTC) changes.
		/// </summary>
		public event EventHandler<SourceEventArgs> OnFocusSourceChanged;

		/// <summary>
		/// Automatically end the meeting after this many milliseconds without any sources being routed
		/// </summary>
		private const int MEETING_TIMEOUT = 1000 * 60 * 10;
		private readonly SafeTimer m_MeetingTimeoutTimer;

		private readonly ConnectProRouting m_Routing;

		private bool m_IsInMeeting;
		private ISource m_FocusSource;

		private List<IPresentationControl> m_SubscribedPresentationControls;

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

				HandleIsInMeetingChanged(m_IsInMeeting);

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
		public abstract string Passcode { get; set; }

		/// <summary>
		/// Gets/sets the ATC number for dialing into the room.
		/// </summary>
		public abstract string AtcNumber { get; set; }

		/// <summary>
		/// Gets the CalendarControl for the room.
		/// </summary>
		public abstract ICalendarControl CalendarControl { get; }

		/// <summary>
		/// Gets the selected OBTP booking.
		/// </summary>
		public IBooking CurrentBooking { get; set; }

		/// <summary>
		/// Gets/sets the source that is currently the primary focus of the room (i.e. VTC).
		/// </summary>
		public ISource FocusSource
		{
			get { return m_FocusSource; }
			set
			{
				if (value == m_FocusSource)
					return;

				m_FocusSource = value;

				UpdateMeetingTimeoutTimer();

				OnFocusSourceChanged.Raise(this, new SourceEventArgs(m_FocusSource));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractConnectProRoom()
		{
			m_Routing = new ConnectProRouting(this);
			m_SubscribedPresentationControls = new List<IPresentationControl>();
			m_MeetingTimeoutTimer = SafeTimer.Stopped(MeetingTimeout);

			Subscribe(m_Routing);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnIsInMeetingChanged = null;
			OnFocusSourceChanged = null;

			base.DisposeFinal(disposing);

			m_MeetingTimeoutTimer.Dispose();

			Unsubscribe(m_Routing);
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

		public void StartMeeting(IBooking booking)
		{
			StartMeeting(false);
			CurrentBooking = booking;

			IEnumerable<IConferenceDeviceControl> dialers =
				ConferenceManager == null
				? Enumerable.Empty<IConferenceDeviceControl>()
				: ConferenceManager.GetDialingProviders();

			// Build map of dialer to best number
			IDialContext dialContext;
			IConferenceDeviceControl preferredDialer = ConferencingBookingUtils.GetBestDialer(booking, dialers, out dialContext);
			if (preferredDialer == null)
				return;

			// Route device to displays and/or audio destination
			IDeviceBase dialerDevice = preferredDialer.Parent;
			ISource source = Routing.Sources.GetCoreSources().FirstOrDefault(s => s.Device == dialerDevice.Id);
			if (source == null)
				return; // if we can't route a source, don't dial into conference users won't know they're in

			FocusSource = source;

			if (preferredDialer.Supports.HasFlag(eCallType.Video))
				Routing.RouteVtc(source);
			else if (preferredDialer.Supports.HasFlag(eCallType.Audio))
				Routing.RouteAtc(source);
			else
				Routing.RouteAllDisplays(source);

			// Dial booking
			preferredDialer.Dial(dialContext);
		}

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		/// <param name="resetRouting"></param>
		public void StartMeeting(bool resetRouting)
		{
			// Change meeting state before any routing for UX
			CurrentBooking = null;
			IsInMeeting = true;

			if (resetRouting)
				Routing.RouteOsd();

			// Reset mute state
			Mute(false);
		}

		/// <summary>
		/// Ends the meeting state. If it is currently sleep time fully powers down the room.
		/// </summary>
		public void EndMeeting()
		{
			bool shutdown = WakeSchedule.IsSleepTime;
			EndMeeting(shutdown);
		}

		/// <summary>
		/// Ends the meeting state. If shutdown is true fully powers down the room.
		/// </summary>
		/// <param name="shutdown"></param>
		public void EndMeeting(bool shutdown)
		{
			EndAllConferences();

			// Change meeting state before any routing for UX
			CurrentBooking = null;
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
		public override void Wake()
		{
			// Change meeting state before any routing for UX
			CurrentBooking = null;
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
			EndAllConferences();

			// Change meeting state before any routing for UX
			CurrentBooking = null;
			IsInMeeting = false;

			// Reset all routing
			Routing.RouteOsd();

			// Power off displays
			foreach (IDestination destination in Routing.Destinations.GetDisplayDestinations())
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
		/// Called when the meeting state is changed
		/// </summary>
		/// <param name="isInMeeting"></param>
		protected virtual void HandleIsInMeetingChanged(bool isInMeeting)
		{
		}

		/// <summary>
		/// Called when the room combine state changes.
		/// </summary>
		protected override void HandleCombineState()
		{
			base.HandleCombineState();

			// End the meeting whenever the combine state changes
			EndMeeting(false);
		}

		protected override void OriginatorsOnChildrenChanged(object sender, EventArgs args)
		{
			base.OriginatorsOnChildrenChanged(sender, args);

			foreach (var presentationControl in m_SubscribedPresentationControls)
				Unsubscribe(presentationControl);
			
			m_SubscribedPresentationControls = this.GetControlsRecursive<IPresentationControl>().ToList();

			foreach (var presentationControl in m_SubscribedPresentationControls)
				Subscribe(presentationControl);
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
			List<IConference> activeConferences = 
				ConferenceManager == null 
					? new List<IConference>() 
					: ConferenceManager.OnlineConferences.ToList();

			foreach (IConference activeConference in activeConferences)
				EndConference(activeConference);
		}

		private void EndConference(IConference conference)
		{
			// TODO - Actually use polymorphism like a good developer
			var traditional = conference as ITraditionalConference;
			if (traditional != null)
				traditional.Hangup();

			var web = conference as IWebConference;
			if (web != null)
				web.LeaveConference();
		}

		/// <summary>
		/// Stops/resets the delayed sleep timer based on the current meeting state.
		/// </summary>
		private void UpdateMeetingTimeoutTimer()
		{
			if (GetIsInActiveMeeting())
				m_MeetingTimeoutTimer.Stop();
			else
				m_MeetingTimeoutTimer.Reset(MEETING_TIMEOUT);
		}

		/// <summary>
		/// Returns true if a source is actively routed to a display or we are in a conference.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsInActiveMeeting()
		{
			//If there is an active focus source return true.
			if (FocusSource != null)
				return true;

			//If there is an active conference return true.
			if (ConferenceManager != null && ConferenceManager.IsInCall != eInCall.None)
				return true;

			//Returns True if there is any real active video sources, otherwise returns false.
			return Routing.State.GetRealActiveVideoSources().Any(kvp => kvp.Value.Count > 0);
		}

		private void MeetingTimeout()
		{
			if (CombineState)
				return;

			Log(eSeverity.Informational, "Meeting timeout occurring at {0}", IcdEnvironment.GetLocalTime().ToShortTimeString());

			EndMeeting();
		}

		#endregion

		#region Routing Callbacks

		private void Subscribe(ConnectProRouting routing)
		{
			routing.State.OnSourceRoutedChanged += StateOnSourceRoutedChanged;
		}

		private void Unsubscribe(ConnectProRouting routing)
		{
			routing.State.OnSourceRoutedChanged -= StateOnSourceRoutedChanged;
		}

		private void StateOnSourceRoutedChanged(object sender, EventArgs e)
		{
			UpdateMeetingTimeoutTimer();
		}

		#endregion

		#region Conference Manager Callbacks 

		/// <summary>
		/// Subscribe to the conference manager events.
		/// </summary>
		/// <param name="conferenceManager"></param>
		protected override void Subscribe(IConferenceManager conferenceManager)
		{
			if (conferenceManager == null)
				return;

			conferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference manager events.
		/// </summary>
		/// <param name="conferenceManager"></param>
		protected override void Unsubscribe(IConferenceManager conferenceManager)
		{
			if (conferenceManager == null)
				return;

			conferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;
		}

		/// <summary>
		/// Called when the conference manager enters/leaves a call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ConferenceManagerOnInCallChanged(object sender, InCallEventArgs eventArgs)
		{
			UpdateMeetingTimeoutTimer();
		}

		#endregion

		#region Presentation Control Callbacks 

		private void Subscribe(IPresentationControl presentationControl)
		{
			presentationControl.OnPresentationActiveChanged += PresentationControlOnPresentationActiveChanged;
		}

		private void Unsubscribe(IPresentationControl presentationControl)
		{
			presentationControl.OnPresentationActiveChanged -= PresentationControlOnPresentationActiveChanged;
		}

		private void PresentationControlOnPresentationActiveChanged(object sender, PresentationActiveApiEventArgs args)
		{
			if (!this.IsCombineRoom())
				return;

			var presentationControl = sender as IPresentationControl;
			var conferenceControl = presentationControl == null ? null : presentationControl.Parent.Controls.GetControl<IConferenceDeviceControl>();
			if (conferenceControl == null || conferenceControl.GetActiveConference() == null)
				return;
			
			var conferenceSource = Originators.GetInstancesRecursive<ISource>().FirstOrDefault(s => s.Device == presentationControl.Parent.Id);
			if (conferenceSource == null)
				return;
			
			Routing.RouteVtc(conferenceSource);
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
