using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.ActiveMeeting
{
	public class WtcActiveMeetingPresenter : AbstractWtcPresenter<IWtcActiveMeetingView>, IWtcActiveMeetingPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly WtcReferencedParticipantPresenterFactory m_PresenterFactory;

		private IWtcReferencedParticipantPresenter m_SelectedParticipant;

		public WtcActiveMeetingPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new WtcReferencedParticipantPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
		}

		protected override void Refresh(IWtcActiveMeetingView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				if (ActiveConferenceControl == null)
				{
					ShowView(false);
					return;
				}

				var activeConference = ActiveConferenceControl.GetActiveConference() as IWebConference;

				view.SetEndMeetingButtonEnabled(activeConference != null);
				view.SetLeaveMeetingButtonEnabled(activeConference != null);
				view.SetKickParticipantButtonEnabled(activeConference != null && m_SelectedParticipant != null);
				view.SetMuteParticipantButtonEnabled(activeConference != null && m_SelectedParticipant != null);

				var participants = activeConference == null
					? Enumerable.Empty<IWebParticipant>()
					: activeConference.GetParticipants();
				foreach (var presenter in m_PresenterFactory.BuildChildren(participants))
				{
					presenter.Selected = presenter == m_SelectedParticipant;
					presenter.ShowView(presenter.Participant != null);
					presenter.Refresh();
				}

				var zoomConference = activeConference as CallComponent;
				view.SetMeetingIdLabelVisibility(zoomConference != null);
				view.SetCallInLabelVisibility(zoomConference != null && zoomConference.CallInfo != null);
				if (zoomConference == null)
					return;

				view.SetMeetingIdLabelText(zoomConference.Number);
				view.SetCallInLabelText(zoomConference.CallInfo == null ? string.Empty : zoomConference.CallInfo.DialIn);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private IEnumerable<IWtcReferencedParticipantView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#endregion

		#region Participant Callbacks

		private void Subscribe(IWtcReferencedParticipantPresenter presenter)
		{
			presenter.OnPressed += PresenterOnOnPressed;
		}

		private void Unsubscribe(IWtcReferencedParticipantPresenter presenter)
		{
			presenter.OnPressed -= PresenterOnOnPressed;
		}

		private void PresenterOnOnPressed(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IWtcReferencedParticipantPresenter;
			if (m_SelectedParticipant == presenter)
				m_SelectedParticipant = null;
			else
				m_SelectedParticipant = presenter;
			RefreshIfVisible();
		}

		#endregion

		#region Control Callbacks

		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);
		}

		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnOnConferenceRemoved;
			
			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);
		}

		private void ControlOnOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			RefreshIfVisible();
		}

		private void ControlOnOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnOnParticipantRemoved;
			conference.OnStatusChanged += ConferenceOnOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			conference.OnParticipantAdded -= ConferenceOnOnParticipantAdded;
			conference.OnParticipantRemoved -= ConferenceOnOnParticipantRemoved;
			conference.OnStatusChanged -= ConferenceOnOnStatusChanged;
		}

		private void ConferenceOnOnParticipantRemoved(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnOnParticipantAdded(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnOnStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IWtcActiveMeetingView view)
		{
			base.Subscribe(view);

			view.OnEndMeetingButtonPressed += ViewOnOnEndMeetingButtonPressed;
			view.OnLeaveMeetingButtonPressed += ViewOnOnLeaveMeetingButtonPressed;
			view.OnKickParticipantButtonPressed += ViewOnOnKickParticipantButtonPressed;
			view.OnMuteParticipantButtonPressed += ViewOnOnMuteParticipantButtonPressed;
			view.OnShowHideCameraButtonPressed += ViewOnOnShowHideCameraButtonPressed;
		}

		protected override void Unsubscribe(IWtcActiveMeetingView view)
		{
			base.Unsubscribe(view);

			view.OnEndMeetingButtonPressed -= ViewOnOnEndMeetingButtonPressed;
			view.OnLeaveMeetingButtonPressed -= ViewOnOnLeaveMeetingButtonPressed;
			view.OnKickParticipantButtonPressed -= ViewOnOnKickParticipantButtonPressed;
			view.OnMuteParticipantButtonPressed -= ViewOnOnMuteParticipantButtonPressed;
			view.OnShowHideCameraButtonPressed -= ViewOnOnShowHideCameraButtonPressed;
		}

		private void ViewOnOnShowHideCameraButtonPressed(object sender, EventArgs eventArgs)
		{
			ActiveConferenceControl.SetCameraEnabled(!ActiveConferenceControl.CameraEnabled);
		}

		private void ViewOnOnMuteParticipantButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_SelectedParticipant == null)
				return;
			var participant = m_SelectedParticipant.Participant;
			participant.Mute(!participant.IsMuted);
		}

		private void ViewOnOnKickParticipantButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_SelectedParticipant == null)
				return;
			var participant = m_SelectedParticipant.Participant;
			participant.Kick();
			m_SelectedParticipant = null;
		}

		private void ViewOnOnLeaveMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			var conference = ActiveConferenceControl.GetActiveConference() as IWebConference;
			if (conference == null)
				return;

			conference.LeaveConference();
			m_SelectedParticipant = null;
		}

		private void ViewOnOnEndMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			var conference = ActiveConferenceControl.GetActiveConference() as IWebConference;
			if (conference == null)
				return;

			conference.EndConference();
			m_SelectedParticipant = null;
		}

		#endregion
	}
}