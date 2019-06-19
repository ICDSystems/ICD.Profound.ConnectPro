﻿using System;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	public class WtcReferencedSelectedContactPresenterFactory : AbstractUiListItemFactory<IContact, IWtcReferencedSelectedContactPresenter, IWtcReferencedSelectedContactView>
	{
		public WtcReferencedSelectedContactPresenterFactory(IConnectProNavigationController navigationController, ListItemFactory<IWtcReferencedSelectedContactView> viewFactory, Action<IWtcReferencedSelectedContactPresenter> subscribe, Action<IWtcReferencedSelectedContactPresenter> unsubscribe) : base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(IContact model, IWtcReferencedSelectedContactPresenter presenter,
		                                     IWtcReferencedSelectedContactView view)
		{
			presenter.Contact = model;
			presenter.SetView(view);
		}
	}
}