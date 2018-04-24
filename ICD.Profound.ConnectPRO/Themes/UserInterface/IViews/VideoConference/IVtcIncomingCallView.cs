using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcIncomingCallView : IView
	{
		/// <summary>
		/// Raised when the user presses the answer button.
		/// </summary>
		event EventHandler OnAnswerButtonPressed;

		/// <summary>
		/// Raised when the user presses the ignore button.
		/// </summary>
		event EventHandler OnIgnoreButtonPressed;

		void SetCallerInfo(string number);
	}
}
