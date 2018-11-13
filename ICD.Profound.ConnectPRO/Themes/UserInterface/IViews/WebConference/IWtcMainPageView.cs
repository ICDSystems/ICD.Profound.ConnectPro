using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference
{
	public interface IWtcMainPageView : IView
	{
		/// <summary>
		/// Raised when the user presses the Meet Now button.
		/// </summary>
		event EventHandler OnMeetNowButtonPressed;

		/// <summary>
		/// Raised when the user presses the Contacts button.
		/// </summary>
		event EventHandler OnContactsButtonPressed;

		/// <summary>
		/// Raised when the user presses the Join By Id button.
		/// </summary>
		event EventHandler OnJoinByIdButtonPressed;

		/// <summary>
		/// Sets the enabled state of the Meet Now button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetMeetNowButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the Join By Id button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetJoinByIdButtonEnabled(bool enabled);
	}
}