using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class ReferencedDisplaysPresenterFactory :
		AbstractListItemFactory<IDestination, IReferencedDisplaysPresenter, IReferencedDisplaysView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public ReferencedDisplaysPresenterFactory(INavigationController navigationController,
		                                          ListItemFactory<IReferencedDisplaysView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IDestination model, IReferencedDisplaysPresenter presenter,
											 IReferencedDisplaysView view)
		{
			presenter.SetView(view);
		}
	}
}
