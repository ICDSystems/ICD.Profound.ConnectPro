using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.VolumePoints;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Profound.ConnectPRO.Dialing;
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

		#region Properties

		bool IsInMeeting { get; }

		/// <summary>
		/// Gets the routing features for this room.
		/// </summary>
		ConnectProRouting Routing { get; }

		/// <summary>
		/// Gets the dialing features for this room.
		/// </summary>
		ConnectProDialing Dialing { get; }

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		string Passcode { get; set; }

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
		/// Gets the ordered volume points for the current context.
		/// </summary>
		IEnumerable<IVolumePoint> GetContextualVolumePoints();

		#endregion
	}
}
