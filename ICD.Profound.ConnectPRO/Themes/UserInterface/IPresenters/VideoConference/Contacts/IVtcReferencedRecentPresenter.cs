using ICD.Connect.Conferencing.Participants;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcReferencedRecentPresenter : IVtcReferencedContactsPresenterBase
	{
		ITraditionalParticipant Recent { get; set; }
	}
}
