using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcCallListToggleView : IView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnButtonPressed;

		void SetContactsMode(bool mode);
	}
}
