using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public sealed class ConnectProRoom : AbstractRoom<ConnectProRoomSettings>, IConnectProRoom
	{
		public event EventHandler<BoolEventArgs> OnIsInMeetingChanged; 

		private bool m_IsInMeeting;

		public bool IsInMeeting
		{
			get { return m_IsInMeeting; }
			set
			{
				if (value == m_IsInMeeting)
					return;

				m_IsInMeeting = value;

				OnIsInMeetingChanged.Raise(this, new BoolEventArgs(m_IsInMeeting));
			}
		}
	}
}
