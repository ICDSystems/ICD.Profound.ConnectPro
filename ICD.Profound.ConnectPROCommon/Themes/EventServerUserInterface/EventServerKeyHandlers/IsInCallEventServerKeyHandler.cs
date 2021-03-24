using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPROCommon.Devices;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface.EventServerKeyHandlers
{
	public sealed class IsInCallEventServerKeyHandler : AbstractEventServerKeyHandler
	{
		private IConferenceManager m_ConferenceManager;

		/// <summary>
		/// Gets the key for the message handler.
		/// </summary>
		public override string Key { get { return ConnectProEventMessages.KEY_CALL; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		/// <param name="device"></param>
		public IsInCallEventServerKeyHandler(IConnectProTheme theme, ConnectProEventServerDevice device)
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
				m_ConferenceManager != null && m_ConferenceManager.Dialers.IsInCall != eInCall.None
					? ConnectProEventMessages.MESSAGE_IN_CALL
					: ConnectProEventMessages.MESSAGE_OUT_OF_CALL;
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

			m_ConferenceManager.Dialers.OnInCallChanged += ConferenceManagerOnInCallChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_ConferenceManager != null)
				m_ConferenceManager.Dialers.OnInCallChanged -= ConferenceManagerOnInCallChanged;

			m_ConferenceManager = null;
		}

		/// <summary>
		/// Called when the in call state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConferenceManagerOnInCallChanged(object sender, InCallEventArgs e)
		{
			Update();
		}

		#endregion
	}
}
