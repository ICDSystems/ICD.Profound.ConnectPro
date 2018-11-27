using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcBaseKeyboardView : IUiView
	{
		/// <summary>
		/// Raised when the user presses a key button.
		/// </summary>
		event EventHandler<KeyboardKeyEventArgs> OnKeyPressed;

		/// <summary>
		/// Raised when the user enters text in the text field.
		/// </summary>
		event EventHandler<StringEventArgs> OnTextEntered;

		/// <summary>
		/// Raised when the user presses the backspace button.
		/// </summary>
		event EventHandler OnBackspaceButtonPressed;

		/// <summary>
		/// Raised when the user presses the submit button.
		/// </summary>
		event EventHandler OnDialButtonPressed;

		/// <summary>
		/// Sets the text in the text entry field.
		/// </summary>
		/// <param name="text"></param>
		void SetText(string text);
	}
}
