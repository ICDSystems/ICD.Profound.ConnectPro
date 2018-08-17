using ICD.Connect.Conferencing.ConferenceSources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcReferencedRecentPresenter : IVtcReferencedContactsPresenterBase
	{
		IConferenceSource Recent { get; set; }
	}
}
