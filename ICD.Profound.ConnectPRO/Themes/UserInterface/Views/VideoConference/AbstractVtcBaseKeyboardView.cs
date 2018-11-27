using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Panels;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public abstract class AbstractVtcBaseKeyboardView : AbstractUiView, IVtcBaseKeyboardView
	{
		/// <summary>
		/// Raised when the user presses a key button.
		/// </summary>
		public abstract event EventHandler<KeyboardKeyEventArgs> OnKeyPressed;

		/// <summary>
		/// Raised when the user enters text in the text field.
		/// </summary>
		public abstract event EventHandler<StringEventArgs> OnTextEntered;

		/// <summary>
		/// Raised when the user presses the backspace button.
		/// </summary>
		public abstract event EventHandler OnBackspaceButtonPressed;

		/// <summary>
		/// Raised when the user presses the submit button.
		/// </summary>
		public abstract event EventHandler OnDialButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractVtcBaseKeyboardView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Sets the text in the text entry field.
		/// </summary>
		/// <param name="text"></param>
		public abstract void SetText(string text);
	}
}
