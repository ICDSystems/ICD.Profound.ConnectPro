using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Participants;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.ActiveMeeting
{
	public class WtcReferencedParticipantPresenter : AbstractUiComponentPresenter<IWtcReferencedParticipantView>, IWtcReferencedParticipantPresenter
	{
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		public IWebParticipant Participant { get; set; }

		public bool Selected { get; set; }

		public WtcReferencedParticipantPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		protected override void Refresh(IWtcReferencedParticipantView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetParticipantName(Participant.Name);
				view.SetButtonSelected(Selected);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		protected override void Subscribe(IWtcReferencedParticipantView view)
		{
			base.Subscribe(view);

			view.OnParticipantPressed += ViewOnOnParticipantPressed;
		}

		protected override void Unsubscribe(IWtcReferencedParticipantView view)
		{
			base.Unsubscribe(view);

			view.OnParticipantPressed -= ViewOnOnParticipantPressed;
		}

		private void ViewOnOnParticipantPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}