using System;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	public class WtcReferencedContactPresenterFactory : AbstractUiListItemFactory<IContact, IWtcReferencedContactPresenter, IWtcReferencedContactView>
	{
		public WtcReferencedContactPresenterFactory(IConnectProNavigationController navigationController, 
		                                            ListItemFactory<IWtcReferencedContactView> viewFactory,
		                                            Action<IWtcReferencedContactPresenter> subscribe,
		                                            Action<IWtcReferencedContactPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(IContact model, IWtcReferencedContactPresenter presenter, IWtcReferencedContactView view)
		{
			presenter.Contact = model;
			presenter.SetView(view);
		}
	}
}