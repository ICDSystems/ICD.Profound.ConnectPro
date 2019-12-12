using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.ActiveConference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Conference.ActiveConference
{
	[PresenterBinding(typeof(IReferencedParticipantPresenter))]
	public sealed class ReferencedParticipantPresenter : AbstractTouchDisplayComponentPresenter<IReferencedParticipantView>, IReferencedParticipantPresenter
	{
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private IParticipant m_Participant;
		private bool m_Selected;

		[CanBeNull]
		public IParticipant Participant
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
		public ReferencedParticipantPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		protected override void Refresh(IReferencedParticipantView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				string tags;

				var webParticipant = Participant as IWebParticipant;
				if (webParticipant == null)
					tags = string.Empty;
				else if (webParticipant.IsHost && webParticipant.IsSelf)
					tags = " (Self, Host)";
				else if (webParticipant.IsHost)
					tags = " (Host)";
				else if (webParticipant.IsSelf)
					tags = " (Self)";
				else
					tags = string.Empty;

				view.SetParticipantName(Participant == null ? string.Empty : Participant.Name + tags);
				view.SetButtonSelected(Selected);
				view.SetMuteIconVisibility(webParticipant != null && webParticipant.IsMuted);

				var zoomParticipant = webParticipant as ZoomWebParticipant;
				view.SetAvatarImageVisibility(zoomParticipant != null && !string.IsNullOrEmpty(zoomParticipant.AvatarUrl));
				view.SetAvatarImagePath(zoomParticipant == null ? null : zoomParticipant.AvatarUrl);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Participant Callbacks

		private void Subscribe(IParticipant participant)
		{
			participant.OnNameChanged += ParticipantOnOnNameChanged;

			var webParticipant = participant as IWebParticipant;
			if (webParticipant == null)
				return;

			webParticipant.OnIsMutedChanged += ParticipantOnIsMutedChanged;
			webParticipant.OnIsHostChanged += ParticipantOnIsHostChanged;
		}

		private void Unsubscribe(IParticipant participant)
		{
			participant.OnNameChanged -= ParticipantOnOnNameChanged;

			var webParticipant = participant as IWebParticipant;
			if (webParticipant == null)
				return;

			webParticipant.OnIsMutedChanged -= ParticipantOnIsMutedChanged;
			webParticipant.OnIsHostChanged -= ParticipantOnIsHostChanged;
		}

		private void ParticipantOnOnNameChanged(object sender, StringEventArgs e)
		{
			RefreshIfVisible();
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

		protected override void Subscribe(IReferencedParticipantView view)
		{
			base.Subscribe(view);

			view.OnParticipantPressed += ViewOnParticipantPressed;
		}

		protected override void Unsubscribe(IReferencedParticipantView view)
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