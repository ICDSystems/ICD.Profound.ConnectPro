using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Commercial;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.Administrative
{
	public sealed class WakeSleepSettingsLeaf : AbstractSettingsLeaf
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
		public WakeSchedule WakeSchedule
		{
			get
			{
				if (Room == null)
					throw new InvalidOperationException("No room assigned to node");

				return Room.WakeSchedule;
			}
		}

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
		public WakeSleepSettingsLeaf()
		{
			Name = "Wake/Sleep";
			Icon = eSettingsIcon.WakeSleep;
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
			if (Room == null)
				throw new InvalidOperationException("No room assigned to node");

			if (IsAwake)
				Room.Sleep();
			else
				Room.Wake();
		}

		/// <summary>
		/// Override to initialize the node once a room has been assigned.
		/// </summary>
		protected override void Initialize(IConnectProRoom room)
		{
			base.Initialize(room);

			IsAwake = room != null && room.IsAwake;
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

			if (room == null)
				return;

			room.OnIsAwakeStateChanged += RoomOnIsAwakeStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIsAwakeStateChanged -= RoomOnIsAwakeStateChanged;
		}

		/// <summary>
		/// Called when the room wake status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RoomOnIsAwakeStateChanged(object sender, BoolEventArgs e)
		{
			if (Room == null)
				throw new InvalidOperationException("No room assigned to node");

			IsAwake = Room.IsAwake;
		}

		#endregion
	}
}
