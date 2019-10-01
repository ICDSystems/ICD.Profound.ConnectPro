using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Calendaring.Controls;
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

		/// <summary>
		/// Gets/sets the combined advanced mode.
		/// </summary>
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

		#endregion

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
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
			m_MasterRoom = masterRoom;

			Name = m_MasterRoom == null ? null : m_MasterRoom.Name;
			CombineName = m_MasterRoom == null ? null : m_MasterRoom.Name;
			WakeSchedule = m_MasterRoom == null ? null : m_MasterRoom.WakeSchedule;
			ConferenceManager = m_MasterRoom == null ? null : m_MasterRoom.ConferenceManager;

			if (!Routing.SupportsSimpleMode())
				CombinedAdvancedMode = eCombineAdvancedMode.Advanced;
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
	}
}
