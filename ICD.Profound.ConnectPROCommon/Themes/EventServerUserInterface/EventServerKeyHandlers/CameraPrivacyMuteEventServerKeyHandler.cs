using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Profound.ConnectPROCommon.Devices;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface.EventServerKeyHandlers
{
	public sealed class CameraPrivacyMuteEventServerKeyHandler : AbstractEventServerKeyHandler
	{
		private IConferenceManager m_ConferenceManager;

		/// <summary>
		/// Gets the key for the message handler.
		/// </summary>
		public override string Key { get { return ConnectProEventMessages.KEY_PRIVACY_MUTE; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		/// <param name="device"></param>
		public CameraPrivacyMuteEventServerKeyHandler(IConnectProTheme theme, ConnectProEventServerDevice device)
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
				m_ConferenceManager != null && m_ConferenceManager.CameraPrivacyMuted
					? ConnectProEventMessages.MESSAGE_CAMERA_PRIVACY_MUTED
					: ConnectProEventMessages.MESSAGE_CAMERA_PRIVACY_UNMUTED;
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			m_ConferenceManager = room == null ? null : room.ConferenceManager;
			if (m_ConferenceManager == null)
				return;

			m_ConferenceManager.OnCameraPrivacyMuteStatusChange += ConferenceManagerOnCameraPrivacyMuteStatusChange;
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

			if (m_ConferenceManager != null)
				m_ConferenceManager.OnCameraPrivacyMuteStatusChange -= ConferenceManagerOnCameraPrivacyMuteStatusChange;

			m_ConferenceManager = null;
		}

		/// <summary>
		/// Called when the conference manager camera privacy mute status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ConferenceManagerOnCameraPrivacyMuteStatusChange(object sender, BoolEventArgs boolEventArgs)
		{
			Update();
		}

		#endregion

		/// <summary>
		/// Called when the device input changes for the given key.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="message"></param>
		protected override void DeviceOnInputChanged(ConnectProEventServerDevice sender, string message)
		{
			base.DeviceOnInputChanged(sender, message);

			if (m_ConferenceManager == null)
				return;

			switch (message)
			{
				case ConnectProEventMessages.MESSAGE_CAMERA_PRIVACY_MUTED:
					m_ConferenceManager.CameraPrivacyMuted = true;
					break;

				case ConnectProEventMessages.MESSAGE_CAMERA_PRIVACY_UNMUTED:
					m_ConferenceManager.CameraPrivacyMuted = false;
					break;
			}
		}
	}
}
