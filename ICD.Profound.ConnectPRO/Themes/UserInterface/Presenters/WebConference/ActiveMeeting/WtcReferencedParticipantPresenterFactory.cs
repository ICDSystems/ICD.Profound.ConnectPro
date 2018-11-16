using ICD.Connect.Conferencing.Participants;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.ActiveMeeting
{
	public class WtcReferencedParticipantPresenterFactory : AbstractListItemFactory<IWebParticipant, IWtcReferencedParticipantPresenter, IWtcReferencedParticipantView>
	{
		public WtcReferencedParticipantPresenterFactory(IConnectProNavigationController navigationController, ListItemFactory<IWtcReferencedParticipantView> viewFactory) : base(navigationController, viewFactory)
		{
		}

		protected override void BindMvpTriad(IWebParticipant model, IWtcReferencedParticipantPresenter presenter, IWtcReferencedParticipantView view)
		{
			presenter.SetView(view);
			presenter.Participant = model;
		}
	}
}