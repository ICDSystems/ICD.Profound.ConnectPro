using System;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings.Conferencing
{
	public interface ISettingsDirectoryView : ITouchDisplayView
	{
		/// <summary>
		/// Raised when the user presses the clear directory button.
		/// </summary>
		event EventHandler OnClearDirectoryButtonPressed;

		/// <summary>
		/// Sets the text for the help label.
		/// </summary>
		/// <param name="text"></param>
		void SetHelpText(string text);
	}
}
