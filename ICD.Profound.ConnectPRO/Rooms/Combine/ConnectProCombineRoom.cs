using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPRO.Rooms.Combine
{
	public sealed class ConnectProCombineRoom : AbstractConnectProRoom<ConnectProCombineRoomSettings>
	{
		/// <summary>
		/// Raised when the combine advanced mode changes.
		/// </summary>
		public event EventHandler<CombineAdvancedModeEventArgs> OnCombinedAdvancedModeChanged;

		private IConnectProRoom m_MasterRoom;
		private eCombineAdvancedMode m_CombinedAdvancedMode;

		#region Properties

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

		public eCombineAdvancedMode CombinedAdvancedMode
		{
			get { return m_CombinedAdvancedMode; }
			set
			{
				if (m_CombinedAdvancedMode == value)
					return;

				m_CombinedAdvancedMode = value;

				OnCombinedAdvancedModeChanged.Raise(this, new CombineAdvancedModeEventArgs(m_CombinedAdvancedMode));
			}
		}

		/// <summary>
		/// Sets combine mode to simple when the meeting state is changed
		/// </summary>
		/// <param name="isInMeeting"></param>
		protected override void HandleIsInMeetingChanged(bool isInMeeting)
		{
			if (Routing.SupportsSimpleMode())
				CombinedAdvancedMode = eCombineAdvancedMode.Simple;
		}

		#endregion

		protected override void DisposeFinal(bool disposing)
		{
			OnCombinedAdvancedModeChanged = null;

			base.DisposeFinal(disposing);
		}

		protected override void OriginatorsOnChildrenChanged(object sender, EventArgs e)
		{
			// Call this before calling the base method
			SetMasterRoom(this.GetMasterRoom() as IConnectProRoom);

			base.OriginatorsOnChildrenChanged(sender, e);
		}

		/// <summary>
		/// Performance - We keep a local record of the master room to avoid lookups.
		/// </summary>
		/// <param name="masterRoom"></param>
		private void SetMasterRoom(IConnectProRoom masterRoom)
		{
			Unsubscribe(WakeSchedule);

			m_MasterRoom = masterRoom;

			Subscribe(WakeSchedule);

			if (m_MasterRoom == null)
				return;

			Name = m_MasterRoom.Name;
			CombineName = m_MasterRoom.Name;

			if (!Routing.SupportsSimpleMode())
				CombinedAdvancedMode = eCombineAdvancedMode.Advanced;
		}

		/// <summary>
		/// Gets the conference manager.
		/// </summary>
		protected override IConferenceManager GetConferenceManager()
		{
			return m_MasterRoom == null ? null : m_MasterRoom.ConferenceManager;
		}
	}
}
