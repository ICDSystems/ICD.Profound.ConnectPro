using ICD.Connect.Conferencing.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	public class WtcReferencedContactPresenterFactory : AbstractListItemFactory<IContact, IWtcReferencedContactPresenter, IWtcReferencedContactView>
	{
		public WtcReferencedContactPresenterFactory(INavigationController navigationController, ListItemFactory<IWtcReferencedContactView> viewFactory) : base(navigationController, viewFactory)
		{
		}

		protected override void BindMvpTriad(IContact model, IWtcReferencedContactPresenter presenter, IWtcReferencedContactView view)
		{
			presenter.SetView(view);
			presenter.Contact = model;
		}
	}
}