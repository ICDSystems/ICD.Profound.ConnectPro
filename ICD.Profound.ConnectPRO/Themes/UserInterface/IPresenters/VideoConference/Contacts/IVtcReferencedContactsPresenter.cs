using ICD.Connect.Conferencing.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcReferencedContactsPresenter : IVtcReferencedContactsPresenterBase
	{
		IContact Contact { get; set; }
	}
}
