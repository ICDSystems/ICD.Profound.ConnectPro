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
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.VolumePoints;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices.Controls.Power;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Panels.Controls.Backlight;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Commercial.Controls.Occupancy;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Profound.ConnectPROCommon.Dialing;
using ICD.Profound.ConnectPROCommon.EventArguments;
using ICD.Profound.ConnectPROCommon.Routing;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPROCommon.Rooms
{
	public abstract class AbstractConnectProRoom<TSettings> : AbstractCommercialRoom<TSettings>, IConnectProRoom
		where TSettings : IConnectProRoomSettings, new()
	{
		/// <summary>
		/// Raised when the source that is currently the primary focus of the room (i.e. VTC) changes.
		/// </summary>
		public event EventHandler<SourceEventArgs> OnFocusSourceChanged;

		/// <summary>
		/// Raised when the upcoming booking changes.
		/// </summary>
		public event EventHandler<GenericEventArgs<IBooking>> OnUpcomingBookingChanged;

		/// <summary>
		/// Automatically end the meeting after this many milliseconds without any sources being routed
		/// </summary>
		private const int MEETING_TIMEOUT = 10 * 60 * 1000;
		private readonly SafeTimer m_MeetingTimeoutTimer;

		private readonly ConnectProRouting m_Routing;
		private readonly ConnectProDialing m_Dialing;

		private readonly IcdTimer m_MeetingStartTimer;

		private readonly List<IPresentationControl> m_SubscribedPresentationControls;
		private readonly List<IPowerDeviceControl> m_SubscribedDisplayPowerControls;

		private IBooking m_UpcomingBooking;
		private ISource m_FocusSource;

		#region Properties

		/// <summary>
		/// Gets the routing features for this room.
		/// </summary>
		public ConnectProRouting Routing { get { return m_Routing; } }

		/// <summary>
		/// Gets the dialing features for this room.
		/// </summary>
		public ConnectProDialing Dialing { get { return m_Dialing; } }

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		public abstract string Passcode { get; set; }

		/// <summary>
		/// Gets the selected OBTP booking.
		/// </summary>
		public IBooking CurrentBooking { get; private set; }

		/// <summary>
		/// Gets the upcoming booking.
		/// </summary>
		public IBooking UpcomingBooking
		{
			get { return m_UpcomingBooking; }
			private set
			{
				if (value == null && m_UpcomingBooking == null)
					return;

				if (value != null && value.Duplicates(m_UpcomingBooking))
					return;

				m_UpcomingBooking = value;

				OnUpcomingBookingChanged.Raise(this, new GenericEventArgs<IBooking>(m_UpcomingBooking));
			}
		}

		///<summary>
		/// Gets the timer for grace period before automatically starting a meeting.
		/// </summary>
		public IcdTimer MeetingStartTimer { get { return m_MeetingStartTimer; } }

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

				RestartMeetingTimeoutTimer();

				OnFocusSourceChanged.Raise(this, new SourceEventArgs(m_FocusSource));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractConnectProRoom()
		{
			m_SubscribedPresentationControls = new List<IPresentationControl>();
			m_SubscribedDisplayPowerControls = new List<IPowerDeviceControl>();

			m_Routing = new ConnectProRouting(this);
			m_Dialing = new ConnectProDialing(this);

			m_MeetingStartTimer = new IcdTimer();
			m_MeetingTimeoutTimer = SafeTimer.Stopped(MeetingTimeout);

			Subscribe(m_Routing);
			Subscribe(m_MeetingStartTimer);

			// Initialize activities
			IsInMeeting = false;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnFocusSourceChanged = null;
			OnUpcomingBookingChanged = null;

			base.DisposeFinal(disposing);

			Unsubscribe(m_MeetingStartTimer);

			m_MeetingStartTimer.Dispose();
			m_MeetingTimeoutTimer.Dispose();

			Unsubscribe(m_Routing);
			m_Routing.Dispose();
		}

		#region Methods

		/// <summary>
		/// Starts meeting for the current booking and configured default source.
		/// </summary>
		public void StartAutoMeeting()
		{
			IBooking booking = CalendarManager == null ? null : CalendarManager.CurrentBooking;
			ISource source = TouchFree == null ? null : TouchFree.Source;

			StartMeeting(booking, source);
		}

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		/// <param name="booking"></param>
		/// <param name="source"></param>
		public void StartMeeting(IBooking booking, ISource source)
		{
			// Stop the meeting start timer
			m_MeetingStartTimer.Stop();

			// Change meeting state before any routing for UX
			if (booking != null)
				CheckIn(booking);
			IsInMeeting = true;

			// Reset mute state
			Mute(false);

			// Dial the booking
			// Don't do any additional routing if we dialed something
			if (booking != null && Dialing.DialBooking(booking))
				return;

			// If a source was specified OR there is only one source automatically route it
			ISource[] sources = Routing.Sources.GetRoomSources().ToArray();
			ISource automaticSource = source ?? (sources.Length == 1 ? sources[0] : null);

			if (automaticSource == null)
				Routing.RouteOsd();
			else
				if (!Routing.RouteSourceByControl(automaticSource))
					Routing.RouteToAllDisplays(automaticSource);
		}

		/// <summary>
		/// Ends the meeting state. If it is currently sleep time fully powers down the room.
		/// </summary>
		public void EndMeeting()
		{
			bool shutdown = WakeSchedule != null && WakeSchedule.IsSleepTime;
			EndMeeting(shutdown);
		}

		/// <summary>
		/// Ends the meeting state. If shutdown is true fully powers down the room.
		/// </summary>
		/// <param name="shutdown"></param>
		public void EndMeeting(bool shutdown)
		{
			Dialing.EndAllConferences();

			// Change meeting state before any routing for UX
			CheckOut();
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
			CheckOut();
			IsInMeeting = false;

			// Reset all routing
			Routing.RouteOsd();

			// Turn on the panel backlights
			Originators.GetInstancesRecursive<IPanelDevice>()
			           .SelectMany(p => p.Controls.GetControls<IBacklightDeviceControl>())
			           .ForEach(c => c.BacklightOn());
		}

		/// <summary>
		/// Shuts down the room.
		/// </summary>
		public override void Sleep()
		{
			Dialing.EndAllConferences();

			// Change meeting state before any routing for UX
			CheckOut();
			IsInMeeting = false;

			// Reset all routing
			Routing.RouteOsd(false);

			// Power off displays
			Routing.Destinations
			       .GetVideoDestinations()
			       .SelectMany(d => d.GetDevices())
			       .Distinct()
			       .ForEach(d => Routing.PowerDevice(d, false));

			// Turn off the panel backlights
			Originators.GetInstancesRecursive<IPanelDevice>()
					   .SelectMany(p => p.Controls.GetControls<IBacklightDeviceControl>())
					   .ForEach(c => c.BacklightOff());
		}

		/// <summary>
		/// Sets the active camera for the room.
		/// If both the selected camera and routing control are not null, attempts to route the camera.
		/// </summary>
		/// <param name="activeCamera"></param>
		/// <param name="vtcDestinationControl"></param>
		public void SetActiveCamera(ICameraDevice activeCamera,
		                            IVideoConferenceRouteControl vtcDestinationControl)
		{
			if (ConferenceManager != null)
				ConferenceManager.Cameras.SetActiveCamera(activeCamera);

			if (activeCamera != null && vtcDestinationControl != null)
				Routing.RouteCameraToVtc(activeCamera, vtcDestinationControl);
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Called when the meeting state is changed
		/// </summary>
		/// <param name="isInMeeting"></param>
		protected override void HandleIsInMeetingChanged(bool isInMeeting)
		{
			base.HandleIsInMeetingChanged(isInMeeting);

			// Clear the focus source
			FocusSource = null;

			RestartMeetingTimeoutTimer();
			UpdateUpcomingBooking();
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

		/// <summary>
		/// Override to handle the room becoming occupied or vacated.
		/// </summary>
		/// <param name="occupancyState"></param>
		protected override void HandleOccupiedChanged(eOccupancyState occupancyState)
		{
			base.HandleOccupiedChanged(occupancyState);

			// Don't handle occupancy changes if combined
			if (CombineState)
				return;

			// Don't handle occupancy changes if TouchFree is disabled
			if (TouchFree == null || !TouchFree.Enabled)
				return;

			switch (occupancyState)
			{
				case eOccupancyState.Unknown:
					break;

				case eOccupancyState.Unoccupied:
					// Room became unoccupied - Restart the meeting timeout and stop counting down to start meeting
					RestartMeetingTimeoutTimer();
					m_MeetingStartTimer.Stop();
					break;

				case eOccupancyState.Occupied:
					// Room became occupied - Wake the room and start counting down to start meeting
					if (!IsInMeeting)
					{
						m_MeetingStartTimer.Restart(TouchFree.CountdownSeconds * 1000);
						if (!IsAwake)
							Wake();
					}
					break;

				default:
					throw new ArgumentOutOfRangeException("occupancyState");
			}
		}

		/// <summary>
		/// Called before this combine space is destroyed as part of an uncombine operation.
		/// </summary>
		public override void HandlePreUncombine()
		{
			base.HandlePreUncombine();

			// End the meeting before the room is torn down.
			EndMeeting(false);
		}

		/// <summary>
		/// Called when the contents of the room change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void OriginatorsOnCollectionChanged(object sender, EventArgs args)
		{
			base.OriginatorsOnCollectionChanged(sender, args);

			IEnumerable<IPresentationControl> presentationControls =
				Routing.Sources
				       .GetRoomSources()
				       .SelectMany(s => s.GetDevices())
				       .SelectMany(s => s.Controls.GetControls<IPresentationControl>());
			SetPresentationControls(presentationControls);

			IEnumerable<IPowerDeviceControl> displayPowerControls =
				Routing.Destinations
				       .GetVideoDestinations()
				       .SelectMany(d => d.GetDevices())
				       .SelectMany(d => d.Controls.GetControls<IPowerDeviceControl>());
			SetDisplayPowerControls(displayPowerControls);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Checks in to the given booking.
		/// </summary>
		/// <param name="booking"></param>
		private void CheckIn([NotNull] IBooking booking)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			CheckOut();

			if (CalendarManager != null)
				CalendarManager.CheckIn(booking);

			CurrentBooking = booking;
		}

		/// <summary>
		/// Checks out of the given booking.
		/// </summary>
		private void CheckOut()
		{
			if (CurrentBooking == null)
				return;

			if (CalendarManager != null)
				CalendarManager.CheckOut(CurrentBooking);

			CurrentBooking = null;
		}

		/// <summary>
		/// Sets the mute state on the room volume point.
		/// </summary>
		/// <param name="mute"></param>
		private void Mute(bool mute)
		{
			foreach (IVolumePoint volumePoint in Originators.GetInstancesRecursive<IVolumePoint>())
			{
				IVolumeDeviceControl volumeControl = volumePoint.Control;
				if (volumeControl != null && volumeControl.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.MuteAssignment))
					volumeControl.SetIsMuted(mute);
			}
		}

		/// <summary>
		/// Resets the Meeting Timeout timer.
		/// </summary>
		private void RestartMeetingTimeoutTimer()
		{
			m_MeetingTimeoutTimer.Reset(MEETING_TIMEOUT);
		}

		///<summary>
		/// Sets the upcoming booking to the first upcoming booking after the current booking.
		/// </summary>
		private void UpdateUpcomingBooking()
		{
			UpcomingBooking =
				CalendarManager == null
					? null
					: CalendarManager.GetBookings()
					                 .Where(b => !b.Duplicates(CurrentBooking) && b.StartTime >= IcdEnvironment.GetUtcTime())
					                 .OrderBy(b => b.StartTime)
					                 .FirstOrDefault();
		}

		/// <summary>
		/// Clears privacy mute if we are between calls.
		/// </summary>
		private void UpdatePrivacyMute()
		{
			if (ConferenceManager == null)
				return;

			bool inCall = Dialing.ConferenceActionsAvailable(eInCall.Audio);

			if (!inCall)
				ConferenceManager.PrivacyMuted = false;
		}

		/// <summary>
		/// Returns the cameras in the room to the home position.
		/// </summary>
		private void ReturnCamerasToHome()
		{
			this.GetControlsRecursive<ICameraDeviceControl>()
				.Where(c => c.SupportedCameraFeatures.HasFlag(eCameraFeatures.Home))
				.ForEach(c => c.ActivateHome());
		}

		/// <summary>
		/// Sets the privacy mute state for the cameras.
		/// </summary>
		/// <param name="privacyMute"></param>
		private void PrivacyMuteCameras(bool privacyMute)
		{
			if (ConferenceManager != null)
				ConferenceManager.CameraPrivacyMuted = privacyMute;
		}

		/// <summary>
		/// Returns false if any of the following is true:
		///		The room is not currently in a meeting
		///		TouchFree is enabled and the room has been unoccupied for the meeting timeout duration
		/// Otherwise, returns true if any of the following is true:
		///		A source is actively routed to a display
		///		A video/audio call is currently active
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsInActiveMeeting()
		{
			// Easy case!
			if (!IsInMeeting)
				return false;

			// The room is unoccupied
			// Subtracting a second from timeout time just in case unoccupancy triggered the timeout timer
			// and we start running into timer precision issues.
			if (OccupancyManager != null && OccupancyManager.OccupancyState == eOccupancyState.Unoccupied &&
			    (IcdEnvironment.GetUtcTime() - OccupancyManager.OccupancyChangeTime).TotalMilliseconds >= MEETING_TIMEOUT - 1000)
				return false;

			// If the user is currently on a control subpage assume that the room is being used
			if (FocusSource != null)
				return true;

			// If there is an active conference the room is being used.
			if (ConferenceManager != null && ConferenceManager.Dialers.IsInCall != eInCall.None)
				return true;

			// If at least one video destination has a routed source the room is being used.
			return Routing.State.GetRealActiveVideoSources().Any(kvp => kvp.Value.Count > 0);
		}

		/// <summary>
		/// Called when the current meeting times out after a duration without activity.
		/// </summary>
		private void MeetingTimeout()
		{
			if (CombineState)
				return;

			if (GetIsInActiveMeeting())
				return;

			if (!IsAwake)
				return;

			Logger.Log(eSeverity.Informational, "Meeting timeout occurring");

			EndMeeting();
		}

		/// <summary>
		/// Updates the rooms IsAwake state.
		/// </summary>
		private void UpdateIsAwake()
		{
			IsAwake = m_SubscribedDisplayPowerControls.Any(p => p.PowerState == ePowerState.PowerOn);
		}

		/// <summary>
		/// Update privacy mute and cameras when a web conferencing source is routed/unrouted.
		/// </summary>
		/// <param name="routed"></param>
		/// <param name="unrouted"></param>
		private void UpdateConferenceFeatures([NotNull] IEnumerable<ISource> routed,
		                                      [NotNull] IEnumerable<ISource> unrouted)
		{
			// If we're in a video call don't mess with cameras and microphones
			if (ConferenceManager != null && ConferenceManager.Dialers.IsInCall.HasFlag(eInCall.Video))
				return;

			// Did a new web conference source become routed?
			ConnectProSource routedWebSource =
				routed.OfType<ConnectProSource>()
				      .FirstOrDefault(s => s.ConferenceOverride == eConferenceOverride.Show);

			// Are there any other web conference sources currently routed?
			bool otherWebSources =
				Routing.State
				       .GetRealActiveVideoSources()
				       .SelectMany(kvp => kvp.Value)
					   .Except(routedWebSource)
				       .Any();

			// Still routing a web source, so don't mess with cameras and microphones
			if (otherWebSources)
				return;
			
			UpdatePrivacyMute();

			// Return cameras to home position when entering a web conference
			if (routedWebSource != null)
			{
				ReturnCamerasToHome();
				PrivacyMuteCameras(false);
			}
			else
			{
				PrivacyMuteCameras(true);
			}
		}

		/// <summary>
		/// Update privacy mute and cameras when the conference manager changes call state.
		/// </summary>
		/// <param name="callState"></param>
		private void UpdateConferenceFeatures(eInCall callState)
		{
			UpdatePrivacyMute();

			// Return cameras to home position when entering a video call
			if (callState.HasFlag(eInCall.Video))
			{
				ReturnCamerasToHome();
				PrivacyMuteCameras(false);
			}
			else
			{
				PrivacyMuteCameras(true);
			}
		}

		#endregion

		#region Meeting Start Timer Callbacks

		private void Subscribe(IcdTimer meetingStartTimer)
		{
			meetingStartTimer.OnElapsed += MeetingStartTimerOnElapsed;
		}

		private void Unsubscribe(IcdTimer meetingStartTimer)
		{
			meetingStartTimer.OnElapsed -= MeetingStartTimerOnElapsed;
		}

		private void MeetingStartTimerOnElapsed(object sender, EventArgs eventArgs)
		{
			StartAutoMeeting();
		}

		#endregion

		#region Routing Callbacks

		/// <summary>
		/// Subscribe to the routing events.
		/// </summary>
		/// <param name="routing"></param>
		private void Subscribe(ConnectProRouting routing)
		{
			routing.State.OnSourceRoutedChanged += StateOnSourceRoutedChanged;
		}

		/// <summary>
		/// Unsubscribe from the routing events.
		/// </summary>
		/// <param name="routing"></param>
		private void Unsubscribe(ConnectProRouting routing)
		{
			routing.State.OnSourceRoutedChanged -= StateOnSourceRoutedChanged;
		}

		/// <summary>
		/// Called when a source becomes routed/unrouted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StateOnSourceRoutedChanged(object sender, SourceRoutedEventArgs eventArgs)
		{
			RestartMeetingTimeoutTimer();

			UpdateConferenceFeatures(eventArgs.Routed, eventArgs.Unrouted);
		}

		#endregion

		#region TouchFree Callbacks

		/// <summary>
		/// Called when the TouchFree becomes enabled/disabled.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		protected override void TouchFreeOnEnabledChanged(object sender, BoolEventArgs boolEventArgs)
		{
			base.TouchFreeOnEnabledChanged(sender, boolEventArgs);

			// Stop the meeting timer because TouchFree was enabled/disabled
			m_MeetingStartTimer.Stop();
		}

		#endregion

		#region Conference Manager Callbacks 

		/// <summary>
		/// Subscribe to the conference manager events.
		/// </summary>
		/// <param name="conferenceManager"></param>
		protected override void Subscribe(IConferenceManager conferenceManager)
		{
			base.Subscribe(conferenceManager);

			if (conferenceManager == null)
				return;

			conferenceManager.Dialers.OnInCallChanged += ConferenceManagerOnInCallChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference manager events.
		/// </summary>
		/// <param name="conferenceManager"></param>
		protected override void Unsubscribe(IConferenceManager conferenceManager)
		{
			base.Unsubscribe(conferenceManager);

			if (conferenceManager == null)
				return;

			conferenceManager.Dialers.OnInCallChanged -= ConferenceManagerOnInCallChanged;
		}

		/// <summary>
		/// Called when the conference manager enters/leaves a call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ConferenceManagerOnInCallChanged(object sender, InCallEventArgs eventArgs)
		{
			RestartMeetingTimeoutTimer();

			UpdateConferenceFeatures(eventArgs.Data);
		}

		#endregion

		#region Calendar Manager Callbacks

		/// <summary>
		/// Called when the calendar bookings change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void CalendarManagerOnBookingsChanged(object sender, EventArgs eventArgs)
		{
			base.CalendarManagerOnBookingsChanged(sender, eventArgs);

			UpdateUpcomingBooking();
		}

		#endregion

		#region Presentation Control Callbacks 

		private void SetPresentationControls([NotNull] IEnumerable<IPresentationControl> presentationControls)
		{
			if (presentationControls == null)
				throw new ArgumentNullException("presentationControls");

			foreach (var presentationControl in m_SubscribedPresentationControls)
				Unsubscribe(presentationControl);

			m_SubscribedPresentationControls.Clear();
			m_SubscribedPresentationControls.AddRange(presentationControls);

			foreach (var presentationControl in m_SubscribedPresentationControls)
				Subscribe(presentationControl);
		}

		/// <summary>
		/// Subscribe to the presentation control events.
		/// </summary>
		/// <param name="presentationControl"></param>
		private void Subscribe(IPresentationControl presentationControl)
		{
			presentationControl.OnPresentationActiveChanged += PresentationControlOnPresentationActiveChanged;
		}

		/// <summary>
		/// Unsubscribe from the presentation control events.
		/// </summary>
		/// <param name="presentationControl"></param>
		private void Unsubscribe(IPresentationControl presentationControl)
		{
			presentationControl.OnPresentationActiveChanged -= PresentationControlOnPresentationActiveChanged;
		}

		/// <summary>
		/// Called when a presentation becomes active/inactive.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PresentationControlOnPresentationActiveChanged(object sender, PresentationActiveApiEventArgs args)
		{
			if (!this.IsCombineRoom())
				return;

			var presentationControl = sender as IPresentationControl;
			var conferenceControl = presentationControl == null ? null : presentationControl.Parent.Controls.GetControl<IConferenceDeviceControl>();
			if (conferenceControl == null || !conferenceControl.GetActiveConferences().Any())
				return;
			
			var conferenceSource = Originators.GetInstancesRecursive<ISource>().FirstOrDefault(s => s.Device == presentationControl.Parent.Id);
			if (conferenceSource == null)
				return;
			
			Routing.RouteVtc(conferenceSource);
		}

		#endregion

		#region Display PowerControl Callbacks

		private void SetDisplayPowerControls(IEnumerable<IPowerDeviceControl> displayPowerControls)
		{
			if (displayPowerControls == null)
				throw new ArgumentNullException("displayPowerControls");

			foreach (var displayPowerControl in m_SubscribedDisplayPowerControls)
				Unsubscribe(displayPowerControl);

			m_SubscribedDisplayPowerControls.Clear();
			m_SubscribedDisplayPowerControls.AddRange(displayPowerControls);

			foreach (var displayPowerControl in m_SubscribedDisplayPowerControls)
				Subscribe(displayPowerControl);

			UpdateIsAwake();
		}

		/// <summary>
		/// Subscribe to the display power control events.
		/// </summary>
		/// <param name="displayPowerControl"></param>
		private void Subscribe(IPowerDeviceControl displayPowerControl)
		{
			displayPowerControl.OnPowerStateChanged += DisplayPowerControlOnPowerStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the display power control events.
		/// </summary>
		/// <param name="displayPowerControl"></param>
		private void Unsubscribe(IPowerDeviceControl displayPowerControl)
		{
			displayPowerControl.OnPowerStateChanged -= DisplayPowerControlOnPowerStateChanged;
		}

		/// <summary>
		/// Called when a display power state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DisplayPowerControlOnPowerStateChanged(object sender, PowerDeviceControlPowerStateApiEventArgs e)
		{
			UpdateIsAwake();
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
