using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Audio.Shure.Devices;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.ShureMicrophoneInterface
{
	public sealed class ConnectProShureMicrophoneInterface : AbstractUserInterface
	{
		private bool m_IsDisposed;

		private readonly IConnectProTheme m_Theme;
		private readonly IShureMicDevice m_Microphone;
		private readonly SafeCriticalSection m_RefreshSection;

		[CanBeNull]
		private IConferenceManager m_SubscribedConferenceManager;

		[CanBeNull]
		private IConnectProRoom m_Room;

		#region Properties

		[CanBeNull]
		public override IRoom Room { get { return m_Room; } }

		public IShureMicDevice Microphone { get { return m_Microphone; } }

		public override object Target { get { return Microphone; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="microphone"></param>
		/// <param name="theme"></param>
		public ConnectProShureMicrophoneInterface(IShureMicDevice microphone, IConnectProTheme theme)
		{
			m_Microphone = microphone;
			Subscribe(m_Microphone);

			m_Theme = theme;
			m_RefreshSection = new SafeCriticalSection();
		}

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
			m_SubscribedConferenceManager.Dialers.OnActiveConferenceStatusChanged += ConferenceManagerOnActiveConferenceStatusChanged;
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
			m_SubscribedConferenceManager.Dialers.OnActiveConferenceStatusChanged -= ConferenceManagerOnActiveConferenceStatusChanged;
			m_SubscribedConferenceManager.OnPrivacyMuteStatusChange -= ConferenceManagerOnPrivacyMuteStatusChange;

			m_SubscribedConferenceManager = null;
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
		/// Called when the active conference changes status.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnActiveConferenceStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			UpdateMicrophoneLeds();
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
		/// Not in a call - LED Off
		/// In a call, On Hold - LED Yellow
		/// In a call, not on hold, muted - LED Red
		/// In a call, not on hold, not muted - LED Green
		/// </summary>
		private void UpdateMicrophoneLeds()
		{
			m_RefreshSection.Enter();

			try
			{
				bool inCall = m_Room != null && m_Room.Dialing.ConferenceActionsAvailable(eInCall.Audio);
				bool onHold = m_SubscribedConferenceManager != null &&
							  m_SubscribedConferenceManager.Dialers.OnlineConferences.AnyAndAll(c => c.Status == eConferenceStatus.OnHold);
				bool privacyMuted = m_SubscribedConferenceManager != null && m_SubscribedConferenceManager.PrivacyMuted;

				eLedBrightness brightness = inCall ? eLedBrightness.Default : eLedBrightness.Disabled;

				eLedColor color =
					onHold
						? eLedColor.Yellow
						: privacyMuted
							  ? eLedColor.Red
							  : eLedColor.Green;

				m_Microphone.SetLedStatus(color, brightness);
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
		private void Subscribe(IShureMicDevice microphone)
		{
			microphone.OnMuteButtonStatusChanged += MicrophoneOnMuteButtonStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the microphone events.
		/// </summary>
		/// <param name="microphone"></param>
		private void Unsubscribe(IShureMicDevice microphone)
		{
			microphone.OnMuteButtonStatusChanged -= MicrophoneOnMuteButtonStatusChanged;
		}

		/// <summary>
		/// Called when the mute button is pressed/released.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void MicrophoneOnMuteButtonStatusChanged(object sender, BoolEventArgs boolEventArgs)
		{
			if (m_Room == null || m_SubscribedConferenceManager == null)
				return;

			// Prevent the user from toggling privacy mute while outside of a call
			if (!m_Room.Dialing.ConferenceActionsAvailable(eInCall.Audio))
				return;

			if (boolEventArgs.Data)
				m_SubscribedConferenceManager.TogglePrivacyMute();
		}

		#endregion
	}
}
