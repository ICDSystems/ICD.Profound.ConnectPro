using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Audio.Controls.Mute;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.EventArguments;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters
{
	[PresenterBinding(typeof(IVolumePresenter))]
	public sealed class VolumePresenter : AbstractTouchDisplayPresenter<IVolumeView>, IVolumePresenter
	{
		private const ushort HIDE_TIME = 20 * 1000;
		private const float RAMP_PERCENTAGE = 3.0f / 100.0f;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly HeaderButtonModel m_HeaderVolumeButton;
		private readonly SafeTimer m_VisibilityTimer;

		private IVolumeDeviceControl m_VolumeControl;

		/// <summary>
		/// Gets/sets the volume device.
		/// </summary>
		public IVolumeDeviceControl VolumeControl
		{
			get { return m_VolumeControl; }
			set
			{
				if (value == m_VolumeControl)
					return;

				Unsubscribe(m_VolumeControl);
				m_VolumeControl = value;
				Subscribe(m_VolumeControl);

				RefreshIfVisible();
			}
		}

		public VolumePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_HeaderVolumeButton = new HeaderButtonModel(0, 2, ToggleVolumeVisibility)
			{
				Mode = eHeaderButtonMode.Green,
				Icon = TouchCueIcons.GetIcon("volumegeneric"),
				LabelText = "Volume"
			};

			m_VisibilityTimer = SafeTimer.Stopped(() => ShowView(false));
		}
		
		private void ToggleVolumeVisibility()
		{
			ShowView(!IsViewVisible);
		}

		protected override void Refresh(IVolumeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{	
				IVolumeMuteFeedbackDeviceControl volumeControlMute = VolumeControl as IVolumeMuteFeedbackDeviceControl;
				bool muted = volumeControlMute != null && volumeControlMute.VolumeIsMuted;
				IVolumePositionDeviceControl volumePositionControl = VolumeControl as IVolumePositionDeviceControl;
				float volume = volumePositionControl == null ? 0 : volumePositionControl.VolumePosition;

				view.SetMuted(muted);
				view.SetVolumePercentage(volume);
				//view.SetControlsEnabled(m_VolumeControl == null || m_VolumeControl.ControlAvailable);
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
			if (VolumeControl == null)
				return;

			ShowView(true);
			StopVisibilityTimer();

			IVolumeMuteDeviceControl volumeControlMute = VolumeControl as IVolumeMuteDeviceControl;
			if (volumeControlMute != null)
				volumeControlMute.SetVolumeMute(false);

			IVolumeRampDeviceControl volumeRampControl = VolumeControl as IVolumeRampDeviceControl;
			IVolumePositionDeviceControl volumePositionControl = volumeRampControl as IVolumePositionDeviceControl;

			if (volumePositionControl != null)
				volumePositionControl.VolumePositionRampUp(RAMP_PERCENTAGE);
			else if (volumeRampControl != null)
				volumeRampControl.VolumeRampUp();
		}

		/// <summary>
		/// Begins ramping the device volume up.
		/// </summary>
		public void VolumeSet(float percentage)
		{
			if (VolumeControl == null)
				return;

			ShowView(true);
			StopVisibilityTimer();

			IVolumeMuteDeviceControl volumeControlMute = VolumeControl as IVolumeMuteDeviceControl;
			if (volumeControlMute != null)
				volumeControlMute.SetVolumeMute(false);
			
			IVolumePositionDeviceControl volumePositionControl = VolumeControl as IVolumePositionDeviceControl;

			if (volumePositionControl != null)
				volumePositionControl.SetVolumePosition(percentage);
		}
		
		/// <summary>
		/// Begins ramping the device volume down.
		/// </summary>
		public void VolumeDown()
		{
			if (VolumeControl == null)
				return;

			ShowView(true);
			StopVisibilityTimer();

			IVolumeMuteDeviceControl volumeControlMute = VolumeControl as IVolumeMuteDeviceControl;
			if (volumeControlMute != null)
				volumeControlMute.SetVolumeMute(false);

			IVolumeRampDeviceControl volumeRampControl = VolumeControl as IVolumeRampDeviceControl;
			IVolumePositionDeviceControl volumePositionControl = volumeRampControl as IVolumePositionDeviceControl;

			if (volumePositionControl != null)
				volumePositionControl.VolumePositionRampDown(RAMP_PERCENTAGE);
			else if (volumeRampControl != null)
				volumeRampControl.VolumeRampDown();
		}
		
		/// <summary>
		/// Stops ramping the device volume.
		/// </summary>
		public void Release()
		{
			if (VolumeControl == null)
				return;

			ResetVisibilityTimer();

			IVolumeRampDeviceControl volumeControlLevel = VolumeControl as IVolumeRampDeviceControl;
			if (volumeControlLevel != null)
				volumeControlLevel.VolumeRampStop();
		}

		/// <summary>
		/// Toggles the mute state of the device.
		/// </summary>
		public void ToggleMute()
		{
			if (VolumeControl == null)
				return;

			ShowView(true);
			ResetVisibilityTimer();

			IVolumeMuteBasicDeviceControl volumeControlMute = VolumeControl as IVolumeMuteBasicDeviceControl;
			if (volumeControlMute != null)
				volumeControlMute.VolumeMuteToggle();
		}

		/// <summary>
		/// Stops the visibility timer.
		/// </summary>
		public void StopVisibilityTimer()
		{
			m_VisibilityTimer.Stop();
		}

		/// <summary>
		/// Resets the visibility timer.
		/// </summary>
		public void ResetVisibilityTimer()
		{
			m_VisibilityTimer.Reset(HIDE_TIME);
		}

		#region Room Callbacks

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
			VolumeControl = room.GetVolumeControl();
		}

		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
			VolumeControl = null;
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			IHeaderPresenter header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			if (e.Data)
				header.AddLeftButton(m_HeaderVolumeButton);
			else
				header.RemoveLeftButton(m_HeaderVolumeButton);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVolumeView view)
		{
			base.Subscribe(view);
			
			view.OnMuteButtonPressed += ViewOnMuteButtonPressed;
			view.OnVolumeButtonReleased += ViewOnVolumeButtonReleased;
			view.OnVolumeUpButtonPressed += ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed += ViewOnVolumeDownButtonPressed;
			view.OnVolumeGaugePressed += ViewOnVolumeGaugePressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVolumeView view)
		{
			base.Unsubscribe(view);
			
			view.OnMuteButtonPressed -= ViewOnMuteButtonPressed;
			view.OnVolumeButtonReleased -= ViewOnVolumeButtonReleased;
			view.OnVolumeUpButtonPressed -= ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed -= ViewOnVolumeDownButtonPressed;
			view.OnVolumeGaugePressed -= ViewOnVolumeGaugePressed;
		}

		/// <summary>
		/// Called when the user presses the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeDownButtonPressed(object sender, EventArgs eventArgs)
		{
			VolumeDown();
		}

		/// <summary>
		/// Called when the user presses the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeUpButtonPressed(object sender, EventArgs eventArgs)
		{
			VolumeUp();
		}

		/// <summary>
		/// Called when the user releases a volume button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeButtonReleased(object sender, EventArgs eventArgs)
		{
			Release();
		}

		/// <summary>
		/// Called when the user presses the mute button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnMuteButtonPressed(object sender, EventArgs eventArgs)
		{
			ToggleMute();
		}

		private void ViewOnVolumeGaugePressed(object sender, UShortEventArgs e)
		{
			
			float percentage = e.Data / (float)ushort.MaxValue;
			VolumeSet(percentage);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			ResetVisibilityTimer();
			
			if (args.Data)
				m_HeaderVolumeButton.Mode = eHeaderButtonMode.Close;
			else
				m_HeaderVolumeButton.Mode = eHeaderButtonMode.Green;
		}

		#endregion

		#region Volume Control Callbacks
		
		/// <summary>
		/// Subscribe to the control events.
		/// </summary>
		/// <param name="control"></param>
		private void Subscribe(IVolumeDeviceControl control)
		{
			if (control == null)
				return;

			control.OnControlAvailableChanged += ControlOnControlAvailableChanged;

			IVolumeMuteFeedbackDeviceControl controlMute = control as IVolumeMuteFeedbackDeviceControl;
			if (controlMute != null)
				controlMute.OnMuteStateChanged += DeviceOnMuteStateChanged;

			IVolumePositionDeviceControl controlPosition = control as IVolumePositionDeviceControl;
			if (controlPosition != null)
				controlPosition.OnVolumeChanged += DeviceOnVolumeChanged;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(IVolumeDeviceControl control)
		{
			if (control == null)
				return;

			control.OnControlAvailableChanged -= ControlOnControlAvailableChanged;

			IVolumeMuteFeedbackDeviceControl controlMute = control as IVolumeMuteFeedbackDeviceControl;
			if (controlMute != null)
				controlMute.OnMuteStateChanged -= DeviceOnMuteStateChanged;

			IVolumePositionDeviceControl controlPosition = control as IVolumePositionDeviceControl;
			if (controlPosition != null)
			{
				controlPosition.VolumeRampStop();
				controlPosition.OnVolumeChanged -= DeviceOnVolumeChanged;
			}
		}

		private void ControlOnControlAvailableChanged(object sender, DeviceControlAvailableApiEventArgs e)
		{

			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the control volume changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DeviceOnVolumeChanged(object sender, EventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the control mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DeviceOnMuteStateChanged(object sender, MuteDeviceMuteStateChangedApiEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}