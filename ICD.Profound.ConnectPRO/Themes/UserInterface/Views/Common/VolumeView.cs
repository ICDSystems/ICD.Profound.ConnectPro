using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class VolumeView : AbstractView, IVolumeView
	{
		public event EventHandler OnVolumeUpButtonPressed;
		public event EventHandler OnVolumeDownButtonPressed;
		public event EventHandler OnVolumeButtonReleased;
		public event EventHandler OnMuteButtonPressed;
		public event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VolumeView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnVolumeUpButtonPressed = null;
			OnVolumeDownButtonPressed = null;
			OnVolumeButtonReleased = null;
			OnMuteButtonPressed = null;
			OnCloseButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the value on the volume bar in the range 0.0f - 1.0f
		/// </summary>
		/// <param name="volume"></param>
		public void SetVolumePercentage(float volume)
		{
			m_Guage.SetValuePercentage(volume);
		}

		/// <summary>
		/// Sets the enabled state of the volume guage.
		/// </summary>
		/// <param name="muted"></param>
		public void SetMuted(bool muted)
		{
			m_Guage.Enable(!muted);
			m_MuteButton.SetSelected(muted);
		}

		#endregion

		#region Private Methods

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
			m_MuteButton.OnPressed += MuteButtonOnPressed;
			m_CloseButton.OnPressed += CloseButtonOnPressed;
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
			m_MuteButton.OnPressed -= MuteButtonOnPressed;
			m_CloseButton.OnPressed -= CloseButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
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
