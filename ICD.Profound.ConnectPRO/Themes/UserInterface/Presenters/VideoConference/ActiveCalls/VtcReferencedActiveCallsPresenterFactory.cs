using System;
﻿using ICD.Connect.Conferencing.Participants;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.ActiveCalls
{
	public sealed class VtcReferencedActiveCallsPresenterFactory :
		AbstractUiListItemFactory<IParticipant, IVtcReferencedActiveCallsPresenter, IVtcReferencedActiveCallsView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public VtcReferencedActiveCallsPresenterFactory(IConnectProNavigationController navigationController,
		                                                ListItemFactory<IVtcReferencedActiveCallsView> viewFactory,
		                                                Action<IVtcReferencedActiveCallsPresenter> subscribe,
		                                                Action<IVtcReferencedActiveCallsPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IParticipant model, IVtcReferencedActiveCallsPresenter presenter,
		                                     IVtcReferencedActiveCallsView view)
		{
			presenter.SetView(view);
			presenter.Participant = model;
		}
	}
}
