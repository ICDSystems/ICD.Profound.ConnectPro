using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews
{
	public interface IDisabledAlertView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the dismiss button.
		/// </summary>
		event EventHandler OnDismissButtonPressed;
	}
}
