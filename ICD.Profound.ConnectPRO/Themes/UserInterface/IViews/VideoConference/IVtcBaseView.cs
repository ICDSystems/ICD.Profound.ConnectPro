using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcBaseView : IPopupView
	{
		/// <summary>
		/// Raised when the user presses the contacts button.
		/// </summary>
		event EventHandler<UShortEventArgs> OnNavButtonPressed;
	}
}
