using System;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class ReferencedRouteListItemPresenterFactory : AbstractUiListItemFactory<RouteListItem, IReferencedRouteListItemPresenter, IReferencedRouteListItemView>
	{
		public ReferencedRouteListItemPresenterFactory(IConnectProNavigationController navigationController, ListItemFactory<IReferencedRouteListItemView> viewFactory, Action<IReferencedRouteListItemPresenter> subscribe, Action<IReferencedRouteListItemPresenter> unsubscribe) : base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(RouteListItem model, IReferencedRouteListItemPresenter presenter, IReferencedRouteListItemView view)
		{
			presenter.SetView(view);
			presenter.Model = model;
		}
	}
}
