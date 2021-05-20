using System;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.ActiveConference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference.ActiveConference
{
	public sealed class ReferencedParticipantPresenterFactory : AbstractTouchDisplayListItemFactory<IParticipant, IReferencedParticipantPresenter, IReferencedParticipantView>
	{
		public ReferencedParticipantPresenterFactory(ITouchDisplayNavigationController navigationController,
		                                                ListItemFactory<IReferencedParticipantView> viewFactory,
		                                                Action<IReferencedParticipantPresenter> subscribe,
		                                                Action<IReferencedParticipantPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(IParticipant model, IReferencedParticipantPresenter presenter, IReferencedParticipantView view)
		{
			presenter.SetView(view);
			presenter.Participant = model;
		}
	}
}