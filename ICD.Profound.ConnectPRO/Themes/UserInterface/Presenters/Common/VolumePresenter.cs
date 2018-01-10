using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Timers;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class VolumePresenter : AbstractPresenter<IVolumeView>, IVolumePresenter
	{
		private const ushort INITIAL_INCREMENT = 1;
		private const ushort REPEAT_INCREMENT = 1;
		private const ushort BEFORE_REPEAT_MILLISECONDS = 500;
		private const ushort BETWEEN_REPEAT_MILLISECONDS = 100;

		private const ushort HIDE_TIME = (ushort)(3.5f * 1000);

		private readonly VolumeRepeater m_Repeater;
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

				m_Repeater.Release();
				m_Repeater.SetControl(m_VolumeControl);

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
			m_Repeater = new VolumeRepeater(INITIAL_INCREMENT, REPEAT_INCREMENT, BEFORE_REPEAT_MILLISECONDS,
			                                BETWEEN_REPEAT_MILLISECONDS);

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

			bool muted = VolumeControl != null && VolumeControl.IsMuted;
			float volume = VolumeControl == null ? 0 : VolumeControl.GetRawVolumeAsSafetyPercentage();

			view.SetMuted(muted);
			view.SetVolumePercentage(volume);
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
			m_Repeater.VolumeUpHold();
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
			m_Repeater.VolumeDownHold();
		}

		/// <summary>
		/// Stops ramping the device volume.
		/// </summary>
		public void Release()
		{
			if (VolumeControl == null)
				return;

			ResetVisibilityTimer();
			m_Repeater.Release();
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
			VolumeControl.MuteToggle();
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

			control.OnMuteStateChanged += DeviceOnMuteStateChanged;
			control.OnRawVolumeChanged += DeviceOnVolumeChanged;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(IVolumeDeviceControl control)
		{
			if (control == null)
				return;

			control.OnMuteStateChanged -= DeviceOnMuteStateChanged;
			control.OnRawVolumeChanged -= DeviceOnVolumeChanged;
		}

		/// <summary>
		/// Called when the control volume changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DeviceOnVolumeChanged(object sender, FloatEventArgs args)
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
