using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface ITouchFreeCancelPromptView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the Cancel Metting Start button.
		/// </summary>
		event EventHandler OnCancelMeetingStartPressed;

		/// <summary>
		/// Raised when the user presses the Start Meeting Now button.
		/// </summary>
		event EventHandler OnStartMeetingNowPressed;

		void SetTimer(int seconds);
	}
}