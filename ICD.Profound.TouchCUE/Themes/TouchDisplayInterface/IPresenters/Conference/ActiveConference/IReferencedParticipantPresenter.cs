using System;
using ICD.Connect.Conferencing.Participants;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.ActiveConference
{
	public interface IReferencedParticipantPresenter : ITouchDisplayPresenter<IReferencedParticipantView>
	{
		event EventHandler OnPressed;

		IParticipant Participant { get; set; }

		bool Selected { get; set; }
	}
}