using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.AudioConference
{
	public interface IAtcBaseView : IPopupView
	{
		/// <summary>
		/// Raised when the user presses the dial button.
		/// </summary>
		event EventHandler OnDialButtonPressed;

		/// <summary>
		/// Raised when the user presses the hangup button.
		/// </summary>
		event EventHandler OnHangupButtonPressed;

		/// <summary>
		/// Raised when the user presses the clear button.
		/// </summary>
		event EventHandler OnClearButtonPressed;

		/// <summary>
		/// Raised when the user presses a keypad button.
		/// </summary>
		event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		/// <summary>
		/// Sets the text for the number being dialed.
		/// </summary>
		/// <param name="number"></param>
		void SetDialNumber(string number);

		/// <summary>
		/// Sets the text for the number actively connected to.
		/// </summary>
		/// <param name="number"></param>
		void SetActiveNumber(string number);

		/// <summary>
		/// Sets the enabled state of the dial button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetDialButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the hangup button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetHangupButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the clear button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetClearButtonEnabled(bool enabled);
	}
}
