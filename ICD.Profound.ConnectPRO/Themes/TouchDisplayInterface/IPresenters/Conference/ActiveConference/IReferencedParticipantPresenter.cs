using System;
using ICD.Connect.Conferencing.Participants;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.ActiveConference
{
	public interface IReferencedParticipantPresenter : ITouchDisplayPresenter<IReferencedParticipantView>
	{
		event EventHandler OnPressed;

		IParticipant Participant { get; set; }

		bool Selected { get; set; }
	}
}