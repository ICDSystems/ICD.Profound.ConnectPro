using System;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Sources
{
	public sealed class ReferencedSourceSelectPresenterFactory :
		AbstractUiListItemFactory<ISource, IReferencedSourceSelectPresenter, IReferencedSourceSelectView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public ReferencedSourceSelectPresenterFactory(IConnectProNavigationController navigationController,
		                                              ListItemFactory<IReferencedSourceSelectView> viewFactory,
		                                              Action<IReferencedSourceSelectPresenter> subscribe,
		                                              Action<IReferencedSourceSelectPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(ISource model, IReferencedSourceSelectPresenter presenter,
		                                     IReferencedSourceSelectView view)
		{
			presenter.Source = model;
			presenter.SetView(view);
		}
	}
}
