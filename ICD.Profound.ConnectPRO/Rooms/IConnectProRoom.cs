using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Routing;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public interface IConnectProRoom : IRoom
	{
		event EventHandler<BoolEventArgs> OnIsInMeetingChanged;

		bool IsInMeeting { get; set; }

		/// <summary>
		/// Gets the routing features for this room.
		/// </summary>
		ConnectProRouting Routing { get; }

		/// <summary>
		/// Gets the conference manager.
		/// </summary>
		IConferenceManager ConferenceManager { get; }
	}
}
