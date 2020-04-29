using System;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Notifications
{
	public interface IConfirmEndMeetingView : ITouchDisplayView
	{
		/// <summary>
		/// Raised when the user presses the Yes button.
		/// </summary>
		event EventHandler OnYesButtonPressed;

		/// <summary>
		/// Raised when the user presses the Cancel button.
		/// </summary>
		event EventHandler OnCancelButtonPressed;

		void SetConfirmText(string text);
	}
}
