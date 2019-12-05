using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.EventArguments;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	[PresenterBinding(typeof(IVolumePresenter))]
	public sealed class VolumePresenter : AbstractUiPresenter<IVolumeView>, IVolumePresenter
	{
		private const ushort HIDE_TIME = 20 * 1000;
		private const float RAMP_PERCENTAGE = 3.0f / 100.0f;
		private const long RAMP_INTERVAL = 300;
		private const long RAMP_TIMEOUT = long.MaxValue;

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
		public VolumePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_VisibilityTimer = SafeTimer.Stopped(() => ShowView(false));
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_VisibilityTimer.Dispose();

			base.Dispose();

			VolumeControl = null;
		}

		#region Methods

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
				bool muted = VolumeControl != null && VolumeControl.IsMuted;
				float volume = VolumeControl == null ? 0 : VolumeControl.GetVolumePercent();

				view.SetMuted(muted);
				view.SetVolumePercentage(volume);
				view.SetControlsEnabled(m_VolumeControl == null || m_VolumeControl.ControlAvailable);
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

			if (VolumeControl.IsMuted)
				VolumeControl.SetIsMuted(false);

			if (VolumeControl.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment))
				VolumeControl.VolumeRampPercent(RAMP_PERCENTAGE, RAMP_INTERVAL, RAMP_TIMEOUT);
			else if (VolumeControl.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Volume))
				VolumeControl.VolumeRamp(true, RAMP_INTERVAL, RAMP_TIMEOUT);
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

			if (VolumeControl.IsMuted)
				VolumeControl.SetIsMuted(false);

			if (VolumeControl.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment))
				VolumeControl.VolumeRampPercent(RAMP_PERCENTAGE * -1, RAMP_INTERVAL, RAMP_TIMEOUT);
			else if (VolumeControl.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Volume))
				VolumeControl.VolumeRamp(false, RAMP_INTERVAL, RAMP_TIMEOUT);
		}

		/// <summary>
		/// Stops ramping the device volume.
		/// </summary>
		public void Release()
		{
			if (VolumeControl == null)
				return;

			ResetVisibilityTimer();

			if (VolumeControl.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Volume))
				VolumeControl.VolumeRampStop();
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

			if (VolumeControl.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Mute))
				VolumeControl.ToggleIsMuted();
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
		/// <param name="volumeControl"></param>
		private void Subscribe(IVolumeDeviceControl volumeControl)
		{
			if (volumeControl == null)
				return;

			volumeControl.OnControlAvailableChanged += ControlOnControlAvailableChanged;
			volumeControl.OnIsMutedChanged += ControlOnIsMutedChanged;
			volumeControl.OnVolumeChanged += ControlOnVolumeChanged;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="volumeControl"></param>
		private void Unsubscribe(IVolumeDeviceControl volumeControl)
		{
			if (volumeControl == null)
				return;

			volumeControl.OnControlAvailableChanged -= ControlOnControlAvailableChanged;
			volumeControl.OnIsMutedChanged -= ControlOnIsMutedChanged;
			volumeControl.OnVolumeChanged -= ControlOnVolumeChanged;

			if (volumeControl.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Volume))
				volumeControl.VolumeRampStop();
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
		private void ControlOnVolumeChanged(object sender, EventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the control mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ControlOnIsMutedChanged(object sender, VolumeControlIsMutedChangedApiEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
