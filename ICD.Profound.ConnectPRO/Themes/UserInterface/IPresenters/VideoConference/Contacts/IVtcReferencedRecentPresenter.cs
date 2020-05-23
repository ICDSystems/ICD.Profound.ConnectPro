using ICD.Connect.Conferencing.ConferenceManagers.History;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcReferencedRecentPresenter : IVtcReferencedContactsPresenterBase
	{
		IHistoricalParticipant Recent { get; set; }
	}
}
