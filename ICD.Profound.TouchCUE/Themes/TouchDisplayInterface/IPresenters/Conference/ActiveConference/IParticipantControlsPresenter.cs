using ICD.Common.Properties;
using ICD.Connect.Conferencing.Participants;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.ActiveConference
{
	public interface IParticipantControlsPresenter : ITouchDisplayPresenter<IParticipantControlsView>
	{
		[CanBeNull]
		IWebParticipant Participant { get; set; }
	}
}