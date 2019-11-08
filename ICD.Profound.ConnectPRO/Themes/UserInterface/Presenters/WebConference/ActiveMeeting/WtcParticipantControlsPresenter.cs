using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
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
		private const ushort INDEX_RECORD = 2;

		private static readonly Dictionary<ushort, string> s_ButtonLabels = new Dictionary<ushort, string>
		{
			{INDEX_KICK, "Kick Participant"},
			{INDEX_MUTE, "Mute Participant"},
			{INDEX_RECORD, "Allow Recording"},
		};

		private static readonly Dictionary<ushort, string> s_ButtonIcons = new Dictionary<ushort, string>
		{
			{INDEX_KICK, "icon_exit_white"},
			{INDEX_MUTE, "icon_volumeMute_white"},
			{INDEX_RECORD, "icon_tcRecord_red"},
		};

		private readonly SafeCriticalSection m_RefreshSection;

		private IWebParticipant m_Participant;

		/// <summary>
		/// Gets/sets the active participant for this presenter.
		/// </summary>
		[CanBeNull]
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

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WtcParticipantControlsPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IWtcParticipantControlsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				// Kick
				view.SetButtonIcon(INDEX_KICK, s_ButtonIcons[INDEX_KICK]);
				view.SetButtonLabel(INDEX_KICK, s_ButtonLabels[INDEX_KICK]);

				// Mute
				bool isMuted = Participant != null && Participant.IsMuted;
				view.SetButtonIcon(INDEX_MUTE, isMuted ? "icon_volumeGeneric_white" : s_ButtonIcons[INDEX_MUTE]);
				view.SetButtonLabel(INDEX_MUTE, isMuted ? "Unmute Participant" : s_ButtonLabels[INDEX_MUTE]);

				// Record
				ZoomWebParticipant zoomParticipant = Participant as ZoomWebParticipant;
				bool canRecord = zoomParticipant != null && zoomParticipant.CanRecord;
				view.SetButtonIcon(INDEX_RECORD, canRecord ? s_ButtonIcons[INDEX_RECORD] : "icon_tcRecord_white");
				view.SetButtonLabel(INDEX_RECORD, canRecord ? "Disallow Recording" : s_ButtonLabels[INDEX_RECORD]);
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

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		protected override void Unsubscribe(IWtcParticipantControlsView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		private void ViewOnButtonPressed(object sender, UShortEventArgs e)
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
				case INDEX_RECORD:
					ZoomWebParticipant zoomParticipant = Participant as ZoomWebParticipant;
					if (zoomParticipant != null)
						zoomParticipant.AllowParticipantRecord(!zoomParticipant.CanRecord);
					break;
			}
		}

		#endregion

		#region Participant Callbacks

		/// <summary>
		/// Subscribe to the participant events.
		/// </summary>
		/// <param name="participant"></param>
		private void Subscribe(IWebParticipant participant)
		{
			if (participant != null)
				participant.OnIsMutedChanged += ParticipantOnIsMuted;

			ZoomWebParticipant zoomParticipant = participant as ZoomWebParticipant;
			if (zoomParticipant != null)
				zoomParticipant.OnCanRecordChanged += ZoomParticipantOnCanRecordChanged;
		}

		/// <summary>
		/// Unsubscribe from the participant events.
		/// </summary>
		/// <param name="participant"></param>
		private void Unsubscribe(IWebParticipant participant)
		{
			if (participant != null)
				participant.OnIsMutedChanged -= ParticipantOnIsMuted;

			ZoomWebParticipant zoomParticipant = participant as ZoomWebParticipant;
			if (zoomParticipant != null)
				zoomParticipant.OnCanRecordChanged -= ZoomParticipantOnCanRecordChanged;
		}

		/// <summary>
		/// Called when the participant mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ParticipantOnIsMuted(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the participant can record state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ZoomParticipantOnCanRecordChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
