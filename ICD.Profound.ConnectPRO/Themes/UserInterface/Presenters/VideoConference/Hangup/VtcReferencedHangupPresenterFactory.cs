using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Hangup;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Hangup
{
	public sealed class VtcReferencedHangupPresenterFactory :
		AbstractListItemFactory<IConferenceSource, IVtcReferencedHangupPresenter, IVtcReferencedHangupView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public VtcReferencedHangupPresenterFactory(INavigationController navigationController,
		                                        ListItemFactory<IVtcReferencedHangupView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IConferenceSource model, IVtcReferencedHangupPresenter presenter,
		                                     IVtcReferencedHangupView view)
		{
			presenter.SetView(view);
			presenter.ConferenceSource = model;
		}
	}
}
