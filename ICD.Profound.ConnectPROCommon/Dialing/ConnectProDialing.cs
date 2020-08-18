using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Calendaring;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.IncomingCalls;
using ICD.Connect.Devices;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.Dialing.ConferenceSetup;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPROCommon.Dialing
{
	public sealed class ConnectProDialing
	{
		/// <summary>
		/// Raised when an incoming call is answered.
		/// </summary>
		public event EventHandler<GenericEventArgs<IIncomingCall>> OnIncomingCallAnswered;

		/// <summary>
		/// Raised when an incoming call is rejected.
		/// </summary>
		public event EventHandler<GenericEventArgs<IIncomingCall>> OnIncomingCallRejected;

		/// <summary>
		/// Raised when a conference setup process starts/stops.
		/// </summary>
		public event EventHandler OnConferenceSetupChanged;

		private readonly IConnectProRoom m_Room;

		private IConferenceSetup m_ConferenceSetup;

		#region Properties

		/// <summary>
		/// Gets the current conference setup process.
		/// </summary>
		[CanBeNull]
		public IConferenceSetup ConferenceSetup
		{
			get { return m_ConferenceSetup; }
			private set
			{
				if (value == m_ConferenceSetup)
					return;

				Unsubscribe(m_ConferenceSetup);

				if (m_ConferenceSetup != null)
					m_ConferenceSetup.Dispose();

				m_ConferenceSetup = value;
				Subscribe(m_ConferenceSetup);

				OnConferenceSetupChanged.Raise(this);
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ConnectProDialing([NotNull] IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			m_Room = room;
		}

		#region Methods

		/// <summary>
		/// Dials the given booking and routes the dialer.
		/// </summary>
		/// <param name="booking"></param>
		/// <returns>Returns true if we were able to dial a number associated with the booking</returns>
		public bool DialBooking([NotNull] IBooking booking)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			IEnumerable<IConferenceDeviceControl> dialers =
				m_Room.ConferenceManager == null
					? Enumerable.Empty<IConferenceDeviceControl>()
					: m_Room.ConferenceManager.Dialers.GetDialingProviders();

			// Build map of dialer to best number
			IDialContext dialContext;
			IConferenceDeviceControl preferredDialer = ConferencingBookingUtils.GetBestDialer(booking, dialers, out dialContext);
			if (preferredDialer == null)
				return false;

			// Route device to displays and/or audio destination
			IDevice dialerDevice = preferredDialer.Parent;
			ISource source = m_Room.Routing.Sources.GetRoomSources().FirstOrDefault(s => s.Device == dialerDevice.Id);
			if (source == null)
				return false; // if we can't route a source, don't dial into conference users won't know they're in

			m_Room.FocusSource = source;

			if (preferredDialer.Supports.HasFlag(eCallType.Video))
				m_Room.Routing.RouteVtc(source);
			else if (preferredDialer.Supports.HasFlag(eCallType.Audio))
				m_Room.Routing.RouteAtc(source);
			else
				m_Room.Routing.RouteToAllDisplays(source);

			// Dial booking
			Dial(preferredDialer, dialContext);
			return true;
		}

		/// <summary>
		/// Dials the context using the given conference control.
		/// </summary>
		/// <param name="conferenceControl"></param>
		/// <param name="context"></param>
		public void Dial([NotNull] IConferenceDeviceControl conferenceControl, [NotNull] IDialContext context)
		{
			if (conferenceControl == null)
				throw new ArgumentNullException("conferenceControl");

			if (context == null)
				throw new ArgumentNullException("context");

			SetupCall(conferenceControl, () => conferenceControl.Dial(context));
		}

		/// <summary>
		/// Starts a personal meeting using the given web conference control.
		/// </summary>
		/// <param name="webConferenceControl"></param>
		public void StartPersonalMeeting([NotNull] IWebConferenceDeviceControl webConferenceControl)
		{
			if (webConferenceControl == null)
				throw new ArgumentNullException("webConferenceControl");

			SetupCall(webConferenceControl, webConferenceControl.StartPersonalMeeting);
		}

		/// <summary>
		/// Answers the incoming call and focuses on the given conference call.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="call"></param>
		public void AnswerIncomingCall([CanBeNull] IConferenceDeviceControl control, [NotNull] IIncomingCall call)
		{
			if (call == null)
				throw new ArgumentNullException("call");

			SetupCall(control, call.Answer);

			m_Room.StartMeeting(null, null);

			// Focus on the dialer source
			IDeviceBase device = control == null ? null : control.Parent;
			ISource source = device == null ? null : m_Room.Originators.GetInstanceRecursive<ISource>(s => s.Device == device.Id);

			m_Room.FocusSource = source;

			if (source != null && control.Supports.HasFlag(eCallType.Video))
				m_Room.Routing.RouteVtc(source);
			else if (source != null && control.Supports.HasFlag(eCallType.Audio))
				m_Room.Routing.RouteAtc(source);

			OnIncomingCallAnswered.Raise(this, new GenericEventArgs<IIncomingCall>(call));
		}

		/// <summary>
		/// Rejects the incoming call.
		/// </summary>
		/// <param name="call"></param>
		public void RejectIncomingCall(IIncomingCall call)
		{
			if (call == null)
				throw new ArgumentNullException("call");

			call.Reject();

			OnIncomingCallRejected.Raise(this, new GenericEventArgs<IIncomingCall>(call));
		}

		/// <summary>
		/// Ends all of the online conferences.
		/// </summary>
		public void EndAllConferences()
		{
			List<IConference> activeConferences =
				m_Room.ConferenceManager == null
					? new List<IConference>()
					: m_Room.ConferenceManager.Dialers.ActiveConferences.ToList();

			foreach (IConference activeConference in activeConferences)
				EndConference(activeConference);
		}

		/// <summary>
		/// Returns true if:
		/// 
		/// We are in a conference and the conference source does not use the Hide override.
		/// OR
		/// We have a routed source using the Show override.
		/// </summary>
		/// <param name="minimumCallType"></param>
		/// <returns></returns>
		public bool ConferenceActionsAvailable(eInCall minimumCallType)
		{
			// Are we in a conference and the source is NOT using the Hide override?
			if (m_Room.ConferenceManager != null && m_Room.ConferenceManager.Dialers.IsInCall >= minimumCallType)
				return GetActiveConferenceSourceOverride() != eConferenceOverride.Hide;

			// Is a source routed with the Show override?
			return
				m_Room.Routing
				      .State
				      .GetFakeActiveVideoSources()
				      .SelectMany(kvp => kvp.Value)
				      .OfType<ConnectProSource>()
				      .Any(s => s.ConferenceOverride == eConferenceOverride.Show);
		}

		#endregion

		#region Private Methods

		private static void EndConference([NotNull] IConference conference)
		{
			if (conference == null)
				throw new ArgumentNullException("conference");

			// TODO - Actually use polymorphism like a good developer
			var traditional = conference as ITraditionalConference;
			if (traditional != null)
				traditional.Hangup();

			var web = conference as IWebConference;
			if (web != null)
				web.LeaveConference();
		}

		/// <summary>
		/// Returns the conference override for the active conferences. Show beats Hide.
		/// </summary>
		/// <returns></returns>
		private eConferenceOverride GetActiveConferenceSourceOverride()
		{
			if (m_Room.ConferenceManager == null)
				return eConferenceOverride.None;

			return
				m_Room.ConferenceManager.Dialers
				      .GetDialingProviders()
				      .Where(p => p.GetActiveConference() != null)
				      .SelectMany(p => m_Room.Routing.Sources.GetSources(p))
				      .OfType<ConnectProSource>()
				      .Select(s => s.ConferenceOverride)
				      .MaxOrDefault();
		}

		/// <summary>
		/// Runs the necessary setup for the given conference control before starting the call.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="startCall"></param>
		private void SetupCall([CanBeNull] IConferenceDeviceControl control, [NotNull] Action startCall)
		{
			if (startCall == null)
				throw new ArgumentNullException("startCall");

			IConferenceSetup setup =
				control == null
					? null
					: ConferenceSetupFactory.BuildConferenceSetup(m_Room, control, startCall);

			// No setup necessary
			if (setup == null)
			{
				startCall();
				return;
			}

			setup.Start();
		}

		#endregion

		#region Conference Setup Callbacks

		/// <summary>
		/// Subscribe to the conference setup events.
		/// </summary>
		/// <param name="conferenceSetup"></param>
		private void Subscribe(IConferenceSetup conferenceSetup)
		{
			if (conferenceSetup == null)
				return;

			conferenceSetup.OnFinished += ConferenceSetupOnFinished;
		}

		/// <summary>
		/// Unsubscribe from the conference setup events.
		/// </summary>
		/// <param name="conferenceSetup"></param>
		private void Unsubscribe(IConferenceSetup conferenceSetup)
		{
			if (conferenceSetup == null)
				return;

			conferenceSetup.OnFinished -= ConferenceSetupOnFinished;
		}

		private void ConferenceSetupOnFinished(object sender, EventArgs eventArgs)
		{
			ConferenceSetup = null;
		}

		#endregion
	}
}
