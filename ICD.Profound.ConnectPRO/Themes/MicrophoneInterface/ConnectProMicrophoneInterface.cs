using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
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
		private readonly SafeCriticalSection m_RefreshSection;

		private IConferenceManager m_SubscribedConferenceManager;

		[CanBeNull]
		public IConnectProRoom Room { get; private set; }

		public IShureMxaDevice Microphone { get { return m_Microphone; } }

		object IUserInterface.Target { get { return Microphone; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="microphone"></param>
		/// <param name="theme"></param>
		public ConnectProMicrophoneInterface(IShureMxaDevice microphone, ConnectProTheme theme)
		{
			m_Microphone = microphone;
			Subscribe(m_Microphone);

			m_Theme = theme;
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_IsDisposed = true;

			Unsubscribe(m_Microphone);

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

			ServiceProvider.GetService<ILoggerService>()
			               .AddEntry(eSeverity.Informational, "{0} setting room to {1}", this, room);

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

			m_SubscribedConferenceManager = null;
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

			m_RefreshSection.Enter();

			try
			{
				if (m_SubscribedConferenceManager != null && m_SubscribedConferenceManager.IsInCall >= eInCall.Audio)
				{
					brightness = eLedBrightness.Default;

					color = m_SubscribedConferenceManager.ActiveConference.Status == eConferenceStatus.OnHold
								? eLedColor.Yellow
								: m_SubscribedConferenceManager.PrivacyMuted
									  ? eLedColor.Red
									  : eLedColor.Green;
				}

				m_Microphone.SetLedBrightness(brightness);
				m_Microphone.SetLedColor(color);
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
		private void Subscribe(IShureMxaDevice microphone)
		{
			microphone.OnMuteButtonStatusChanged += MicrophoneOnMuteButtonStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the microphone events.
		/// </summary>
		/// <param name="microphone"></param>
		private void Unsubscribe(IShureMxaDevice microphone)
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
			if (boolEventArgs.Data && m_SubscribedConferenceManager != null)
				m_SubscribedConferenceManager.TogglePrivacyMute();
		}

		#endregion
	}
}
