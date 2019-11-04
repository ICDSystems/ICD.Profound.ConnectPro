using System;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu
{
	public sealed class WtcReferencedLeftMenuPresenterFactory :
		AbstractUiListItemFactory<IWtcLeftMenuButtonModel, IWtcReferencedLeftMenuPresenter, IWtcReferencedLeftMenuView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public WtcReferencedLeftMenuPresenterFactory(IConnectProNavigationController navigationController,
		                                             ListItemFactory<IWtcReferencedLeftMenuView> viewFactory,
		                                             Action<IWtcReferencedLeftMenuPresenter> subscribe,
		                                             Action<IWtcReferencedLeftMenuPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IWtcLeftMenuButtonModel model, IWtcReferencedLeftMenuPresenter presenter,
											 IWtcReferencedLeftMenuView view)
		{
			presenter.ButtonModel = model;
			presenter.SetView(view);
		}
	}
}
