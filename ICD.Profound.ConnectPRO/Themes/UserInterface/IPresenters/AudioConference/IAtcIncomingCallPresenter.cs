using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.AudioConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference
{
	public interface IAtcIncomingCallPresenter : IPresenter<IAtcIncomingCallView>
	{
		/// <summary>
		/// Raised when the user answers the call.
		/// </summary>
		event EventHandler OnCallAnswered;
	}
}
