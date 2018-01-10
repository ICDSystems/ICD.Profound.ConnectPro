using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common
{
	public interface IConfirmPresenter : IPresenter<IConfirmView>
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
