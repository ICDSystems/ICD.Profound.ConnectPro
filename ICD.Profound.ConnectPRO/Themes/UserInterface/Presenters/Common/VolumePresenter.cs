using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Audio.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class VolumePresenter : AbstractPresenter<IVolumeView>, IVolumePresenter
	{
		private const ushort HIDE_TIME = 20 * 1000;

		private readonly SafeTimer m_VisibilityTimer;
		private readonly SafeCriticalSection m_RefreshSection;

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

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VolumePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_VisibilityTimer = SafeTimer.Stopped(() => ShowView(false));
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_VisibilityTimer.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVolumeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IVolumeMuteFeedbackDeviceControl volumeControlMute = VolumeControl as IVolumeMuteFeedbackDeviceControl;
				bool muted = volumeControlMute != null && volumeControlMute.VolumeIsMuted;
				IVolumeLevelDeviceControl volumeControlLevel = VolumeControl as IVolumeLevelDeviceControl;
				float volume = volumeControlLevel == null ? 0 : volumeControlLevel.VolumePosition;

				view.SetMuted(muted);
				view.SetVolumePercentage(volume);
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

			IVolumeLevelBasicDeviceControl volumeControlLevel = VolumeControl as IVolumeLevelBasicDeviceControl;
			if (volumeControlLevel != null)
				volumeControlLevel.VolumeLevelRampUp();
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

			IVolumeLevelBasicDeviceControl volumeControlLevel = VolumeControl as IVolumeLevelBasicDeviceControl;
			if (volumeControlLevel != null)
				volumeControlLevel.VolumeLevelRampDown();
		}

		/// <summary>
		/// Stops ramping the device volume.
		/// </summary>
		public void Release()
		{
			if (VolumeControl == null)
				return;

			ResetVisibilityTimer();

			IVolumeLevelBasicDeviceControl volumeControlLevel = VolumeControl as IVolumeLevelBasicDeviceControl;
			if (volumeControlLevel != null)
				volumeControlLevel.VolumeLevelRampStop();
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

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVolumeView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
			view.OnMuteButtonPressed += ViewOnMuteButtonPressed;
			view.OnVolumeButtonReleased += ViewOnVolumeButtonReleased;
			view.OnVolumeUpButtonPressed += ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed += ViewOnVolumeDownButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVolumeView view)
		{
			base.Unsubscribe(view);

			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
			view.OnMuteButtonPressed -= ViewOnMuteButtonPressed;
			view.OnVolumeButtonReleased -= ViewOnVolumeButtonReleased;
			view.OnVolumeUpButtonPressed -= ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed -= ViewOnVolumeDownButtonPressed;
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

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
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
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribe to the control events.
		/// </summary>
		/// <param name="control"></param>
		private void Subscribe(IVolumeDeviceControl control)
		{
			if (control == null)
				return;

			IVolumeMuteFeedbackDeviceControl controlMute = control as IVolumeMuteFeedbackDeviceControl;
			if (controlMute != null)
				controlMute.OnMuteStateChanged += DeviceOnMuteStateChanged;

			IVolumeLevelDeviceControl controlLevel = control as IVolumeLevelDeviceControl;
			if (controlLevel != null)
				controlLevel.OnVolumeChanged += DeviceOnVolumeChanged;

		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(IVolumeDeviceControl control)
		{
			if (control == null)
				return;

			IVolumeMuteFeedbackDeviceControl controlMute = control as IVolumeMuteFeedbackDeviceControl;
			if (controlMute != null)
				controlMute.OnMuteStateChanged -= DeviceOnMuteStateChanged;

			IVolumeLevelDeviceControl controlLevel = control as IVolumeLevelDeviceControl;
			if (controlLevel != null)
			{
				controlLevel.VolumeLevelRampStop();
				controlLevel.OnVolumeChanged -= DeviceOnVolumeChanged;
			}
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
		private void DeviceOnMuteStateChanged(object sender, BoolEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
