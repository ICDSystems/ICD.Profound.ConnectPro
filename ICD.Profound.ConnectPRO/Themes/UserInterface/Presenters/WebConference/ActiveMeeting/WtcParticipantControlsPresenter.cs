using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.ActiveMeeting
{
	[PresenterBinding(typeof(IWtcParticipantControlsPresenter))]
	public sealed class WtcParticipantControlsPresenter : AbstractWtcPresenter<IWtcParticipantControlsView>, IWtcParticipantControlsPresenter
	{
		private const ushort INDEX_KICK = 0;
		private const ushort INDEX_MUTE = 1;

		private static readonly Dictionary<ushort, string> s_ButtonLabels = new Dictionary<ushort, string>
		{
			{INDEX_KICK, "Kick Participant"},
			{INDEX_MUTE, "Mute Participant"}
		};

		private static readonly Dictionary<ushort, string> s_ButtonIcons = new Dictionary<ushort, string>
		{
			{INDEX_KICK, "icon_exit_white"},
			{INDEX_MUTE, "icon_volumeMute_white"}
		};

		private readonly SafeCriticalSection m_RefreshSection;

		private IWebParticipant m_Participant;
		public IWebParticipant Participant
		{
			get { return m_Participant; }
			set
			{
				if (m_Participant == value)
					return;

				Unsubscribe(m_Participant);
				m_Participant = value;
				Subscribe(m_Participant);

				RefreshIfVisible();
			}
		}

		public WtcParticipantControlsPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IWtcParticipantControlsView view)
		{
			base.Refresh(view);

			if (Participant == null)
				return;

			m_RefreshSection.Enter();
			try
			{
				view.SetButtonIcon(INDEX_KICK, s_ButtonIcons[INDEX_KICK]);
				view.SetButtonLabel(INDEX_KICK, s_ButtonLabels[INDEX_KICK]);

				view.SetButtonIcon(INDEX_MUTE, Participant.IsMuted ? "icon_volumeGeneric_white" : s_ButtonIcons[INDEX_MUTE]);
				view.SetButtonLabel(INDEX_MUTE, Participant.IsMuted ? "Unmute Participant" : s_ButtonLabels[INDEX_MUTE]);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		protected override void Subscribe(IWtcParticipantControlsView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnOnButtonPressed;
		}

		protected override void Unsubscribe(IWtcParticipantControlsView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnOnButtonPressed;
		}

		private void ViewOnOnButtonPressed(object sender, UShortEventArgs e)
		{
			if (Participant == null)
				return;

			switch (e.Data)
			{
				case INDEX_KICK:
					Participant.Kick();
					break;
				case INDEX_MUTE:
					Participant.Mute(!Participant.IsMuted);
					break;
			}
		}

		#endregion

		#region Participant Callbacks

		private void Subscribe(IWebParticipant participant)
		{
			if (participant != null)
				participant.OnIsMutedChanged += ParticipantOnOnIsMutedChanged;
		}

		private void Unsubscribe(IWebParticipant participant)
		{
			if (participant != null)
				participant.OnIsMutedChanged -= ParticipantOnOnIsMutedChanged;
		}

		private void ParticipantOnOnIsMutedChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
