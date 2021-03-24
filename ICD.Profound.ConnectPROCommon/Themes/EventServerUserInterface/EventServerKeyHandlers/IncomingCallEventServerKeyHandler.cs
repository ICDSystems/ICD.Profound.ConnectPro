using ICD.Common.Utils.Collections;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.IncomingCalls;
using ICD.Profound.ConnectPROCommon.Devices;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface.EventServerKeyHandlers
{
	public sealed class IncomingCallEventServerKeyHandler : AbstractEventServerKeyHandler
	{
		private readonly IcdHashSet<IIncomingCall> m_IncomingCalls;

		private IConferenceManager m_ConferenceManager;

		/// <summary>
		/// Gets the key for the message handler.
		/// </summary>
		public override string Key { get { return ConnectProEventMessages.KEY_INCOMING_CALL; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		/// <param name="device"></param>
		public IncomingCallEventServerKeyHandler(IConnectProTheme theme, ConnectProEventServerDevice device)
			: base(theme, device)
		{
			m_IncomingCalls = new IcdHashSet<IIncomingCall>();

			Update();
		}

		/// <summary>
		/// Updates the message.
		/// </summary>
		public override void Update()
		{
			base.Update();

			Message =
				m_IncomingCalls.Count > 0
					? ConnectProEventMessages.MESSAGE_INCOMING_CALL
					: ConnectProEventMessages.MESSAGE_NO_INCOMING_CALL;
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

			m_ConferenceManager.Dialers.OnIncomingCallAdded += ConferenceManagerOnIncomingCallAdded;
			m_ConferenceManager.Dialers.OnIncomingCallRemoved += ConferenceManagerOnIncomingCallRemoved;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_ConferenceManager != null)
			{
				m_ConferenceManager.Dialers.OnIncomingCallAdded -= ConferenceManagerOnIncomingCallAdded;
				m_ConferenceManager.Dialers.OnIncomingCallRemoved -= ConferenceManagerOnIncomingCallRemoved;
			}

			m_ConferenceManager = null;
		}

		/// <summary>
		/// Called when an incoming call starts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConferenceManagerOnIncomingCallAdded(object sender, ConferenceControlIncomingCallEventArgs e)
		{
			m_IncomingCalls.Add(e.IncomingCall);
			Update();
		}

		/// <summary>
		/// Called when an incoming call stops.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConferenceManagerOnIncomingCallRemoved(object sender, ConferenceControlIncomingCallEventArgs e)
		{
			m_IncomingCalls.Remove(e.IncomingCall);
			Update();
		}

		#endregion
	}
}
