using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcBaseView : IPopupView
	{
		/// <summary>
		/// Raised when the user presses the contacts button.
		/// </summary>
		event EventHandler OnContactsButtonPressed;

		/// <summary>
		/// Raised when the user presses the camera button.
		/// </summary>
		event EventHandler OnCameraButtonPressed;

		/// <summary>
		/// Raised when the user presses the share button.
		/// </summary>
		event EventHandler OnShareButtonPressed;

		/// <summary>
		/// Raised when the user presses the DTMF button.
		/// </summary>
		event EventHandler OnDtmfButtonPressed;
	}
}
