using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.ActiveMeeting
{
	[PresenterBinding(typeof(IWtcReferencedParticipantPresenter))]
	public sealed class WtcReferencedParticipantPresenter : AbstractUiComponentPresenter<IWtcReferencedParticipantView>, IWtcReferencedParticipantPresenter
	{
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private IWebParticipant m_Participant;
		private bool m_Selected;

		[CanBeNull]
		public IWebParticipant Participant
		{
			get { return m_Participant; }
			set
			{
				if (m_Participant == value)
					return;

				if (m_Participant != null)
					Unsubscribe(m_Participant);

				m_Participant = value;

				if (m_Participant != null)
					Subscribe(m_Participant);

				RefreshIfVisible();
			}
		}

		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
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
				string tags;

				if (Participant == null)
					tags = string.Empty;
				else if (Participant.IsHost && Participant.IsSelf)
					tags = " (Self, Host)";
				else if (Participant.IsHost)
					tags = " (Host)";
				else if (Participant.IsSelf)
					tags = " (Self)";
				else
					tags = string.Empty;

				view.SetParticipantName(Participant == null ? string.Empty : Participant.Name + tags);
				view.SetButtonSelected(Selected);
				view.SetMuteIconVisibility(Participant != null && Participant.IsMuted);

				var zoomParticipant = Participant as ZoomWebParticipant;
				view.SetAvatarImageVisibility(zoomParticipant != null && !string.IsNullOrEmpty(zoomParticipant.AvatarUrl));
				view.SetAvatarImagePath(zoomParticipant == null ? null : zoomParticipant.AvatarUrl);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Participant Callbacks

		private void Subscribe(IWebParticipant participant)
		{
			participant.OnIsMutedChanged += ParticipantOnIsMutedChanged;
			participant.OnIsHostChanged += ParticipantOnIsHostChanged;
		}

		private void Unsubscribe(IWebParticipant participant)
		{
			participant.OnIsMutedChanged -= ParticipantOnIsMutedChanged;
			participant.OnIsHostChanged -= ParticipantOnIsHostChanged;
		}

		private void ParticipantOnIsMutedChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		private void ParticipantOnIsHostChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IWtcReferencedParticipantView view)
		{
			base.Subscribe(view);

			view.OnParticipantPressed += ViewOnParticipantPressed;
		}

		protected override void Unsubscribe(IWtcReferencedParticipantView view)
		{
			base.Unsubscribe(view);

			view.OnParticipantPressed -= ViewOnParticipantPressed;
		}

		private void ViewOnParticipantPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}