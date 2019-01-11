using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference
{
	public interface IWtcStartMeetingView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the Meet Now button.
		/// </summary>
		event EventHandler OnMeetNowButtonPressed;

		/// <summary>
		/// Raised when the user presses the Join By Id button.
		/// </summary>
		event EventHandler OnJoinByIdButtonPressed;
		
		/// <summary>
		/// Raised when the user presses a digit button on the keypad.
		/// </summary>
		event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		/// <summary>
		/// Raised when the user presses the keypad Clear button.
		/// </summary>
		event EventHandler OnClearButtonPressed;

		/// <summary>
		/// Raised when the user presses the keypad Back button.
		/// </summary>
		event EventHandler OnBackButtonPressed;

		/// <summary>
		/// Raised when the user enters text into the Zoom Meeting Id text field.
		/// </summary>
		event EventHandler<StringEventArgs> OnTextEntered;

		/// <summary>
		/// Sets the enabled state of the Meet Now button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetMeetNowButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the Join By Id button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetJoinByIdButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the text in the Zoom Meeting Id text field.
		/// </summary>
		/// <param name="text"></param>
		void SetMeetingIdText(string text);
	}
}