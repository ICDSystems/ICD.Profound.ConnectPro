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
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Profound.ConnectPRO.Dialing;
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
		private const int MEETING_TIMEOUT = 10 * 60 * 1000;
		private readonly SafeTimer m_MeetingTimeoutTimer;

		private readonly ConnectProRouting m_Routing;
		private readonly ConnectProDialing m_Dialing;

		private bool m_IsInMeeting;
		private ISource m_FocusSource;

		private readonly List<IPresentationControl> m_SubscribedPresentationControls;
		private readonly List<IPowerDeviceControl> m_SubscribedDisplayPowerControls;

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
		/// Gets the dialing features for this room.
		/// </summary>
		public ConnectProDialing Dialing { get { return m_Dialing; } }

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		public abstract string Passcode { get; set; }

		/// <summary>
		/// Gets the CalendarControl for the room.
		/// </summary>
		public abstract ICalendarControl CalendarControl { get; }

		/// <summary>
		/// Gets the selected OBTP booking.
		/// </summary>
		public IBooking CurrentBooking { get; private set; }

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
			m_SubscribedPresentationControls = new List<IPresentationControl>();
			m_SubscribedDisplayPowerControls = new List<IPowerDeviceControl>();

			m_Routing = new ConnectProRouting(this);
			m_Dialing = new ConnectProDialing(this);

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
			m_Routing.Dispose();
		}

		#region Methods

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
			CheckOut();
			IsInMeeting = true;

			if (resetRouting)
				Routing.RouteOsd();

			// Reset mute state
			Mute(false);

			UpdateMeetingTimeoutTimer();
		}

		/// <summary>
		/// Starts a meeting for the given booking.
		/// </summary>
		/// <param name="booking"></param>
		public void StartMeeting([NotNull] IBooking booking)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			StartMeeting(false);
			CheckIn(booking);

			Dialing.DialBooking(booking);
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

			// Power the panels
			Originators.GetInstancesRecursive<IPanelDevice>()
			           .ForEach(p => Routing.PowerDevice(p, true));
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
			foreach (IDestinationBase destination in Routing.Destinations.GetVideoDestinations())
			{
				foreach (IDeviceBase device in destination.GetDevices())
					Routing.PowerDevice(device, false);
			}

			// Power off the panels
			Originators.GetInstancesRecursive<IPanelDevice>()
			           .ForEach(p => Routing.PowerDevice(p, false));
		}

		/// <summary>
		/// Gets the ordered volume points for the current context.
		/// </summary>
		public IEnumerable<IVolumePoint> GetContextualVolumePoints()
		{
			eVolumeType type = GetVolumeTypeForContext();

			return Originators.GetInstancesRecursive<IVolumePoint>()
			                  .Where(v => EnumUtils.HasAnyFlags(v.VolumeType, type))
			                  .OrderBy(v => v, new VolumeContextComparer(type));
		}

		#endregion

		#region Protected Methods

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

		/// <summary>
		/// Called when the contents of the room change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void OriginatorsOnChildrenChanged(object sender, EventArgs args)
		{
			base.OriginatorsOnChildrenChanged(sender, args);

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

			if (CalendarControl == null)
				throw new InvalidOperationException("Room has no calendar");

			if (CalendarControl.CanCheckIn(booking))
				CalendarControl.CheckIn(booking);

			CurrentBooking = booking;
		}

		/// <summary>
		/// Checks out of the given booking.
		/// </summary>
		private void CheckOut()
		{
			if (CurrentBooking == null)
				return;

			if (CalendarControl == null)
				throw new InvalidOperationException("Room has no calendar");

			if (CalendarControl.CanCheckOut(CurrentBooking))
				CalendarControl.CheckOut(CurrentBooking);

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
		/// Clears privacy mute if we are between calls.
		/// </summary>
		private void UpdatePrivacyMute()
		{
			if (ConferenceManager == null)
				return;

			bool inCall = Dialing.ConferenceActionsAvailable(eInCall.Audio);

			if (!inCall)
				ConferenceManager.EnablePrivacyMute(false);
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
		/// Returns true if a source is actively routed to a display or we are in a conference.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsInActiveMeeting()
		{
			// Easy case!
			if (!IsInMeeting)
				return false;

			// If the user is currently on a control subpage assume that the room is being used
			if (FocusSource != null)
				return true;

			// If there is an active conference the room is being used.
			if (ConferenceManager != null && ConferenceManager.IsInCall != eInCall.None)
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

			if (!GetIsInActiveMeeting())
				return;

			Log(eSeverity.Informational, "Meeting timeout occurring at {0}", IcdEnvironment.GetLocalTime().ToShortTimeString());

			EndMeeting();
		}

		/// <summary>
		/// Updates the rooms IsAwake state.
		/// </summary>
		private void UpdateIsAwake()
		{
			IsAwake = m_SubscribedDisplayPowerControls.Any(p => p.PowerState == ePowerState.PowerOn);
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
		/// <param name="e"></param>
		private void StateOnSourceRoutedChanged(object sender, EventArgs e)
		{
			UpdateMeetingTimeoutTimer();
			UpdatePrivacyMute();
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
			UpdatePrivacyMute();

			// Return cameras to home position when entering a video call
			if (eventArgs.Data.HasFlag(eInCall.Video))
				ReturnCamerasToHome();
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

		private void Subscribe(IPowerDeviceControl displayPowerControl)
		{
			displayPowerControl.OnPowerStateChanged += DisplayPowerControlOnPowerStateChanged;
		}

		private void Unsubscribe(IPowerDeviceControl displayPowerControl)
		{
			displayPowerControl.OnPowerStateChanged -= DisplayPowerControlOnPowerStateChanged;
		}

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
