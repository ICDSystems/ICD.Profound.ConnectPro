using System;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.WebConferencing
{
	public sealed class ReferencedWebConferencingAlertPresenterFactory :
		AbstractUiListItemFactory
			<WebConferencingAppInstructions, IReferencedWebConferencingAlertPresenter, IReferencedWebConferencingAlertView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public ReferencedWebConferencingAlertPresenterFactory(IConnectProNavigationController navigationController,
		                                                      ListItemFactory<IReferencedWebConferencingAlertView> viewFactory,
		                                                      Action<IReferencedWebConferencingAlertPresenter> subscribe,
		                                                      Action<IReferencedWebConferencingAlertPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(WebConferencingAppInstructions model,
		                                     IReferencedWebConferencingAlertPresenter presenter,
		                                     IReferencedWebConferencingAlertView view)
		{
			presenter.App = model;
			presenter.SetView(view);
		}
	}
}
