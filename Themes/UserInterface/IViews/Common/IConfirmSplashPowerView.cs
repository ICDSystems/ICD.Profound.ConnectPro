using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface IConfirmSplashPowerView : IView
	{
		/// <summary>
		/// Raised when the user presses the Yes button.
		/// </summary>
		event EventHandler OnYesButtonPressed;

		/// <summary>
		/// Raised when the user presses the Cancel button.
		/// </summary>
		event EventHandler OnCancelButtonPressed;
	}
}
