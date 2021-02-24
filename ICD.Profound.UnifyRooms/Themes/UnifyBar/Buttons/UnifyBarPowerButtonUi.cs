using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons
{
	public sealed class UnifyBarPowerButtonUi : AbstractUnifyBarButtonUi
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyBarPowerButtonUi()
		{
			Icon = eMainButtonIcon.Power;
			Type = eMainButtonType.Normal;
			Label = "POWER";

			UpdateAwakeState();
		}

		#region Private Methods

		/// <summary>
		/// Sets the room.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(ICommercialRoom room)
		{
			base.SetRoom(room);

			UpdateAwakeState();
			UpdateVisibility();
		}

		/// <summary>
		/// Override to implement the button press behaviour.
		/// </summary>
		/// <param name="pressed"></param>
		protected override void HandleButtonPress(bool pressed)
		{
			if (!pressed)
				return;

			if (Room == null)
				return;

			if (Room.IsAwake)
				Room.Sleep();
			else
				Room.Wake();
		}

		/// <summary>
		/// Updates the room awake state.
		/// </summary>
		private void UpdateAwakeState()
		{
			Enabled = Room != null;
			Selected = Room != null && Room.IsAwake;
		}

		/// <summary>
		/// Updates the visibility of the button.
		/// </summary>
		private void UpdateVisibility()
		{
			Visible = Room != null;
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe([CanBeNull] ICommercialRoom room)
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
		protected override void Unsubscribe([CanBeNull] ICommercialRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIsAwakeStateChanged -= RoomOnIsAwakeStateChanged;
		}

		/// <summary>
		/// Called when the room awake state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RoomOnIsAwakeStateChanged(object sender, BoolEventArgs e)
		{
			UpdateAwakeState();
		}

		#endregion
	}
}
