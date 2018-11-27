using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcCallListToggleView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// When true shows the "contacts" button, otherwise shows the "call" button.
		/// </summary>
		/// <param name="mode"></param>
		void SetContactsMode(bool mode);

		/// <summary>
		/// Sets the visibility of the button and label.
		/// </summary>
		/// <param name="visible"></param>
		void SetButtonVisible(bool visible);
	}
}
