using System;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class ReferencedAdvancedDisplayPresenterFactory : AbstractUiListItemFactory<MenuDisplaysPresenterDisplay, IReferencedAdvancedDisplayPresenter, IReferencedAdvancedDisplayView>
	{
		public ReferencedAdvancedDisplayPresenterFactory(IConnectProNavigationController navigationController, ListItemFactory<IReferencedAdvancedDisplayView> viewFactory, Action<IReferencedAdvancedDisplayPresenter> subscribe, Action<IReferencedAdvancedDisplayPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(MenuDisplaysPresenterDisplay model, IReferencedAdvancedDisplayPresenter presenter, IReferencedAdvancedDisplayView view)
		{
			presenter.SetView(view);
			presenter.Model = model;
		}
	}

	
}
