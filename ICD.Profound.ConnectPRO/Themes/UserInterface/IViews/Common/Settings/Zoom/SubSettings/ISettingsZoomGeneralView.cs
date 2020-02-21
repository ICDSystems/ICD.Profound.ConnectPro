using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom.SubSettings
{
	public interface ISettingsZoomGeneralView : ISettingsZoomSubView
	{
		/// <summary>
		/// Raised when the user presses the mute all participants button.
		/// </summary>
		event EventHandler OnMuteAllParticipantsButtonPressed;

		/// <summary>
		/// Raised when the user presses the mute my camera on call start button.
		/// </summary>
		event EventHandler OnMuteMyCameraButtonPressed;

		/// <summary>
		/// Sets the selected state of the mute all participants button.
		/// </summary>
		/// <param name="selected"></param>
		void SetMuteAllButtonSelected(bool selected);

		/// <summary>
		/// Sets the selected state of the mute my camera on call start button.
		/// </summary>
		/// <param name="selected"></param>
		void SetMuteMyCameraButtonSelected(bool selected);
	}
}
