using System;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.ActiveMeeting
{
	public sealed class WtcReferencedParticipantPresenterFactory : AbstractUiListItemFactory<IWebParticipant, IWtcReferencedParticipantPresenter, IWtcReferencedParticipantView>
	{
		public WtcReferencedParticipantPresenterFactory(IConnectProNavigationController navigationController,
		                                                ListItemFactory<IWtcReferencedParticipantView> viewFactory,
		                                                Action<IWtcReferencedParticipantPresenter> subscribe,
		                                                Action<IWtcReferencedParticipantPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(IWebParticipant model, IWtcReferencedParticipantPresenter presenter, IWtcReferencedParticipantView view)
		{
			presenter.SetView(view);
			presenter.Participant = model;
		}
	}
}