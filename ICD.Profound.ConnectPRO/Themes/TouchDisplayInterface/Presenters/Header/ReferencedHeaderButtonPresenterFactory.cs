﻿using System;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Header
{
	public sealed class ReferencedHeaderButtonPresenterFactory : AbstractTouchDisplayListItemFactory<HeaderButtonModel, IReferencedHeaderButtonPresenter, IReferencedHeaderButtonView>
	{
		public ReferencedHeaderButtonPresenterFactory(ITouchDisplayNavigationController navigationController, ListItemFactory<IReferencedHeaderButtonView> viewFactory, Action<IReferencedHeaderButtonPresenter> subscribe, Action<IReferencedHeaderButtonPresenter> unsubscribe) : base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(HeaderButtonModel model, IReferencedHeaderButtonPresenter presenter,
			IReferencedHeaderButtonView view)
		{
			presenter.Model = model;
			presenter.SetView(view);
		}
	}
}
