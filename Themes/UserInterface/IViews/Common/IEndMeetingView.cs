using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface IEndMeetingView : IView
	{
		/// <summary>
		/// Raised when the user presses the end meeting button.
		/// </summary>
		event EventHandler OnEndMeetingButtonPressed;
	}
}
