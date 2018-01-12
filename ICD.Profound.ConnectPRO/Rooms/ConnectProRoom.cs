using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Routing;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public sealed class ConnectProRoom : AbstractRoom<ConnectProRoomSettings>, IConnectProRoom
	{
		public event EventHandler<BoolEventArgs> OnIsInMeetingChanged;

		private readonly ConnectProRouting m_Routing;

		private bool m_IsInMeeting;

		private readonly IConferenceManager m_ConferenceManager;

		#region Properties

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

		public ConnectProRouting Routing { get { return m_Routing; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProRoom()
		{
			m_Routing = new ConnectProRouting(this);
		}
	}
}
