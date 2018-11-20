using System;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	public class WtcReferencedDirectoryItemPresenterFactory : AbstractUiListItemFactory<DirectoryItem, IWtcReferencedDirectoryItemPresenter, IWtcReferencedDirectoryItemView>
	{
		public WtcReferencedDirectoryItemPresenterFactory(IConnectProNavigationController navigationController, 
		                                            ListItemFactory<IWtcReferencedDirectoryItemView> viewFactory,
		                                            Action<IWtcReferencedDirectoryItemPresenter> subscribe,
		                                            Action<IWtcReferencedDirectoryItemPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(DirectoryItem model, IWtcReferencedDirectoryItemPresenter presenter, IWtcReferencedDirectoryItemView view)
		{
			presenter.SetView(view);
			presenter.DirectoryItem = model;
		}
	}
}