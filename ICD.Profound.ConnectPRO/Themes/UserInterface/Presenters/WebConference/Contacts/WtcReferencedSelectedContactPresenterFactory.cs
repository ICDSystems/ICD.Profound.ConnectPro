using System;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	public sealed class WtcReferencedSelectedContactPresenterFactory :
		AbstractUiListItemFactory<IContact, IWtcReferencedSelectedContactPresenter, IWtcReferencedSelectedContactView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public WtcReferencedSelectedContactPresenterFactory(IConnectProNavigationController navigationController,
		                                                    ListItemFactory<IWtcReferencedSelectedContactView> viewFactory,
		                                                    Action<IWtcReferencedSelectedContactPresenter> subscribe,
		                                                    Action<IWtcReferencedSelectedContactPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IContact model, IWtcReferencedSelectedContactPresenter presenter,
		                                     IWtcReferencedSelectedContactView view)
		{
			presenter.Contact = model;
			presenter.SetView(view);
		}
	}
}
