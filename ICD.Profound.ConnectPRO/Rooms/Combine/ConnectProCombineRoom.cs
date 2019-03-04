using System;
using ICD.Connect.Calendaring.CalendarControl;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPRO.Rooms.Combine
{
	public sealed class ConnectProCombineRoom : AbstractConnectProRoom<ConnectProCombineRoomSettings>
	{
		private IConnectProRoom m_MasterRoom;

		#region Properties

		/// <summary>
		/// Gets the conference manager.
		/// </summary>
		public override IConferenceManager ConferenceManager { get { return m_MasterRoom == null ? null : m_MasterRoom.ConferenceManager; } }

		/// <summary>
		/// Gets the wake/sleep schedule.
		/// </summary>
		public override WakeSchedule WakeSchedule { get { return m_MasterRoom == null ? null : m_MasterRoom.WakeSchedule; } }

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		public override string Passcode
		{
			get { return m_MasterRoom == null ? null : m_MasterRoom.Passcode; }
			set
			{
				if (m_MasterRoom != null)
					m_MasterRoom.Passcode = value;
			}
		}

		/// <summary>
		/// Gets/sets the ATC number for dialing into the room.
		/// </summary>
		public override string AtcNumber
		{
			get { return m_MasterRoom == null ? null : m_MasterRoom.AtcNumber; }
			set
			{
				if (m_MasterRoom != null)
					m_MasterRoom.AtcNumber = value;
			}
		}

		/// <summary>
		/// Gets the CalendarControl for the room.
		/// </summary>
		public override ICalendarControl CalendarControl { get { return m_MasterRoom == null ? null : m_MasterRoom.CalendarControl; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProCombineRoom()
		{
			Originators.OnChildrenChanged += OriginatorsOnChildrenChanged;
		}

		protected override void DisposeFinal(bool disposing)
		{
			Originators.OnChildrenChanged -= OriginatorsOnChildrenChanged;

			base.DisposeFinal(disposing);
		}

		private void OriginatorsOnChildrenChanged(object sender, EventArgs e)
		{
			Unsubscribe(WakeSchedule);

			m_MasterRoom = this.GetMasterRoom() as IConnectProRoom;

			Subscribe(WakeSchedule);

			if (m_MasterRoom == null)
				return;

			Name = m_MasterRoom.Name;
			CombineName = m_MasterRoom.Name;
		}
	}
}
