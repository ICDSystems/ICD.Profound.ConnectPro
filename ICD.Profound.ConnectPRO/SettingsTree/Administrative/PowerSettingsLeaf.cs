using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Commercial;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Administrative
{
	public sealed class PowerSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Raised when the room wakes or goes to sleep.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnRoomIsAwakeStateChanged;

		private bool m_IsAwake;

		#region Properties

		/// <summary>
		/// Gets the wake schedule.
		/// </summary>
		public WakeSchedule WakeSchedule { get { return Room.WakeSchedule; } }

		/// <summary>
		/// Gets the display power state.
		/// </summary>
		public bool IsAwake
		{
			get { return m_IsAwake; }
			private set
			{
				if (value == m_IsAwake)
					return;

				m_IsAwake = value;

				OnRoomIsAwakeStateChanged.Raise(this, new BoolEventArgs(m_IsAwake));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public PowerSettingsLeaf(IConnectProRoom room)
			: base(room)
		{
			Name = "Power";
			Icon = SettingsTreeIcons.ICON_POWER;
			IsAwake = room.IsAwake;
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnRoomIsAwakeStateChanged = null;

			base.Dispose();
		}

		/// <summary>
		/// Toggles the current is awake state.
		/// </summary>
		public void ToggleIsAwake()
		{
			if (IsAwake)
				Room.Sleep();
			else
				Room.Wake();
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			room.OnIsAwakeStateChanged += RoomOnIsAwakeStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			room.OnIsAwakeStateChanged -= RoomOnIsAwakeStateChanged;
		}

		/// <summary>
		/// Called when the room wake status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RoomOnIsAwakeStateChanged(object sender, BoolEventArgs e)
		{
			IsAwake = Room.IsAwake;
		}

		#endregion
	}
}