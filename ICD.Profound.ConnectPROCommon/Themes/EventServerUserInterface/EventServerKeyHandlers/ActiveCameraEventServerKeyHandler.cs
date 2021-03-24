using ICD.Common.Utils.EventArguments;
using ICD.Connect.Devices;
using ICD.Profound.ConnectPROCommon.Devices;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface.EventServerKeyHandlers
{
	public sealed class ActiveCameraEventServerKeyHandler : AbstractEventServerKeyHandler
	{
		/// <summary>
		/// Gets the key for the message handler.
		/// </summary>
		public override string Key { get { return ConnectProEventMessages.KEY_ACTIVE_CAMERA; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		/// <param name="device"></param>
		public ActiveCameraEventServerKeyHandler(IConnectProTheme theme, ConnectProEventServerDevice device)
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

			IDeviceBase activeCamera = Room == null ? null : Room.ActiveCamera;
			Message =
				activeCamera == null
					? "no active camera selected"
					: string.Format("active camera [{0}]", activeCamera.Name);
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

			room.OnActiveCameraChanged += RoomOnActiveCameraChanged;
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

			room.OnActiveCameraChanged -= RoomOnActiveCameraChanged;
		}

		/// <summary>
		/// Called when the active camera for the room changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnActiveCameraChanged(object sender, GenericEventArgs<IDeviceBase> eventArgs)
		{
			Update();
		}

		#endregion
	}
}
