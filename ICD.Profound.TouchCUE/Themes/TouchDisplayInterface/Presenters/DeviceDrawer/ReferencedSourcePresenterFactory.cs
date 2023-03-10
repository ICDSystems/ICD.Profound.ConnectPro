using System;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.DeviceDrawer
{
	public sealed class ReferencedSourcePresenterFactory : AbstractTouchDisplayListItemFactory<ISource, IReferencedSourcePresenter, IReferencedSourceView>
	{
		public ReferencedSourcePresenterFactory(ITouchDisplayNavigationController navigationController, ListItemFactory<IReferencedSourceView> viewFactory, Action<IReferencedSourcePresenter> subscribe, Action<IReferencedSourcePresenter> unsubscribe) : base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(ISource model, IReferencedSourcePresenter presenter, IReferencedSourceView view)
		{
			presenter.Source = model;
			presenter.SetView(view);
		}
	}
}
