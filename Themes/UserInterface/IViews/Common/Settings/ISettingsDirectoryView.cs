using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings
{
	public interface ISettingsDirectoryView : IView
	{
		/// <summary>
		/// Raised when the user presses the clear directory button.
		/// </summary>
		event EventHandler OnClearDirectoryButtonPressed;
	}
}
