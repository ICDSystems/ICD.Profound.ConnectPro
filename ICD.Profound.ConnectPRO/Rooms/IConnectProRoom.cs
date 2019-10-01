using System;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Calendaring.CalendarControl;
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

		#endregion

		#region Methods

		/// <summary>
		/// Gets the volume control matching the configured volume point.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		IVolumeDeviceControl GetVolumeControl();

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		void StartMeeting();

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		void StartMeeting(bool resetRouting);

		/// <summary>
		/// Ends the meeting state. If shutdown is true fully powers down the room.
		/// </summary>
		/// <param name="shutdown"></param>
		void EndMeeting(bool shutdown);

		/// <summary>
		/// Wakes up the room.
		/// </summary>
		void Wake();

		/// <summary>
		/// Shuts down the room.
		/// </summary>
		void Sleep();

		#endregion
	}
}
