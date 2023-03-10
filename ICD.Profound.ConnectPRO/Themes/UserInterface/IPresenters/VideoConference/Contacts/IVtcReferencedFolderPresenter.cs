using ICD.Connect.Conferencing.Directory.Tree;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcReferencedFolderPresenter : IVtcReferencedContactsPresenterBase
	{
		IDirectoryFolder Folder { get; set; }
	}
}
