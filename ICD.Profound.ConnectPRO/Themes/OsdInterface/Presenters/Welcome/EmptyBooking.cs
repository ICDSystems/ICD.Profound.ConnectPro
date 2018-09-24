using System;
using ICD.Connect.Calendaring.Booking;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Welcome
{
	public class EmptyBooking : IBooking
	{
		/// <summary>
		/// Returns the name of the meeting.
		/// </summary>
		public string MeetingName
		{
			get { return "Available"; }
		}

		/// <summary>
		/// Returns the organizer's name.
		/// </summary>
		public string OrganizerName
		{
			get { return null; }
		}

		/// <summary>
		/// Returns the organizer's email.
		/// </summary>
		public string OrganizerEmail
		{
			get { return null; }
		}

		/// <summary>
		/// Returns the meeting start time.
		/// </summary>
		public DateTime StartTime { get; set; }

		/// <summary>
		/// Returns the meeting end time.
		/// </summary>
		public DateTime EndTime { get; set; }

		/// <summary>
		/// Returns the meeting type.
		/// </summary>
		public eMeetingType Type
		{
			get { return eMeetingType.Presentation; }
		}

		/// <summary>
		/// Returns true if meeting is private.
		/// </summary>
		public bool IsPrivate
		{
			get { return false; }
		}
	}
}