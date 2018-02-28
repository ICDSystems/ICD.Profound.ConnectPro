using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcIncomingCallPresenter : IPresenter<IVtcIncomingCallView>
	{
		event EventHandler OnCallAnswered;
	}
}
