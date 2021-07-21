using System.Linq;
using ICD.Connect.Partitioning.Partitions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPROCommon.Devices;

namespace ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface.EventServerKeyHandlers
{
	public sealed class RoomCombineEventServerKeyHandler : AbstractEventServerKeyHandler
	{
		/// <summary>
		/// Gets the key for the message handler.
		/// </summary>
		public override string Key { get { return ConnectProEventMessages.KEY_ROOM_COMBINED; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		/// <param name="device"></param>
		public RoomCombineEventServerKeyHandler(IConnectProTheme theme, ConnectProEventServerDevice device)
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
				Room != null && (Room.IsCombineRoom() || Room.CombineState)
					? ConnectProEventMessages.MESSAGE_ROOM_COMBINED
					: ConnectProEventMessages.MESSAGE_ROOM_UNCOMBINED;
		}

		#region Device Callbacks

		/// <summary>
		/// Called when the device input changes for the given key.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="message"></param>
		protected override void DeviceOnInputChanged(ConnectProEventServerDevice sender, string message)
		{
			base.DeviceOnInputChanged(sender, message);

			IPartition[] partitions =
				Theme.Core
				     .Originators
				     .GetChildren<IPartition>()
				     .ToArray();

			switch (message)
			{
				case ConnectProEventMessages.MESSAGE_ROOM_COMBINED:
					// Combine all?
					Theme.CombineRooms(new IPartition[0], partitions);
					break;

				case ConnectProEventMessages.MESSAGE_ROOM_UNCOMBINED:
					// Uncombine all?
					Theme.CombineRooms(partitions, new IPartition[0]);
					break;
			}
		}

		#endregion
	}
}
