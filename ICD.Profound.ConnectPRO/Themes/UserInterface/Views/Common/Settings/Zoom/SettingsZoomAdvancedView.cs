using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Zoom
{
	[ViewBinding(typeof(ISettingsZoomAdvancedView))]
	public sealed partial class SettingsZoomAdvancedView : AbstractUiView, ISettingsZoomAdvancedView
	{
		/// <summary>
		/// Raised when the user presses the audio processing button.
		/// </summary>
		public event EventHandler OnAudioProcessingButtonPressed;

		/// <summary>
		/// Raised when the user presses the audio reverb button.
		/// </summary>
		public event EventHandler OnAudioReverbButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsZoomAdvancedView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnAudioProcessingButtonPressed = null;
			OnAudioReverbButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the selected state of the audio processing button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetAudioProcessingButtonSelected(bool selected)
		{
			// Toggle button feedback is reversed :(
			m_AudioProcessingButton.SetSelected(!selected);
		}

		/// <summary>
		/// Sets the selected state of the audio reverb button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetAudioReverbButtonSelected(bool selected)
		{
			// Toggle button feedback is reversed :(
			m_AudioReverbButton.SetSelected(!selected);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_AudioReverbButton.OnPressed += AudioReverbButtonOnPressed;
			m_AudioProcessingButton.OnPressed += AudioProcessingButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_AudioReverbButton.OnPressed -= AudioReverbButtonOnPressed;
			m_AudioProcessingButton.OnPressed -= AudioProcessingButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the audio processing button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void AudioProcessingButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnAudioProcessingButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the audio reverb button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void AudioReverbButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnAudioReverbButtonPressed.Raise(this);
		}

		#endregion
	}
}
