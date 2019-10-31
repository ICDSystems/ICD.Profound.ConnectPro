﻿using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.ConnectPRO.Devices;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing;

namespace ICD.Profound.ConnectPRO.Themes.EventServerUserInterface
{
	public sealed class ConnectProEventServerUserInterface : AbstractUserInterface
	{
		private readonly ConnectProEventServerDevice m_Device;
		private readonly SafeCriticalSection m_RefreshSection;

		private IConferenceManager m_SubscribedConferenceManager;

		private IConnectProRoom m_Room;
		private bool m_IsDisposed;

		private bool m_RoomCombined;
		private bool m_SourceRouted;
		private bool m_IncomingCall;
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

		private bool SourceRouted
		{
			get { return m_SourceRouted; }
			set
			{
				if (value == m_SourceRouted)
					return;

				m_SourceRouted = value;

				Refresh();
			}
		}

		private bool IncomingCall
		{
			get { return m_IncomingCall; }
			set
			{
				if (value == m_IncomingCall)
					return;

				m_IncomingCall = value;

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
					IncomingCall
						? ConnectProEventMessages.MESSAGE_INCOMING_CALL
						: ConnectProEventMessages.MESSAGE_NO_INCOMING_CALL;

				string messageSourceRouted =
					SourceRouted
						? ConnectProEventMessages.MESSAGE_SOURCE_ROUTED
						: ConnectProEventMessages.MESSAGE_SOURCE_UNROUTED;

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
				m_Device.SendMessage(ConnectProEventMessages.KEY_SOURCE_ROUTED, messageSourceRouted);
				m_Device.SendMessage(ConnectProEventMessages.KEY_ROOM_COMBINED, messageRoomCombined);
				m_Device.SendMessage(ConnectProEventMessages.KEY_PRIVACY_MUTE, messagePrivacyMuted);
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

			// Combine State
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

			// Combine State
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
			}
			m_SubscribedConferenceManager = null;
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
			SourceRouted =
				m_Room != null &&
				m_Room.Routing
				      .State
				      .GetSourceRoutedStates()
				      .Select(kvp => kvp.Value)
				      .Any(p => p == eSourceState.Active);
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

		#endregion
	}
}