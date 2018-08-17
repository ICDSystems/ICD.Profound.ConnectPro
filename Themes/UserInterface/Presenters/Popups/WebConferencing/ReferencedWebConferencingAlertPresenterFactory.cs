using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.WebConferencing
{
	public sealed class ReferencedWebConferencingAlertPresenterFactory :
		AbstractListItemFactory<WebConferencingAppInstructions, IReferencedWebConferencingAlertPresenter, IReferencedWebConferencingAlertView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public ReferencedWebConferencingAlertPresenterFactory(INavigationController navigationController,
		                                                      ListItemFactory<IReferencedWebConferencingAlertView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(WebConferencingAppInstructions model, IReferencedWebConferencingAlertPresenter presenter,
		                                     IReferencedWebConferencingAlertView view)
		{
			presenter.App = model;
			presenter.SetView(view);
		}
	}
}