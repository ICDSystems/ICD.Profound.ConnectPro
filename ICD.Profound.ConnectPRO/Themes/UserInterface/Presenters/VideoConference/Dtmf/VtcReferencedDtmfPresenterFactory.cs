using System;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Dtmf;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Dtmf
{
	public sealed class VtcReferencedDtmfPresenterFactory :
		AbstractUiListItemFactory<IConferenceSource, IVtcReferencedDtmfPresenter, IVtcReferencedDtmfView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public VtcReferencedDtmfPresenterFactory(IConnectProNavigationController navigationController,
		                                         ListItemFactory<IVtcReferencedDtmfView> viewFactory,
		                                         Action<IVtcReferencedDtmfPresenter> subscribe,
		                                         Action<IVtcReferencedDtmfPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
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
