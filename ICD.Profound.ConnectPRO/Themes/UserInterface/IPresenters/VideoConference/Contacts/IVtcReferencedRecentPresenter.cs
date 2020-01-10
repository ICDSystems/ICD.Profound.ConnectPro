using ICD.Connect.Conferencing.ConferenceManagers.Recents;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcReferencedRecentPresenter : IVtcReferencedContactsPresenterBase
	{
		IRecentCall Recent { get; set; }
	}
}
