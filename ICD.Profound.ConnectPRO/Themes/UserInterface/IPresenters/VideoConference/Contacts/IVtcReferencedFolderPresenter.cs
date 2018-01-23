using ICD.Connect.Conferencing.Cisco.Components.Directory.Tree;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcReferencedFolderPresenter : IVtcReferencedContactsPresenterBase
	{
		IFolder Folder { get; set; }
	}
}
