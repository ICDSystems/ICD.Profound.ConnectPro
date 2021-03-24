using System;
using ICD.Common.Properties;
using ICD.Profound.ConnectPROCommon.Devices;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface.EventServerKeyHandlers
{
	public abstract class AbstractEventServerKeyHandler : IEventServerKeyHandler
	{
		private readonly IConnectProTheme m_Theme;
		private readonly ConnectProEventServerDevice m_Device;

		private string m_Message;
		private IConnectProRoom m_Room;

		#region Properties

		/// <summary>
		/// Gets the key for the message handler.
		/// </summary>
		public abstract string Key { get; }

		/// <summary>
		/// Gets the current message.
		/// </summary>
		public string Message
		{
			get { return m_Message; }
			protected set
			{
				if (value == m_Message)
					return;

				m_Message = value;

				m_Device.SendMessage(Key, m_Message);
			}
		}

		/// <summary>
		/// Gets the room.
		/// </summary>
		public IConnectProRoom Room { get { return m_Room; } }

		/// <summary>
		/// Gets the theme.
		/// </summary>
		public IConnectProTheme Theme { get { return m_Theme; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		/// <param name="device"></param>
		protected AbstractEventServerKeyHandler([NotNull] IConnectProTheme theme, [NotNull] ConnectProEventServerDevice device)
		{
			if (theme == null)
				throw new ArgumentNullException("theme");

			if (device == null)
				throw new ArgumentNullException("device");

			m_Theme = theme;
			m_Device = device;

			Subscribe(m_Device);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			Unsubscribe(m_Device);

			SetRoom(null);
		}

		#region Methods

		/// <summary>
		/// Sets the room.
		/// </summary>
		/// <param name="value"></param>
		public virtual void SetRoom([CanBeNull] IConnectProRoom value)
		{
			if (value == m_Room)
				return;

			Unsubscribe(m_Room);
			m_Room = value;
			Subscribe(m_Room);
		}

		/// <summary>
		/// Pushes the current state to the device.
		/// </summary>
		public virtual void Update()
		{
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Subscribe([CanBeNull] IConnectProRoom room)
		{
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Unsubscribe([CanBeNull] IConnectProRoom room)
		{
		}

		#endregion

		#region Device Callbacks

		/// <summary>
		/// Subscribe to the device events.
		/// </summary>
		/// <param name="device"></param>
		private void Subscribe(ConnectProEventServerDevice device)
		{
			device.RegisterInputKeyHandler(Key, DeviceOnInputChanged);
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="device"></param>
		private void Unsubscribe(ConnectProEventServerDevice device)
		{
			device.DeregisterInputKeyHandler(Key, DeviceOnInputChanged);
		}

		/// <summary>
		/// Called when the device input changes for the given key.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="message"></param>
		protected virtual void DeviceOnInputChanged(ConnectProEventServerDevice sender, string message)
		{
		}

		#endregion
	}
}
