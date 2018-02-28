﻿using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcIncomingCallPresenter : IPresenter<IVtcIncomingCallView>
	{
		void SetCallerInfo(string number);
	}
}
