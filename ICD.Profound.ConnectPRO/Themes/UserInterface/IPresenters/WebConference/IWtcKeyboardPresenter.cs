using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference
{
	public interface IWtcKeyboardPresenter : IUiPresenter<IWtcKeyboardView>
	{
		/// <summary>
		/// Raised when the entered string changes at all.
		/// </summary>
		event EventHandler<StringEventArgs> OnStringChanged;

		/// <summary>
		/// Raised when the user presses the enter button. EventArgs contains the final string.
		/// </summary>
		event EventHandler<StringEventArgs> OnEnterPressed;

		/// <summary>
		/// Raised when the user presses the close button.
		/// </summary>
		event EventHandler OnClosePressed;

		/// <summary>
		/// Opens the keyboard and defaults the text to the given string.
		/// </summary>
		/// <param name="text"></param>
		void Show(string text);
	}
}
