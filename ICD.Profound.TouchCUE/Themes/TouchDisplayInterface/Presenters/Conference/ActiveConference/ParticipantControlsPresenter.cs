using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Controls.Conferencing;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.ActiveConference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference.ActiveConference
{
	[PresenterBinding(typeof(IParticipantControlsPresenter))]
	public sealed class ParticipantControlsPresenter : AbstractTouchDisplayPresenter<IParticipantControlsView>, IParticipantControlsPresenter
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

		private readonly SafeCriticalSection m_RefreshSection;

		private IParticipant m_Participant;

		/// <summary>
		/// Gets/sets the active participant for this presenter.
		/// </summary>
		[CanBeNull]
		public IParticipant Participant
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
		public ParticipantControlsPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IParticipantControlsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetNumberOfItems((ushort)s_ButtonLabels.Count);

				// Kick
				view.SetButtonLabel(INDEX_KICK, s_ButtonLabels[INDEX_KICK]);
				view.SetButtonVisible(INDEX_KICK, true);

				// Mute
				bool isMuted = Participant != null && Participant.IsMuted;
				view.SetButtonLabel(INDEX_MUTE, isMuted ? "Unmute Participant" : s_ButtonLabels[INDEX_MUTE]);
				view.SetButtonVisible(INDEX_MUTE, true);

				// Record
				ZoomParticipant zoomParticipant = Participant as ZoomParticipant;
				bool canRecord = zoomParticipant != null && zoomParticipant.CanRecord;
				view.SetButtonLabel(INDEX_RECORD, canRecord ? "Disallow Recording" : s_ButtonLabels[INDEX_RECORD]);
				view.SetButtonVisible(INDEX_RECORD, true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		protected override void Subscribe(IParticipantControlsView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		protected override void Unsubscribe(IParticipantControlsView view)
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
					ZoomParticipant zoomParticipant = Participant as ZoomParticipant;
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
		private void Subscribe(IParticipant participant)
		{
			if (participant != null)
				participant.OnIsMutedChanged += ParticipantOnIsMuted;

			ZoomParticipant zoomParticipant = participant as ZoomParticipant;
			if (zoomParticipant != null)
				zoomParticipant.OnCanRecordChanged += ZoomParticipantOnCanRecordChanged;
		}

		/// <summary>
		/// Unsubscribe from the participant events.
		/// </summary>
		/// <param name="participant"></param>
		private void Unsubscribe(IParticipant participant)
		{
			if (participant != null)
				participant.OnIsMutedChanged -= ParticipantOnIsMuted;

			ZoomParticipant zoomParticipant = participant as ZoomParticipant;
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
