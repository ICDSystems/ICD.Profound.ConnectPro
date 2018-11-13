using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference
{
	public interface IWtcCallOutView : IView
	{
		/// <summary>
		/// Raised when the text in the text entry changes.
		/// </summary>
		event EventHandler<StringEventArgs> OnTextEntered;

		/// <summary>
		/// Raised when the Call button is pressed.
		/// </summary>
		event EventHandler OnCallButtonPressed;

		/// <summary>
		/// Raised then the Hangup button is pressed.
		/// </summary>
		event EventHandler OnHangupButtonPressed;

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
		/// Sets the enabled state of the Call button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetCallButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the Hangup button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetHangupButtonEnabled(bool enabled);
	}
}