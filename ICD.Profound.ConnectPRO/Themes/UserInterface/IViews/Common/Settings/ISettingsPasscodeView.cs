using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings
{
	public interface ISettingsPasscodeView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the cancel button.
		/// </summary>
		event EventHandler OnCancelButtonPressed;

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
		/// Sets the label text for the passcode.
		/// </summary>
		/// <param name="label"></param>
		void SetPasscodeLabel(string label);

		/// <summary>
		/// Sets the label text for the version.
		/// </summary>
		/// <param name="version"></param>
		void SetVersionLabel(string version);
	}
}
