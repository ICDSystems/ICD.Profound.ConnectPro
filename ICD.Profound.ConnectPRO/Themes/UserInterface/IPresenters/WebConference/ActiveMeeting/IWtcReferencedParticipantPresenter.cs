﻿using System;
using ICD.Connect.Conferencing.Participants;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting
{
	public interface IWtcReferencedParticipantPresenter : IUiPresenter<IWtcReferencedParticipantView>
	{
		event EventHandler OnPressed;

		IWebParticipant Participant { get; set; }

		bool Selected { get; set; }
	}
}