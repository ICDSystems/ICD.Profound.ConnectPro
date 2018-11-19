using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups
{
	public interface IPopupView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the close button.
		/// </summary>
		event EventHandler OnCloseButtonPressed;
	}
}
