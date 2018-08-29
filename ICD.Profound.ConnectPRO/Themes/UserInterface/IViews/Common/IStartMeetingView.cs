using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface IStartMeetingView : IView
	{
		/// <summary>
		/// Raised when the user presses the start meeting button.
		/// </summary>
		event EventHandler OnStartMeetingButtonPressed;

		/// <summary>
		/// Raised when the user presses the settings button.
		/// </summary>
		event EventHandler OnSettingsButtonPressed;

		/// <summary>
		/// Sets the enabled state of the start meeting button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetStartMeetingButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the image path for the logo.
		/// </summary>
		/// <param name="url"></param>
		void SetLogoPath(string url);
	}
}
