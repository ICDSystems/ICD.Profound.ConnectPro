using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference
{
	public interface IWtcCallOutView : IUiView
	{
		/// <summary>
		/// Raised when the Call button is pressed.
		/// </summary>
		event EventHandler OnCallButtonPressed;

		/// <summary>
		/// Raised when the clear button is pressed.
		/// </summary>
		event EventHandler OnClearButtonPressed;

		/// <summary>
		/// Raised when the back button is pressed.
		/// </summary>
		event EventHandler OnBackButtonPressed;

		/// <summary>
		/// Raised when a keypad button is pressed.
		/// </summary>
		event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		/// <summary>
		/// Sets the text value of the text entry box.
		/// </summary>
		/// <param name="text"></param>
		void SetText(string text);

		/// <summary>
		/// Sets the enabled state of the clear button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetClearButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the back button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetBackButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the selected state of the call button;
		/// </summary>
		/// <param name="selected"></param>
		void SetCallButtonSelected(bool selected);

		/// <summary>
		/// Sets the label text for the call button.
		/// </summary>
		/// <param name="label"></param>
		void SetCallButtonLabel(string label);

		/// <summary>
		/// Sets the label text for the call status
		/// </summary>
		/// <param name="label"></param>
		void SetCallStatusLabel(string label);
	}
}