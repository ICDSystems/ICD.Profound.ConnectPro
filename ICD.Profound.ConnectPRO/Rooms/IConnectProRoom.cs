﻿using System;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.IncomingCalls;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Profound.ConnectPRO.Routing;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public interface IConnectProRoom : ICommercialRoom
	{
		/// <summary>
		/// Raised when the room starts/stops a meeting.
		/// </summary>
		event EventHandler<BoolEventArgs> OnIsInMeetingChanged;

		/// <summary>
		/// Raised when the source that is currently the primary focus of the room (i.e. VTC) changes.
		/// </summary>
		event EventHandler<SourceEventArgs> OnFocusSourceChanged;

		/// <summary>
		/// Raised when an incoming call is answered.
		/// </summary>
		event EventHandler<GenericEventArgs<IIncomingCall>> OnIncomingCallAnswered;

		/// <summary>
		/// Raised when an incoming call is rejected.
		/// </summary>
		event EventHandler<GenericEventArgs<IIncomingCall>> OnIncomingCallRejected;

		#region Properties

		bool IsInMeeting { get; }

		/// <summary>
		/// Gets the routing features for this room.
		/// </summary>
		ConnectProRouting Routing { get; }

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		string Passcode { get; set; }

		/// <summary>
		/// Gets/sets the ATC number for dialing into the room.
		/// </summary>
		string AtcNumber { get; set; }

		/// <summary>
		/// Gets the CalendarControl for the room.
		/// </summary>
		ICalendarControl CalendarControl { get; }

		/// <summary>
		/// Gets the selected OBTP booking.
		/// </summary>
		IBooking CurrentBooking { get; }

		/// <summary>
		/// Gets/sets the source that is currently the primary focus of the room (i.e. VTC).
		/// </summary>
		[CanBeNull]
		ISource FocusSource { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		void StartMeeting();

		/// <summary>
		/// Enters the meeting state with chosen OBTP booking.
		/// </summary>
		/// <param name="booking">Selected OBTP booking</param>
		void StartMeeting(IBooking booking);

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		/// <param name="resetRouting">true to reset routing to default, false to leave as is</param>
		void StartMeeting(bool resetRouting);

		/// <summary>
		/// Ends the meeting state. If shutdown is true fully powers down the room.
		/// </summary>
		/// <param name="shutdown">true to fully power down room</param>
		void EndMeeting(bool shutdown);

		/// <summary>
		/// Ends the meeting state. If it is currently sleep time fully powers down the room.
		/// </summary>
		void EndMeeting();

		/// <summary>
		/// Wakes up the room.
		/// </summary>
		void Wake();

		/// <summary>
		/// Shuts down the room.
		/// </summary>
		void Sleep();

		/// <summary>
		/// Answers the incoming call and focuses on the given conference call.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="call"></param>
		void AnswerIncomingCall(IConferenceDeviceControl control, IIncomingCall call);

		/// <summary>
		/// Rejects the incoming call.
		/// </summary>
		/// <param name="call"></param>
		void RejectIncomingCall(IIncomingCall call);

		/// <summary>
		/// Returns true if:
		/// 
		/// We are in a conference and the conference source does not use the Hide override.
		/// OR
		/// We have a routed source using the Show override.
		/// </summary>
		/// <param name="minimumCallType"></param>
		/// <returns></returns>
		bool ConferenceActionsAvailable(eInCall minimumCallType);

		#endregion
	}
}
