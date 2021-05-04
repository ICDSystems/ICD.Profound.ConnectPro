using ICD.Common.Properties;
using ICD.Connect.Conferencing.Participants;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting
{
	public interface IWtcParticipantControlsPresenter : IWtcPresenter<IWtcParticipantControlsView>
	{
		[CanBeNull]
		IParticipant Participant { get; set; }
	}
}