using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.Shure;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.MicrophoneInterface
{
	public sealed class ConnectProMicrophoneInterface : IUserInterface
	{
		private bool m_IsDisposed;

		private readonly ConnectProTheme m_Theme;
		private readonly IShureMxaDevice m_Microphone;

		private IConferenceManager m_SubscribedConferenceManager;

		[CanBeNull]
		private IConnectProRoom Room { get; set; }

		public IShureMxaDevice Microphone { get { return m_Microphone; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="microphone"></param>
		/// <param name="theme"></param>
		public ConnectProMicrophoneInterface(IShureMxaDevice microphone, ConnectProTheme theme)
		{
			m_Microphone = microphone;
			m_Theme = theme;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_IsDisposed = true;

			SetRoom(null);
		}

		/// <summary>
		/// Sets the room for this interface.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			if (room == Room)
				return;

			Unsubscribe(Room);
			Room = room;
			Subscribe(Room);

			if (!m_IsDisposed)
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

			m_SubscribedConferenceManager = room.ConferenceManager;
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;
			m_SubscribedConferenceManager.OnRecentConferenceAdded += ConferenceManagerOnRecentConferenceAdded;
			m_SubscribedConferenceManager.OnActiveConferenceStatusChanged += ConferenceManagerOnActiveConferenceStatusChanged;
			m_SubscribedConferenceManager.OnPrivacyMuteStatusChange += ConferenceManagerOnPrivacyMuteStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Unsubscribe(IConnectProRoom room)
		{
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;
			m_SubscribedConferenceManager.OnRecentConferenceAdded -= ConferenceManagerOnRecentConferenceAdded;
			m_SubscribedConferenceManager.OnActiveConferenceStatusChanged -= ConferenceManagerOnActiveConferenceStatusChanged;
			m_SubscribedConferenceManager.OnPrivacyMuteStatusChange -= ConferenceManagerOnPrivacyMuteStatusChange;
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
		/// Called when a conference is added to the conference manager.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnRecentConferenceAdded(object sender, ConferenceEventArgs args)
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
			eLedBrightness brightness = eLedBrightness.Disabled;
			eLedColor color = eLedColor.White;

			IConferenceManager conferenceManager = Room == null ? null : Room.ConferenceManager;

			if (conferenceManager != null && conferenceManager.IsInCall >= eInCall.Audio)
			{
				brightness = eLedBrightness.Default;

				color = conferenceManager.ActiveConference.Status == eConferenceStatus.OnHold
					        ? eLedColor.Yellow
					        : conferenceManager.PrivacyMuted
						        ? eLedColor.Red
						        : eLedColor.Green;
			}

			m_Microphone.SetLedBrightness(brightness);
			m_Microphone.SetLedColor(color);
		}

		#endregion
	}
}
