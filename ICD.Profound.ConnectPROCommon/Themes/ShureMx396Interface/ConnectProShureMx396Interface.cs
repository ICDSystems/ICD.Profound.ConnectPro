using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Audio.Shure.Devices.MX;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.ShureMx396Interface
{
	public sealed class ConnectProShureMx396Interface : AbstractUserInterface
	{
		private readonly ShureMx396Device m_Microphone;
		private readonly SafeCriticalSection m_RefreshSection;

		[CanBeNull]
		private IConferenceManager m_SubscribedConferenceManager;

		[CanBeNull]
		private IConnectProRoom m_Room;
		private bool m_IsDisposed;

		#region Properties

		[CanBeNull]
		public override IRoom Room { get { return m_Room; } }

		public ShureMx396Device Microphone { get { return m_Microphone; } }

		public override object Target { get { return Microphone; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="microphone"></param>
		public ConnectProShureMx396Interface([NotNull] ShureMx396Device microphone)
		{
			if (microphone == null)
				throw new ArgumentNullException("microphone");

			m_Microphone = microphone;
			Subscribe(m_Microphone);

			m_RefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_IsDisposed = true;

			Unsubscribe(m_Microphone);

			SetRoom(null);
		}

		/// <summary>
		/// Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IRoom room)
		{
			SetRoom(room as IConnectProRoom);
		}

		/// <summary>
		/// Sets the room for this interface.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			if (room == m_Room)
				return;

			ServiceProvider.GetService<ILoggerService>()
			               .AddEntry(eSeverity.Informational, "{0} setting room to {1}", this, room);

			Unsubscribe(m_Room);
			m_Room = room;
			Subscribe(m_Room);

			if (!m_IsDisposed)
				UpdateMicrophoneLeds();
		}

		/// <summary>
		/// Tells the UI that it should be considered ready to use.
		/// For example updating the online join on a panel or starting a long-running process that should be delayed.
		/// </summary>
		public override void Activate()
		{
			UpdateMicrophoneLeds();
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Subscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.Routing.State.OnSourceRoutedChanged += StateOnSourceRoutedChanged;

			m_SubscribedConferenceManager = room.ConferenceManager;
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.Dialers.OnInCallChanged += ConferenceManagerOnInCallChanged;
			m_SubscribedConferenceManager.OnPrivacyMuteStatusChange += ConferenceManagerOnPrivacyMuteStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Unsubscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.Routing.State.OnSourceRoutedChanged -= StateOnSourceRoutedChanged;

			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.Dialers.OnInCallChanged -= ConferenceManagerOnInCallChanged;
			m_SubscribedConferenceManager.OnPrivacyMuteStatusChange -= ConferenceManagerOnPrivacyMuteStatusChange;

			m_SubscribedConferenceManager = null;
		}

		/// <summary>
		/// Called when we enable/disable privacy mute.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnPrivacyMuteStatusChange(object sender, BoolEventArgs args)
		{
			UpdateMicrophoneLeds();
		}

		/// <summary>
		/// Called when we enter a call, or leave all calls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnInCallChanged(object sender, InCallEventArgs args)
		{
			UpdateMicrophoneLeds();
		}


		/// <summary>
		/// Called when a source becomes routed/unrouted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StateOnSourceRoutedChanged(object sender, EventArgs eventArgs)
		{
			UpdateMicrophoneLeds();
		}

		/// <summary>
		/// Not in a call - LED Green
		/// In a call, muted - LED Red
		/// In a call, not muted - LED Green
		/// </summary>
		private void UpdateMicrophoneLeds()
		{
			m_RefreshSection.Enter();

			try
			{
				bool inCall = m_Room != null && m_Room.Dialing.ConferenceActionsAvailable(eInCall.Audio);
				bool privacyMuted = m_SubscribedConferenceManager != null && m_SubscribedConferenceManager.PrivacyMuted;

				bool green = !inCall || !privacyMuted;

				m_Microphone.SetLedState(green);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region Microphone Callbacks

		/// <summary>
		/// Subscribe to the microphone events.
		/// </summary>
		/// <param name="microphone"></param>
		private void Subscribe(ShureMx396Device microphone)
		{
			microphone.OnButtonPressedChanged += MicrophoneOnButtonPressedChanged;
		}

		/// <summary>
		/// Unsubscribe from the microphone events.
		/// </summary>
		/// <param name="microphone"></param>
		private void Unsubscribe(ShureMx396Device microphone)
		{
			microphone.OnButtonPressedChanged -= MicrophoneOnButtonPressedChanged;
		}

		/// <summary>
		/// Called when the mute button is pressed/released.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void MicrophoneOnButtonPressedChanged(object sender, BoolEventArgs eventArgs)
		{
			if (m_Room == null || m_SubscribedConferenceManager == null)
				return;

			// Prevent the user from toggling privacy mute while outside of a call
			if (!m_Room.Dialing.ConferenceActionsAvailable(eInCall.Audio))
				return;

			if (eventArgs.Data)
				m_SubscribedConferenceManager.TogglePrivacyMute();
		}

		#endregion
	}
}
