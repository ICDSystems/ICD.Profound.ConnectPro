using System;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class ReferencedDisplayPresenterFactory : AbstractUiListItemFactory<MenuDisplaysPresenterDisplay, IReferencedDisplayPresenter, IReferencedDisplayView>
	{
		public ReferencedDisplayPresenterFactory(IConnectProNavigationController navigationController, ListItemFactory<IReferencedDisplayView> viewFactory, Action<IReferencedDisplayPresenter> subscribe, Action<IReferencedDisplayPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(MenuDisplaysPresenterDisplay model, IReferencedDisplayPresenter presenter, IReferencedDisplayView view)
		{
			presenter.SetView(view);
			presenter.Model = model;
		}
	}

	
}
