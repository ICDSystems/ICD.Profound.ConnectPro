using System;
using ICD.Connect.Sources.TvTuner.TvPresets;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.CableTv
{
	public sealed class ReferencedCableTvPresenterFactory :
		AbstractUiListItemFactory<Station, IReferencedCableTvPresenter, IReferencedCableTvView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public ReferencedCableTvPresenterFactory(IConnectProNavigationController navigationController,
		                                         ListItemFactory<IReferencedCableTvView> viewFactory,
		                                         Action<IReferencedCableTvPresenter> subscribe,
		                                         Action<IReferencedCableTvPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(Station model, IReferencedCableTvPresenter presenter,
		                                     IReferencedCableTvView view)
		{
			presenter.Station = model;
			presenter.SetView(view);
		}
	}
}
