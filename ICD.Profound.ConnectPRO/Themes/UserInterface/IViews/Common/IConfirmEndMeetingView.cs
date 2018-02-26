using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface IConfirmEndMeetingView : IView
	{
		/// <summary>
		/// Raised when the user presses the Yes button.
		/// </summary>
		event EventHandler OnYesButtonPressed;

		/// <summary>
		/// Raised when the user presses the Cancel button.
		/// </summary>
		event EventHandler OnCancelButtonPressed;

		/// <summary>
		/// Raised when the user presses the Shutdown button.
		/// </summary>
		event EventHandler OnShutdownButtonPressed;
	}
}
