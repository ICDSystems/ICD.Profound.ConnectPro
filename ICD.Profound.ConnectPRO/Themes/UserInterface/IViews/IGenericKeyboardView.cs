using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews
{
	public interface IGenericKeyboardView : IUiView
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
		/// Raised when the user presses the space button.
		/// </summary>
		event EventHandler OnSpaceButtonPressed;

		/// <summary>
		/// Raised when the user presses the caps button.
		/// </summary>
		event EventHandler OnCapsButtonPressed;

		/// <summary>
		/// Raised when the user presses the shift button.
		/// </summary>
		event EventHandler OnShiftButtonPressed;

		/// <summary>
		/// Raised when the user presses the enter button.
		/// </summary>
		event EventHandler OnEnterButtonPressed;

		/// <summary>
		/// Raised when the user presses the close button.
		/// </summary>
		event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Sets the prompt above the text box.
		/// </summary>
		/// <param name="prompt"></param>
		void SetPrompt(string prompt);

		/// <summary>
		/// Sets the text in the text entry field.
		/// </summary>
		/// <param name="text"></param>
		void SetText(string text);

		/// <summary>
		/// Sets the selected state of the caps button.
		/// </summary>
		void SelectCapsButton(bool select);

		/// <summary>
		/// Sets the selected state of the shift button.
		/// </summary>
		void SelectShiftButton(bool select);

		/// <summary>
		/// Sets the shift state of the chars.
		/// </summary>
		/// <param name="shift"></param>
		/// <param name="caps"></param>
		void SetShift(bool shift, bool caps);
	}
}
