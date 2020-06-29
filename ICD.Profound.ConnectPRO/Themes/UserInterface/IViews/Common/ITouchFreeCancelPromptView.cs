using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface ITouchFreeCancelPromptView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the Cancel Metting Start button.
		/// </summary>
		event EventHandler OnCancelMeetingStartPressed;

		void SetTimer(int seconds);
	}
}