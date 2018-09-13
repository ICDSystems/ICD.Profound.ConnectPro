﻿using System;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.Controls;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Routing;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public interface IConnectProRoom : IRoom
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
		/// Gets the conference manager.
		/// </summary>
		IConferenceManager ConferenceManager { get; }

		/// <summary>
		/// Gets the wake/sleep schedule.
		/// </summary>
		WakeSchedule WakeSchedule { get; }

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		string Passcode { get; set; }

		/// <summary>
		/// Gets/sets the ATC number for dialing into the room.
		/// </summary>
		string AtcNumber { get; set; }

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
