using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Audio.Controls;
using ICD.Connect.Audio.EventArguments;
using ICD.Connect.Misc.Keypads;
using ICD.Connect.Panels.Crestron.Controls.TouchScreens;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.Mpc3201UserInterface
{
	public sealed class ConnectProMpc3201Interface : IUserInterface
	{
		private readonly IMPC3x201TouchScreenControl m_Control;
		private readonly ConnectProTheme m_Theme;
		private readonly SafeCriticalSection m_RefreshSection;

		private readonly IcdHashSet<ISource> m_ActiveSources;
		private readonly SafeCriticalSection m_ActiveSourcesSection;

		private bool m_IsDisposed;

		private IVolumeDeviceControl m_VolumeControl;
		private IVolumeMuteFeedbackDeviceControl m_VolumeMuteFeedbackControl;
		private ISource[] m_Sources;

		private static readonly BiDictionary<int, uint> s_IndexToSourceButton = new BiDictionary<int, uint>
		{
			{0, MPC3x201TouchScreenButtons.BUTTON_ACTION_1},
			{1, MPC3x201TouchScreenButtons.BUTTON_ACTION_2},
			{2, MPC3x201TouchScreenButtons.BUTTON_ACTION_3},
			{3, MPC3x201TouchScreenButtons.BUTTON_ACTION_4},
			{4, MPC3x201TouchScreenButtons.BUTTON_ACTION_5},
			{5, MPC3x201TouchScreenButtons.BUTTON_ACTION_6},
		}; 

		#region Properties

		/// <summary>
		/// Returns the room to an UI.
		/// </summary>
		public IConnectProRoom Room { get; private set; }

		/// <summary>
		/// Gets the touchscreen.
		/// </summary>
		public IMPC3x201TouchScreenControl TouchScreen { get { return m_Control; } }

		object IUserInterface.Target { get { return TouchScreen; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="theme"></param>
		public ConnectProMpc3201Interface(IMPC3x201TouchScreenControl control, ConnectProTheme theme)
		{
			if (control == null)
				throw new ArgumentNullException("control");

			if (theme == null)
				throw new ArgumentNullException("theme");

			m_ActiveSources = new IcdHashSet<ISource>();
			m_ActiveSourcesSection = new SafeCriticalSection();

			m_Sources = new ISource[0];

			m_Control = control;
			m_Theme = theme;

			m_RefreshSection = new SafeCriticalSection();

			Subscribe(m_Control);

			Refresh();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_IsDisposed = true;

			Unsubscribe(m_Control);

			SetRoom(null);
		}

		/// <summary>
		/// Updates the UI to represent the given room.
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

			m_Sources = GetSources(room).ToArray();

			if (!m_IsDisposed)
				Refresh();
		}

		private static IEnumerable<ISource> GetSources(IConnectProRoom room)
		{
			return room == null
				       ? Enumerable.Empty<ISource>()
				       : room.Routing.GetCoreSources();
		}

		/// <summary>
		/// Update the touchscreen LEDs.
		/// </summary>
		public void Refresh()
		{
			m_RefreshSection.Enter();

			try
			{
				// Sources
				m_ActiveSourcesSection.Enter();

				try
				{
					for (int index = 0; index < 6; index++)
					{
						ISource source;
						m_Sources.TryElementAt(index, out source);

						bool enabled = source != null;
						bool active = source != null && m_ActiveSources.Contains(source);

						m_Control.SetNumericalButtonEnabled((uint)(index + 1), enabled);
						m_Control.SetNumericalButtonSelected((uint)(index + 1), active);
					}
				}
				finally
				{
					m_ActiveSourcesSection.Leave();
				}

				// Volume
				bool volumeEnabled = m_VolumeControl is IVolumeLevelBasicDeviceControl;

				IVolumeMuteBasicDeviceControl muteControl = m_VolumeControl as IVolumeMuteBasicDeviceControl;
				bool muteEnabled = muteControl != null;
				bool muteActive = m_VolumeMuteFeedbackControl != null && m_VolumeMuteFeedbackControl.VolumeIsMuted;

				IVolumeLevelDeviceControl volumeControl = m_VolumeControl as IVolumeLevelDeviceControl;
				ushort percent = 
					volumeControl == null ? (ushort)0 : (ushort)(MathUtils.Clamp(volumeControl.VolumePosition, 0, 1) * ushort.MaxValue);

				m_Control.SetVolumeBargraph(percent);

				m_Control.SetMuteButtonEnabled(muteEnabled);
				m_Control.SetMuteButtonSelected(muteActive);
				
				m_Control.SetVolumeUpButtonEnabled(volumeEnabled);
				m_Control.SetVolumeDownButtonEnabled(volumeEnabled);

				// Power
				bool inMeeting = Room != null && Room.IsInMeeting;
				m_Control.SetPowerButtonEnabled(true);
				m_Control.SetPowerButtonSelected(!inMeeting);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
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
			room.Routing.OnDisplaySourceChanged += RoutingOnDisplaySourceChanged;

			m_VolumeControl = room.GetVolumeControl();
			m_VolumeMuteFeedbackControl = m_VolumeControl as IVolumeMuteFeedbackDeviceControl;
			Subscribe(m_VolumeControl);
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
			room.Routing.OnDisplaySourceChanged -= RoutingOnDisplaySourceChanged;

			Unsubscribe(m_VolumeMuteFeedbackControl);
			m_VolumeMuteFeedbackControl = null;
			m_VolumeControl = null;
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
			m_ActiveSourcesSection.Enter();

			try
			{
				IcdHashSet<ISource> activeSources =
					Room.Routing.GetCachedActiveVideoSources()
					    .SelectMany(kvp => kvp.Value)
					    .ToIcdHashSet();

				if (activeSources.SetEquals(m_ActiveSources))
					return;

				m_ActiveSources.Clear();
				m_ActiveSources.AddRange(activeSources);
			}
			finally
			{
				m_ActiveSourcesSection.Leave();
			}

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
			if (Room != null && !Room.IsInMeeting)
				Room.Routing.RouteOsd();
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
					if (eventArgs.ButtonState == eButtonState.Pressed)
						ToggleMute();
					break;

				case MPC3x201TouchScreenButtons.BUTTON_VOLUME_DOWN:
					switch (eventArgs.ButtonState)
					{
						case eButtonState.Pressed:
							VolumeDown();
							break;
						case eButtonState.Released:
							VolumeRelease();
							break;
					}
					break;

				case MPC3x201TouchScreenButtons.BUTTON_VOLUME_UP:
					switch (eventArgs.ButtonState)
					{
						case eButtonState.Pressed:
							VolumeUp();
							break;
						case eButtonState.Released:
							VolumeRelease();
							break;
					}
					break;

				case MPC3x201TouchScreenButtons.BUTTON_POWER:
					if (eventArgs.ButtonState == eButtonState.Pressed)
						TogglePower();
					break;

				case MPC3x201TouchScreenButtons.BUTTON_ACTION_1:
				case MPC3x201TouchScreenButtons.BUTTON_ACTION_2:
				case MPC3x201TouchScreenButtons.BUTTON_ACTION_3:
				case MPC3x201TouchScreenButtons.BUTTON_ACTION_4:
				case MPC3x201TouchScreenButtons.BUTTON_ACTION_5:
				case MPC3x201TouchScreenButtons.BUTTON_ACTION_6:
					if (eventArgs.ButtonState == eButtonState.Pressed)
					{
						int index = s_IndexToSourceButton.GetKey(eventArgs.ButtonId);
						HandlePressedSource(index);
					}
					break;
			}
		}

		private void HandlePressedSource(int index)
		{
			if (Room == null)
				return;

			ISource source;
			m_Sources.TryElementAt(index, out source);

			if (source == null)
				return;

			// Start the meeting if we are not currently in one
			if (!Room.IsInMeeting)
				Room.StartMeeting(false);

			// Route the source to the display
			Room.Routing.RouteSingleDisplay(source);
		}

		private void TogglePower()
		{
			if (Room != null)
				Room.EndMeeting(false);
		}

		private void VolumeUp()
		{
			IVolumeLevelBasicDeviceControl volumeControl = m_VolumeControl as IVolumeLevelBasicDeviceControl;
			if (volumeControl != null)
				volumeControl.VolumeLevelRampUp();
		}

		private void VolumeDown()
		{
			IVolumeLevelBasicDeviceControl volumeControl = m_VolumeControl as IVolumeLevelBasicDeviceControl;
			if (volumeControl != null)
				volumeControl.VolumeLevelRampDown();
		}

		private void VolumeRelease()
		{
			IVolumeLevelBasicDeviceControl volumeControl = m_VolumeControl as IVolumeLevelBasicDeviceControl;
			if (volumeControl != null)
				volumeControl.VolumeLevelRampStop();
		}

		private void ToggleMute()
		{
			IVolumeMuteBasicDeviceControl muteControl = m_VolumeControl as IVolumeMuteBasicDeviceControl;
			if (muteControl != null)
				muteControl.VolumeMuteToggle();
		}

		#endregion

		#region Volume Control Callbacks

		/// <summary>
		/// Subscribe to the volume control events.
		/// </summary>
		/// <param name="volumeControl"></param>
		private void Subscribe(IVolumeDeviceControl volumeControl)
		{
			IVolumeLevelDeviceControl volumeLevelControl = volumeControl as IVolumeLevelDeviceControl;
			if (volumeLevelControl != null)
				volumeLevelControl.OnVolumeChanged += VolumeLevelControlOnVolumeChanged;

			IVolumeMuteFeedbackDeviceControl volumeMuteControl = volumeControl as IVolumeMuteFeedbackDeviceControl;
			if (volumeMuteControl != null)
				volumeMuteControl.OnMuteStateChanged += VolumeMuteControlOnMuteStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the volume control events.
		/// </summary>
		/// <param name="volumeControl"></param>
		private void Unsubscribe(IVolumeDeviceControl volumeControl)
		{
			IVolumeLevelDeviceControl volumeLevelControl = volumeControl as IVolumeLevelDeviceControl;
			if (volumeLevelControl != null)
				volumeLevelControl.OnVolumeChanged -= VolumeLevelControlOnVolumeChanged;

			IVolumeMuteFeedbackDeviceControl volumeMuteControl = volumeControl as IVolumeMuteFeedbackDeviceControl;
			if (volumeMuteControl != null)
				volumeMuteControl.OnMuteStateChanged -= VolumeMuteControlOnMuteStateChanged;
		}

		/// <summary>
		/// Called when the mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void VolumeMuteControlOnMuteStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			Refresh();
		}

		/// <summary>
		/// Called when the volume level changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void VolumeLevelControlOnVolumeChanged(object sender, VolumeDeviceVolumeChangedEventArgs eventArgs)
		{
			Refresh();
		}

		#endregion
	}
}
