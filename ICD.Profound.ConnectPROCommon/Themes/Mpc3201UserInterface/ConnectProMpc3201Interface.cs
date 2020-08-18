using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.Repeaters;
using ICD.Connect.Audio.Utils;
using ICD.Connect.Misc.Keypads;
using ICD.Connect.Panels.Crestron.Controls.TouchScreens;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.ConnectPROCommon.EventArguments;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPROCommon.Themes.Mpc3201UserInterface
{
	public sealed class ConnectProMpc3201Interface : AbstractUserInterface
	{
		private const long POWER_BUTTON_HOLD_MILLISECONDS = (long)(0.5f * 1000);

		private static readonly BiDictionary<int, uint> s_IndexToSourceButton = new BiDictionary<int, uint>
		{
			{0, MPC3x201TouchScreenButtons.BUTTON_ACTION_1},
			{1, MPC3x201TouchScreenButtons.BUTTON_ACTION_2},
			{2, MPC3x201TouchScreenButtons.BUTTON_ACTION_3},
			{3, MPC3x201TouchScreenButtons.BUTTON_ACTION_4},
			{4, MPC3x201TouchScreenButtons.BUTTON_ACTION_5},
			{5, MPC3x201TouchScreenButtons.BUTTON_ACTION_6},
		};

		private readonly IMPC3x201TouchScreenControl m_Control;
		private readonly IConnectProTheme m_Theme;
		private readonly SafeCriticalSection m_RefreshSection;

		private readonly SafeTimer m_HoldTimer;
		private readonly VolumeRepeater m_VolumeRepeater;
		private readonly VolumePointHelper m_VolumePointHelper;

		private bool m_IsDisposed;

		[CanBeNull]
		private IConnectProRoom m_Room;

		#region Properties

		public override IRoom Room { get { return m_Room; } }

		/// <summary>
		/// Gets the touchscreen.
		/// </summary>
		public IMPC3x201TouchScreenControl TouchScreen { get { return m_Control; } }

		public override object Target { get { return m_Control; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="theme"></param>
		public ConnectProMpc3201Interface(IMPC3x201TouchScreenControl control, IConnectProTheme theme)
		{
			if (control == null)
				throw new ArgumentNullException("control");

			if (theme == null)
				throw new ArgumentNullException("theme");

			m_VolumeRepeater = new VolumeRepeater();
			m_VolumePointHelper = new VolumePointHelper();
			m_RefreshSection = new SafeCriticalSection();
			m_HoldTimer = SafeTimer.Stopped(PowerButtonHeld);

			m_Control = control;
			m_Theme = theme;

			Subscribe(m_VolumePointHelper);
			Subscribe(m_Control);

			Refresh();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_IsDisposed = true;
			m_VolumeRepeater.Dispose();

			Unsubscribe(m_Control);

			Unsubscribe(m_VolumePointHelper);
			m_VolumePointHelper.Dispose();

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
		/// Updates the UI to represent the given room.
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

		/// <summary>
		/// Update the touchscreen LEDs.
		/// </summary>
		public void Refresh()
		{
			m_RefreshSection.Enter();

			try
			{
				IEnumerable<ISource> sources =
					m_Room == null
						? Enumerable.Empty<ISource>()
						: m_Room.Routing
						        .Sources
						        .GetRoomSourcesForUi(eSourceAppearance.Routing)
						        .Take(6); // Only 6 buttons

				// Sources
				int index = 0;
				foreach (ISource source in sources)
				{
					bool enabled = source != null;

					bool active = m_Room != null &&
					              enabled &&
					              m_Room.Routing.State.GetIsRoutedCached(source, eConnectionType.Video) &&
					              m_Room.IsInMeeting;

					m_Control.SetNumericalButtonEnabled((uint)(index + 1), enabled);
					m_Control.SetNumericalButtonSelected((uint)(index + 1), active);

					index++;
				}

				// Volume
				bool volumeEnabled = m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Volume);
				ushort percent = (ushort)(MathUtils.Clamp(m_VolumePointHelper.GetVolumePercent(), 0, 1) * ushort.MaxValue);

				// Mute
				bool muteEnabled = m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Mute);
				bool muteActive = m_VolumePointHelper.IsMuted;

				m_Control.SetVolumeBargraph(percent);

				m_Control.SetMuteButtonEnabled(muteEnabled);
				m_Control.SetMuteButtonSelected(muteActive);
				
				m_Control.SetVolumeUpButtonEnabled(volumeEnabled);
				m_Control.SetVolumeDownButtonEnabled(volumeEnabled);

				// Power
				bool inMeeting = m_Room != null && m_Room.IsInMeeting;
				m_Control.SetPowerButtonEnabled(true);
				m_Control.SetPowerButtonSelected(!inMeeting);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Begins ramping the device volume up.
		/// </summary>
		public void VolumeUp()
		{
			if (!m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment))
				return;

			m_VolumeRepeater.VolumeUpHold(m_VolumePointHelper.VolumePoint);
		}

		/// <summary>
		/// Begins ramping the device volume down.
		/// </summary>
		public void VolumeDown()
		{
			if (!m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment))
				return;

			m_VolumeRepeater.VolumeDownHold(m_VolumePointHelper.VolumePoint);
		}

		/// <summary>
		/// Stops ramping the device volume.
		/// </summary>
		public void VolumeRelease()
		{
			m_VolumeRepeater.Release();
		}

		/// <summary>
		/// Toggles the mute state of the device.
		/// </summary>
		public void ToggleMute()
		{
			if (!m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Mute))
				return;

			m_VolumePointHelper.ToggleIsMuted();
		}

		#endregion

		#region Private Methods

		private static IEnumerable<ISource> GetSources(IConnectProRoom room)
		{
			return room == null
				       ? Enumerable.Empty<ISource>()
				       : room.Routing.Sources.GetRoomSources();
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

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
			room.Routing.State.OnDisplaySourceChanged += RoutingOnDisplaySourceChanged;
			room.Routing.State.OnSourceRoutedChanged += StateOnSourceRoutedChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Unsubscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
			room.Routing.State.OnDisplaySourceChanged -= RoutingOnDisplaySourceChanged;
			room.Routing.State.OnSourceRoutedChanged -= StateOnSourceRoutedChanged;
		}

		/// <summary>
		/// Called when the room enters/leaves a meeting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs boolEventArgs)
		{
			Refresh();
		}

		/// <summary>
		/// Called when the active source for a display changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoutingOnDisplaySourceChanged(object sender, EventArgs eventArgs)
		{
			Refresh();
		}

		/// <summary>
		/// Called when a source routing state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="sourceRoutedEventArgs"></param>
		private void StateOnSourceRoutedChanged(object sender, SourceRoutedEventArgs sourceRoutedEventArgs)
		{
			Refresh();
		}

		#endregion

		#region TouchScreen Callbacks

		/// <summary>
		/// Subscribe to the control callbacks.
		/// </summary>
		/// <param name="control"></param>
		private void Subscribe(IMPC3x201TouchScreenControl control)
		{
			control.OnButtonStateChange += ControlOnButtonStateChange;
			control.OnProximityDetectedStateChange += ControlOnProximityDetectedStateChange;
		}

		/// <summary>
		/// Unsubscribe from the control callbacks.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(IMPC3x201TouchScreenControl control)
		{
			control.OnButtonStateChange -= ControlOnButtonStateChange;
			control.OnProximityDetectedStateChange -= ControlOnProximityDetectedStateChange;
		}

		/// <summary>
		/// Called when the touchscreen proximity detection state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ControlOnProximityDetectedStateChange(object sender, BoolEventArgs eventArgs)
		{
			if (!eventArgs.Data)
				return;

			// Reset the routing for the room when proximity is detected
			if (m_Room != null && !m_Room.IsInMeeting)
				m_Room.Wake();
		}

		/// <summary>
		/// Called when a touchscreen button state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ControlOnButtonStateChange(object sender, KeypadButtonPressedEventArgs eventArgs)
		{
			switch (eventArgs.ButtonId)
			{
				case MPC3x201TouchScreenButtons.BUTTON_MUTE:
					HandleMuteButton(eventArgs.ButtonState == eButtonState.Pressed);
					break;

				case MPC3x201TouchScreenButtons.BUTTON_VOLUME_DOWN:
					HandleVolumeDownButton(eventArgs.ButtonState == eButtonState.Pressed);
					break;

				case MPC3x201TouchScreenButtons.BUTTON_VOLUME_UP:
					HandleVolumeUpButton(eventArgs.ButtonState == eButtonState.Pressed);
					break;

				case MPC3x201TouchScreenButtons.BUTTON_POWER:
					HandlePowerButton(eventArgs.ButtonState == eButtonState.Pressed);
					break;

				case MPC3x201TouchScreenButtons.BUTTON_ACTION_1:
				case MPC3x201TouchScreenButtons.BUTTON_ACTION_2:
				case MPC3x201TouchScreenButtons.BUTTON_ACTION_3:
				case MPC3x201TouchScreenButtons.BUTTON_ACTION_4:
				case MPC3x201TouchScreenButtons.BUTTON_ACTION_5:
				case MPC3x201TouchScreenButtons.BUTTON_ACTION_6:
					int index = s_IndexToSourceButton.GetKey(eventArgs.ButtonId);
					HandleSourceButton(index, eventArgs.ButtonState == eButtonState.Pressed);
					break;
			}
		}

		private void HandleMuteButton(bool pressed)
		{
			if (pressed)
				ToggleMute();
		}

		private void HandleVolumeDownButton(bool pressed)
		{
			if (pressed)
				VolumeDown();
			else
				VolumeRelease();
		}

		private void HandleVolumeUpButton(bool pressed)
		{
			if (pressed)
				VolumeUp();
			else
				VolumeRelease();
		}

		private void HandlePowerButton(bool pressed)
		{
			if (pressed)
			{
				m_HoldTimer.Reset(POWER_BUTTON_HOLD_MILLISECONDS);
				return;
			}

			m_HoldTimer.Stop();

			// Check IsInMeeting because it's possible we're releasing after holding
			if (m_Room != null && m_Room.IsInMeeting)
				m_Room.EndMeeting(false);
		}

		private void PowerButtonHeld()
		{
			if (m_Room != null)
				m_Room.EndMeeting(true);
		}

		private void HandleSourceButton(int index, bool pressed)
		{
			if (!pressed)
				return;

			if (m_Room == null)
				return;

			IEnumerable<ISource> sources =
				m_Room == null
					? Enumerable.Empty<ISource>()
					: m_Room.Routing
					        .Sources
					        .GetRoomSourcesForUi(eSourceAppearance.Routing)
					        .Take(6); // Only 6 buttons

			ISource source;
			if (!sources.TryElementAt(index, out source) || source == null)
				return;

			// Start the meeting if we are not currently in one
			if (!m_Room.IsInMeeting)
				m_Room.StartMeeting(null, source);
		}

		#endregion

		#region Volume Control Callbacks

		/// <summary>
		/// Subscribe to the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		private void Subscribe(VolumePointHelper volumePointHelper)
		{
			if (volumePointHelper == null)
				return;

			volumePointHelper.OnVolumeControlAvailableChanged += VolumePointHelperOnVolumeControlAvailableChanged;
			volumePointHelper.OnVolumeControlVolumeChanged += VolumePointHelperOnVolumeControlVolumeChanged;
			volumePointHelper.OnVolumeControlIsMutedChanged += VolumePointHelperOnVolumeControlIsMutedChanged;
		}

		/// <summary>
		/// Unsubscribe from the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		private void Unsubscribe(VolumePointHelper volumePointHelper)
		{
			if (volumePointHelper == null)
				return;

			volumePointHelper.OnVolumeControlAvailableChanged -= VolumePointHelperOnVolumeControlAvailableChanged;
			volumePointHelper.OnVolumeControlVolumeChanged -= VolumePointHelperOnVolumeControlVolumeChanged;
			volumePointHelper.OnVolumeControlIsMutedChanged -= VolumePointHelperOnVolumeControlIsMutedChanged;
		}

		/// <summary>
		/// Called when the mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void VolumePointHelperOnVolumeControlIsMutedChanged(object sender, BoolEventArgs eventArgs)
		{
			Refresh();
		}

		/// <summary>
		/// Called when the volume level changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void VolumePointHelperOnVolumeControlVolumeChanged(object sender, FloatEventArgs eventArgs)
		{
			Refresh();
		}

		/// <summary>
		/// Called when the volume control availability changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void VolumePointHelperOnVolumeControlAvailableChanged(object sender, BoolEventArgs eventArgs)
		{
			Refresh();
		}

		#endregion
	}
}
