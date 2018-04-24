using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcIncomingCallPresenter : IPresenter<IVtcIncomingCallView>
	{
		/// <summary>
		/// Raised when the user answers the incoming call.
		/// </summary>
		event EventHandler OnCallAnswered;
	}
}
