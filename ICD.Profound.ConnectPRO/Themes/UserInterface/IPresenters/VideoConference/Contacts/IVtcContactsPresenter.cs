using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcContactsPresenter<TView> : IUiPresenter<TView>, IVtcPresenter
		where TView : IVtcContactsView
	{
	}
}
