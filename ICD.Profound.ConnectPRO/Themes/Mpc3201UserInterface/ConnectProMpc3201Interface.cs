using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Audio.Controls;
using ICD.Connect.Audio.EventArguments;
using ICD.Connect.Misc.Keypads;
using ICD.Connect.Panels.Crestron.Controls.TouchScreens;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.Mpc3201UserInterface
{
	public sealed class ConnectProMpc3201Interface : IUserInterface
	{
		private readonly IMPC3x201TouchScreenControl m_Control;
		private readonly ConnectProTheme m_Theme;
		private readonly SafeCriticalSection m_RefreshSection;

		private bool m_IsDisposed;

		private IVolumeDeviceControl m_VolumeControl;

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

			if (!m_IsDisposed)
				Refresh();
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
				

				// Volume
				IVolumeMuteBasicDeviceControl muteControl = m_VolumeControl as IVolumeMuteBasicDeviceControl;
				bool muteEnabled = muteControl != null;

				IVolumeLevelDeviceControl volumeControl = m_VolumeControl as IVolumeLevelDeviceControl;
				ushort percent = 
					volumeControl == null ? (ushort)0 : (ushort)(MathUtils.Clamp(volumeControl.VolumePosition, 0, 1) * ushort.MaxValue);

				m_Control.SetMuteButtonEnabled(muteEnabled);
				m_Control.SetVolumeBargraph(percent);

				// Power
				m_Control.SetPowerButtonEnabled(true);
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

			m_VolumeControl = room.GetVolumeControl();
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

			Unsubscribe(m_VolumeControl);
			m_VolumeControl = null;
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
		}

		/// <summary>
		/// Unsubscribe from the control callbacks.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(IMPC3x201TouchScreenControl control)
		{
			control.OnButtonStateChange -= ControlOnButtonStateChange;
		}

		/// <summary>
		/// Called when a touchscreen button state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ControlOnButtonStateChange(object sender, KeypadButtonPressedEventArgs eventArgs)
		{
			IcdConsole.PrintLine(eConsoleColor.Cyan, "Button {0} State {1}", eventArgs.ButtonId, eventArgs.ButtonState);
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
			if (volumeLevelControl == null)
				return;

			volumeLevelControl.OnVolumeChanged += VolumeLevelControlOnVolumeChanged;
		}

		/// <summary>
		/// Unsubscribe from the volume control events.
		/// </summary>
		/// <param name="volumeControl"></param>
		private void Unsubscribe(IVolumeDeviceControl volumeControl)
		{
			IVolumeLevelDeviceControl volumeLevelControl = volumeControl as IVolumeLevelDeviceControl;
			if (volumeLevelControl == null)
				return;

			volumeLevelControl.OnVolumeChanged -= VolumeLevelControlOnVolumeChanged;
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
