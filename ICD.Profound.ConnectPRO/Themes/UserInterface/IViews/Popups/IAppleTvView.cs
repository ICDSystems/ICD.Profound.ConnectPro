using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups
{
	public interface IAppleTvView : IPopupView
	{
		/// <summary>
		/// Raised when the user presses the up d-pad button.
		/// </summary>
		event EventHandler OnDPadUpButtonPressed;

		/// <summary>
		/// Raised when the user presses the down d-pad button.
		/// </summary>
		event EventHandler OnDPadDownButtonPressed;

		/// <summary>
		/// Raised when the user presses the left d-pad button.
		/// </summary>
		event EventHandler OnDPadLeftButtonPressed;

		/// <summary>
		/// Raised when the user presses the right d-pad button.
		/// </summary>
		event EventHandler OnDPadRightButtonPressed;

		/// <summary>
		/// Raised when the user releases a d-pad button.
		/// </summary>
		event EventHandler OnDPadButtonReleased;

		/// <summary>
		/// Raised when the user presses the menu button.
		/// </summary>
		event EventHandler OnMenuButtonPressed;

		/// <summary>
		/// Raised when the user presses the play button.
		/// </summary>
		event EventHandler OnPlayButtonPressed;
	}
}
