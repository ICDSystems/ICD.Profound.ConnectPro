using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.Administrative
{
	public interface ISettingsPinView : ITouchDisplayView
	{
		/// <summary>
		/// Raised when the user presses a keypad button.
		/// </summary>
		event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		/// <summary>
		/// Raised when the user presses the clear button.
		/// </summary>
		event EventHandler OnClearButtonPressed;

		/// <summary>
		/// Raised when the user presses the enter button.
		/// </summary>
		event EventHandler OnEnterButtonPressed;

		/// <summary>
		/// Sets the instructional text.
		/// </summary>
		/// <param name="label"></param>
		void SetInstructionLabel(string label);

		/// <summary>
		/// Sets the passcode text.
		/// </summary>
		/// <param name="label"></param>
		void SetPasscodeLabel(string label);
	}
}
