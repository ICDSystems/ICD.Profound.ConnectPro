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
		/// Sets the selected state of the mute all button.
		/// </summary>
		/// <param name="selected"></param>
		void SetMuteAllButtonSelected(bool selected);
	}
}
