using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.ActiveCalls
{
	public sealed class VtcReferencedActiveCallsPresenterFactory :
		AbstractListItemFactory<IConferenceSource, IVtcReferencedActiveCallsPresenter, IVtcReferencedActiveCallsView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public VtcReferencedActiveCallsPresenterFactory(IConnectProNavigationController navigationController,
		                                        ListItemFactory<IVtcReferencedActiveCallsView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IConferenceSource model, IVtcReferencedActiveCallsPresenter presenter,
		                                     IVtcReferencedActiveCallsView view)
		{
			presenter.SetView(view);
			presenter.ConferenceSource = model;
		}
	}
}
