using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.IncomingCalls;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.ConnectPRO.Devices;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.EventServerUserInterface
{
	public sealed class ConnectProEventServerUserInterface : AbstractUserInterface
	{
		private readonly ConnectProEventServerDevice m_Device;
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly IcdHashSet<IIncomingCall> m_IncomingCalls;

		private IConferenceManager m_SubscribedConferenceManager;

		[CanBeNull]
		private IConnectProRoom m_Room;
		private bool m_IsDisposed;

		private bool m_RoomCombined;
		private bool m_IsInCall;
		private bool m_IsAwake;
		private bool m_IsInMeeting;
		private bool m_PrivacyMuted;

		#region Properties

		[CanBeNull]
		public override IRoom Room { get { return m_Room; } }

		public ConnectProEventServerDevice Device { get { return m_Device; } }

		public override object Target { get { return Device; } }

		private bool RoomCombined
		{
			get { return m_RoomCombined; }
			set
			{
				if (value == m_RoomCombined)
					return;

				m_RoomCombined = value;

				Refresh();
			}
		}

		private bool IsInCall
		{
			get { return m_IsInCall; }
			set
			{
				if (value == m_IsInCall)
					return;

				m_IsInCall = value;

				Refresh();
			}
		}

		private bool IsAwake
		{
			get { return m_IsAwake; }
			set
			{
				if (value == m_IsAwake)
					return;

				m_IsAwake = value;

				Refresh();
			}
		}

		private bool IsInMeeting
		{
			get { return m_IsInMeeting; }
			set
			{
				if (value == m_IsInMeeting)
					return;

				m_IsInMeeting = value;

				Refresh();
			}
		}

		private bool PrivacyMuted
		{
			get { return m_PrivacyMuted; }
			set
			{
				if (value == m_PrivacyMuted)
					return;

				m_PrivacyMuted = value;

				Refresh();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="theme"></param>
		public ConnectProEventServerUserInterface(ConnectProEventServerDevice device, ConnectProTheme theme)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			m_IncomingCalls = new IcdHashSet<IIncomingCall>();
			m_RefreshSection = new SafeCriticalSection();

			m_Device = device;
		}

		#region Methods

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
				Refresh();
		}

		/// <summary>
		/// Tells the UI that it should be considered ready to use.
		/// For example updating the online join on a panel or starting a long-running process that should be delayed.
		/// </summary>
		public override void Activate()
		{
		}

		#endregion

		/// <summary>
		/// Updates the device with the current state of the room.
		/// </summary>
		private void Refresh()
		{
			m_RefreshSection.Enter();

			try
			{
				string messageInMeeting =
					IsInMeeting
						? ConnectProEventMessages.MESSAGE_IN_MEETING
						: ConnectProEventMessages.MESSAGE_OUT_OF_MEETING;

				string messageIsAwake =
					IsAwake
						? ConnectProEventMessages.MESSAGE_WAKE
						: ConnectProEventMessages.MESSAGE_SLEEP;

				string messageInCall =
					IsInCall
						? ConnectProEventMessages.MESSAGE_IN_CALL
						: ConnectProEventMessages.MESSAGE_OUT_OF_CALL;

				string messageIncomingCall =
					m_IncomingCalls.Count > 0
						? ConnectProEventMessages.MESSAGE_INCOMING_CALL
						: ConnectProEventMessages.MESSAGE_NO_INCOMING_CALL;

				string messageRoomCombined =
					RoomCombined
						? ConnectProEventMessages.MESSAGE_ROOM_COMBINED
						: ConnectProEventMessages.MESSAGE_ROOM_UNCOMBINED;

				string messagePrivacyMuted =
					PrivacyMuted
						? ConnectProEventMessages.MESSAGE_PRIVACY_MUTED
						: ConnectProEventMessages.MESSAGE_PRIVACY_UNMUTED;

				m_Device.SendMessage(ConnectProEventMessages.KEY_MEETING, messageInMeeting);
				m_Device.SendMessage(ConnectProEventMessages.KEY_WAKE, messageIsAwake);
				m_Device.SendMessage(ConnectProEventMessages.KEY_CALL, messageInCall);
				m_Device.SendMessage(ConnectProEventMessages.KEY_INCOMING_CALL, messageIncomingCall);
				m_Device.SendMessage(ConnectProEventMessages.KEY_ROOM_COMBINED, messageRoomCombined);
				m_Device.SendMessage(ConnectProEventMessages.KEY_PRIVACY_MUTE, messagePrivacyMuted);

				// Video routing
				List<ISource> videoSources =
					m_Room == null
						? new List<ISource>()
						: m_Room.Routing.State.GetFakeActiveVideoSources().SelectMany(kvp => kvp.Value).ToList();

				string videoMessage =
					videoSources.Count == 0
						? "no sources routed for video"
						: string.Format("sources routed for video {0}",
						                StringUtils.ArrayFormat(videoSources.Select(s => s.Name)));

				m_Device.SendMessage(ConnectProEventMessages.KEY_VIDEO_SOURCES, videoMessage);

				// Audio routing
				List<ISource> audioSources =
					m_Room == null
						? new List<ISource>()
						: m_Room.Routing.State.GetCachedActiveAudioSources().ToList();

				string audioMessage =
					audioSources.Count == 0
						? "no sources routed for audio"
						: string.Format("sources routed for audio {0}",
						                StringUtils.ArrayFormat(audioSources.Select(s => s.Name)));

				m_Device.SendMessage(ConnectProEventMessages.KEY_AUDIO_SOURCES, audioMessage);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
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

			// Awake state
			room.OnIsAwakeStateChanged -= RoomOnIsAwakeStateChanged;

			// Combine state
			room.OnCombineStateChanged += RoomOnCombineStateChanged;

			// Is in meeting
			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;

			// Source routed
			room.Routing.State.OnSourceRoutedChanged += RoomRoutingStateOnSourceRoutedChanged;

			m_SubscribedConferenceManager = room.ConferenceManager;
			if (m_SubscribedConferenceManager != null)
			{
				// IsInCall
				m_SubscribedConferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;

				// Incoming Call
				m_SubscribedConferenceManager.OnIncomingCallAdded += ConferenceManagerOnIncomingCallAdded;
				m_SubscribedConferenceManager.OnIncomingCallRemoved += ConferenceManagerOnIncomingCallRemoved;

				// Privacy Mute
				m_SubscribedConferenceManager.OnPrivacyMuteStatusChange += ConferenceManagerOnPrivacyMuteStatusChange;
			}
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Unsubscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			// Awake state
			room.OnIsAwakeStateChanged -= RoomOnIsAwakeStateChanged;

			// Combine state
			room.OnCombineStateChanged -= RoomOnCombineStateChanged;

			// Is in meeting
			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;

			// Source routed
			room.Routing.State.OnSourceRoutedChanged -= RoomRoutingStateOnSourceRoutedChanged;

			if (m_SubscribedConferenceManager != null)
			{
				// IsInCall
				m_SubscribedConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;

				// Privacy Mute
				m_SubscribedConferenceManager.OnPrivacyMuteStatusChange -= ConferenceManagerOnPrivacyMuteStatusChange;

				// Incoming Call
				m_SubscribedConferenceManager.OnIncomingCallAdded -= ConferenceManagerOnIncomingCallAdded;
				m_SubscribedConferenceManager.OnIncomingCallRemoved += ConferenceManagerOnIncomingCallRemoved;
			}
			m_SubscribedConferenceManager = null;
		}

		/// <summary>
		/// Called when the room wakes or goes to sleep.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnIsAwakeStateChanged(object sender, BoolEventArgs eventArgs)
		{
			IsAwake = eventArgs.Data;
		}

		/// <summary>
		/// Called when the room enters/leaves a meeting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs eventArgs)
		{
			IsInMeeting = eventArgs.Data;
		}

		/// <summary>
		/// Called when the room enters/leaves combine state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnCombineStateChanged(object sender, BoolEventArgs eventArgs)
		{
			RoomCombined = eventArgs.Data;
		}

		private void RoomRoutingStateOnSourceRoutedChanged(object sender, EventArgs eventArgs)
		{
			Refresh();
		}

		/// <summary>
		/// Called when we enable/disable privacy mute.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnPrivacyMuteStatusChange(object sender, BoolEventArgs args)
		{
			PrivacyMuted = args.Data;
		}

		/// <summary>
		/// Called when we enter a call, or leave all calls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnInCallChanged(object sender, InCallEventArgs args)
		{
			IsInCall = args.Data != eInCall.None;
		}

		/// <summary>
		/// Called when an incoming call starts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConferenceManagerOnIncomingCallAdded(object sender, ConferenceControlIncomingCallEventArgs e)
		{
			m_IncomingCalls.Add(e.IncomingCall);
			Refresh();
		}

		/// <summary>
		/// Called when an incoming call stops.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConferenceManagerOnIncomingCallRemoved(object sender, ConferenceControlIncomingCallEventArgs e)
		{
			m_IncomingCalls.Remove(e.IncomingCall);
			Refresh();
		}

		#endregion
	}
}
