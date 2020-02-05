using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views
{
	[ViewBinding(typeof(IVolumeView))]
	public sealed partial class VolumeView : AbstractTouchDisplayView, IVolumeView
	{
		public event EventHandler OnMuteButtonPressed;
		public event EventHandler OnVolumeUpButtonPressed;
		public event EventHandler OnVolumeDownButtonPressed;
		public event EventHandler OnVolumeButtonReleased;
		public event EventHandler<UShortEventArgs> OnVolumeGaugePressed;

		public VolumeView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public void SetMuteButtonVisible(bool visible)
		{
			m_VolumeMuteButton.Show(visible);
		}

		public void SetMuted(bool muted)
		{
			m_VolumeMuteButton.SetSelected(muted);
		}

		public void SetVolumePercentage(float volume)
		{
			m_VolumeGauge.SetValuePercentage(volume);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_VolumeUpButton.OnPressed += VolumeUpButtonOnPressed;
			m_VolumeUpButton.OnReleased += VolumeUpButtonOnReleased;
			m_VolumeDownButton.OnPressed += VolumeDownButtonOnPressed;
			m_VolumeDownButton.OnReleased += VolumeDownButtonOnReleased;
			m_VolumeMuteButton.OnPressed += MuteButtonOnPressed;
			m_VolumeGauge.OnTouched += VolumeGaugeOnTouched;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_VolumeUpButton.OnPressed -= VolumeUpButtonOnPressed;
			m_VolumeUpButton.OnReleased -= VolumeUpButtonOnReleased;
			m_VolumeDownButton.OnPressed -= VolumeDownButtonOnPressed;
			m_VolumeDownButton.OnReleased -= VolumeDownButtonOnReleased;
			m_VolumeMuteButton.OnPressed -= MuteButtonOnPressed;
			m_VolumeGauge.OnTouched -= VolumeGaugeOnTouched;
		}

		/// <summary>
		/// Called when the user touches the volume gauge
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VolumeGaugeOnTouched(object sender, UShortEventArgs e)
		{
			OnVolumeGaugePressed.Raise(this, new UShortEventArgs(e.Data));
		}

		/// <summary>
		/// Called when the user presses the mute button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void MuteButtonOnPressed(object sender, EventArgs args)
		{
			OnMuteButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user releases the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeDownButtonOnReleased(object sender, EventArgs args)
		{
			OnVolumeButtonReleased.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeDownButtonOnPressed(object sender, EventArgs args)
		{
			OnVolumeDownButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user releases the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeUpButtonOnReleased(object sender, EventArgs args)
		{
			OnVolumeButtonReleased.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeUpButtonOnPressed(object sender, EventArgs args)
		{
			OnVolumeUpButtonPressed.Raise(this);
		}

		#endregion
	}
}
