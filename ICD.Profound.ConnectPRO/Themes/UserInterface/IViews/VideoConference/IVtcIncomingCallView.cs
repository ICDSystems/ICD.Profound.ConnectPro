using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcIncomingCallView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the answer button.
		/// </summary>
		event EventHandler OnAnswerButtonPressed;

		/// <summary>
		/// Raised when the user presses the ignore button.
		/// </summary>
		event EventHandler OnIgnoreButtonPressed;

		/// <summary>
		/// Sets the incoming caller number.
		/// </summary>
		/// <param name="number"></param>
		void SetCallerInfo(string number);
	}
}
