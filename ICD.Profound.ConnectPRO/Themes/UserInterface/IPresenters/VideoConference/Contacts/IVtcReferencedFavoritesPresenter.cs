using ICD.Connect.Conferencing.Favorites;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcReferencedFavoritesPresenter : IVtcReferencedContactsPresenterBase
	{
		Favorite Favorite { get; set; }
	}
}
