using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Dtmf;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Dtmf
{
	public sealed class VtcReferencedDtmfPresenterFactory :
		AbstractListItemFactory<IConferenceSource, IVtcReferencedDtmfPresenter, IVtcReferencedDtmfView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public VtcReferencedDtmfPresenterFactory(IConnectProNavigationController navigationController,
		                                         ListItemFactory<IVtcReferencedDtmfView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IConferenceSource model, IVtcReferencedDtmfPresenter presenter,
											 IVtcReferencedDtmfView view)
		{
			presenter.SetView(view);
			presenter.Source = model;
		}
	}
}
