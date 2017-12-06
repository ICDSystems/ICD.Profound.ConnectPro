using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface IStartMeetingView : IView
	{
		/// <summary>
		/// Raised when the user presses the start meeting button.
		/// </summary>
		event EventHandler OnStartMeetingButtonPressed;
	}
}
