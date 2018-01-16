using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcDtmfView : IView
	{
		/// <summary>
		/// Raised when the user presses a dialtone button.
		/// </summary>
		event EventHandler<CharEventArgs> OnToneButtonPressed;
	}
}
