using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Audio.ClockAudio.Devices.CCRM4000;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.Ccrm4000UserInterface
{
	public sealed class ConnectProCcrm4000UserInterface : AbstractUserInterface
	{
		private bool m_IsDisposed;

		private readonly IConnectProTheme m_Theme;
		private readonly ClockAudioCcrm4000Device m_Microphone;

		[CanBeNull]
		private IConferenceManager m_SubscribedConferenceManager;

		[CanBeNull]
		private IConnectProRoom m_Room;

		#region Properties

		[CanBeNull]
		public override IRoom Room { get { return m_Room; } }

		public ClockAudioCcrm4000Device Microphone { get { return m_Microphone; } }

		public override object Target { get { return Microphone; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="microphone"></param>
		/// <param name="theme"></param>
		public ConnectProCcrm4000UserInterface(ClockAudioCcrm4000Device microphone, IConnectProTheme theme)
		{
			m_Microphone = microphone;
			m_Theme = theme;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_IsDisposed = true;

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
				ActuateMicrophone();
		}

		/// <summary>
		/// Tells the UI that it should be considered ready to use.
		/// For example updating the online join on a panel or starting a long-running process that should be delayed.
		/// </summary>
		public override void Activate()
		{
			ActuateMicrophone();
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

			m_SubscribedConferenceManager = null;
		}

		/// <summary>
		/// Called when a source becomes routed/unrouted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StateOnSourceRoutedChanged(object sender, EventArgs eventArgs)
		{
			ActuateMicrophone();
		}

		/// <summary>
		/// Called when we enter a call, or leave all calls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnInCallChanged(object sender, InCallEventArgs args)
		{
			ActuateMicrophone();
		}

		/// <summary>
		/// Extend the microphone when in a call, retract when out of a call.
		/// </summary>
		private void ActuateMicrophone()
		{
			bool inCall = m_Room != null && m_Room.Dialing.ConferenceActionsAvailable(eInCall.Audio);

			if (inCall)
				m_Microphone.Extend();
			else
				m_Microphone.Retract();
		}

		#endregion
	}
}
