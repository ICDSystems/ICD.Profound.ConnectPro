using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcKeypadView : IVtcBaseKeyboardView
	{
		/// <summary>
		/// Raised when the user presses the exit button.
		/// </summary>
		event EventHandler OnKeyboardButtonPressed;
	}
}
