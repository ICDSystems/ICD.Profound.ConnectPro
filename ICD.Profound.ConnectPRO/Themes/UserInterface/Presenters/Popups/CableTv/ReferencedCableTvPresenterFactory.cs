using ICD.Connect.TvPresets;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.CableTv
{
	public sealed class ReferencedCableTvPresenterFactory :
		AbstractListItemFactory<Station, IReferencedCableTvPresenter, IReferencedCableTvView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public ReferencedCableTvPresenterFactory(INavigationController navigationController,
													  ListItemFactory<IReferencedCableTvView> viewFactory)
			: base(navigationController, viewFactory)
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
