using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPROCommon.Devices;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface.EventServerKeyHandlers
{
	public sealed class AwakeEventServerKeyHandler : AbstractEventServerKeyHandler
	{
		/// <summary>
		/// Gets the key for the message handler.
		/// </summary>
		public override string Key { get { return ConnectProEventMessages.KEY_WAKE; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		/// <param name="device"></param>
		public AwakeEventServerKeyHandler(IConnectProTheme theme, ConnectProEventServerDevice device)
			: base(theme, device)
		{
			Update();
		}

		/// <summary>
		/// Updates the message.
		/// </summary>
		public override void Update()
		{
			base.Update();

			Message =
				Room != null && Room.IsAwake
					? ConnectProEventMessages.MESSAGE_WAKE
					: ConnectProEventMessages.MESSAGE_SLEEP;
		}

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
		/// Called when the room awake state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RoomOnIsAwakeStateChanged(object sender, BoolEventArgs e)
		{
			Update();
		}

		#endregion

		#region Device Callbacks

		/// <summary>
		/// Called when the device input changes for the given key.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="message"></param>
		protected override void DeviceOnInputChanged(ConnectProEventServerDevice sender, string message)
		{
			base.DeviceOnInputChanged(sender, message);

			if (Room == null)
				return;

			switch (message)
			{
				case ConnectProEventMessages.MESSAGE_WAKE:
					Room.Wake();
					break;

				case ConnectProEventMessages.MESSAGE_SLEEP:
					Room.Sleep();
					break;
			}
		}

		#endregion
	}
}
