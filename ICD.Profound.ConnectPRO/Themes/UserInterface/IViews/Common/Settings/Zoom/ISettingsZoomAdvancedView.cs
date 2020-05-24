using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom
{
	public interface ISettingsZoomAdvancedView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the audio processing button.
		/// </summary>
		event EventHandler OnAudioProcessingButtonPressed;

		/// <summary>
		/// Raised when the user presses the audio reverb button.
		/// </summary>
		event EventHandler OnAudioReverbButtonPressed;

		/// <summary>
		/// Sets the selected state of the audio processing button.
		/// </summary>
		/// <param name="selected"></param>
		void SetAudioProcessingButtonSelected(bool selected);

		/// <summary>
		/// Sets the selected state of the audio reverb button.
		/// </summary>
		/// <param name="selected"></param>
		void SetAudioReverbButtonSelected(bool selected);
	}
}
