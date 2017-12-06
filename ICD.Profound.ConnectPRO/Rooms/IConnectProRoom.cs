using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public interface IConnectProRoom : IRoom
	{
		event EventHandler<BoolEventArgs> OnIsInMeetingChanged;

		bool IsInMeeting { get; set; }
	}
}
