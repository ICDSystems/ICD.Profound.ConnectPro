using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	public sealed partial class HardButtonsView : AbstractUiView, IHardButtonsView
	{
		public event EventHandler OnPowerButtonPressed;
		public event EventHandler OnHomeButtonPressed;
		public event EventHandler OnLightsButtonPressed;
		public event EventHandler OnVolumeUpButtonPressed;
		public event EventHandler OnVolumeDownButtonPressed;
		public event EventHandler OnVolumeButtonReleased;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public HardButtonsView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPowerButtonPressed = null;
			OnHomeButtonPressed = null;
			OnLightsButtonPressed = null;
			OnVolumeUpButtonPressed = null;
			OnVolumeDownButtonPressed = null;
			OnVolumeButtonReleased = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the visibility of the view.
		/// </summary>
		/// <param name="visible"></param>
		public override void Show(bool visible)
		{
			// Always visible
		}

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_HomeButton.OnPressed += HomeButtonOnPressed;
			m_LightButton.OnPressed += LightButtonOnPressed;
			m_PowerButton.OnPressed += PowerButtonOnPressed;
			m_VolumeUpButton.OnPressed += VolumeUpButtonOnPressed;
			m_VolumeUpButton.OnReleased += VolumeUpButtonOnReleased;
			m_VolumeDownButton.OnPressed += VolumeDownButtonOnPressed;
			m_VolumeDownButton.OnReleased += VolumeDownButtonOnReleased;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_HomeButton.OnPressed -= HomeButtonOnPressed;
			m_LightButton.OnPressed -= LightButtonOnPressed;
			m_PowerButton.OnPressed -= PowerButtonOnPressed;
			m_VolumeUpButton.OnPressed -= VolumeUpButtonOnPressed;
			m_VolumeUpButton.OnReleased -= VolumeUpButtonOnReleased;
			m_VolumeDownButton.OnPressed -= VolumeDownButtonOnPressed;
			m_VolumeDownButton.OnReleased -= VolumeDownButtonOnReleased;
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
		/// Called when the user releases the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeUpButtonOnReleased(object sender, EventArgs args)
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
		/// Called when the user presses the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeUpButtonOnPressed(object sender, EventArgs args)
		{
			OnVolumeUpButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the power button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PowerButtonOnPressed(object sender, EventArgs args)
		{
			OnPowerButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the light button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightButtonOnPressed(object sender, EventArgs args)
		{
			OnLightsButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the home button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void HomeButtonOnPressed(object sender, EventArgs args)
		{
			OnHomeButtonPressed.Raise(this);
		}

		#endregion
	}
}
