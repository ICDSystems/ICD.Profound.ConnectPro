using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.ActiveMeeting
{
	public class WtcActiveMeetingPresenter : AbstractWtcPresenter<IWtcActiveMeetingView>, IWtcActiveMeetingPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly WtcReferencedParticipantPresenterFactory m_PresenterFactory;
		private readonly IWtcParticipantControlsPresenter m_ParticipantControls;

		private IWtcReferencedParticipantPresenter m_SelectedParticipant;

		public WtcActiveMeetingPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new WtcReferencedParticipantPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
			m_ParticipantControls = nav.LazyLoadPresenter<IWtcParticipantControlsPresenter>();
		}

		private IWtcReferencedParticipantPresenter SelectedParticipant
		{
			get { return m_SelectedParticipant; }
			set
			{
				if (m_SelectedParticipant == value)
					return;

				if (m_SelectedParticipant != null)
					UnsubscribeSelected(m_SelectedParticipant);

				m_SelectedParticipant = value;

				if (m_SelectedParticipant != null)
					SubscribeSelected(m_SelectedParticipant);

				m_ParticipantControls.Participant = m_SelectedParticipant == null ? null : m_SelectedParticipant.Participant;
				RefreshIfVisible();
			}
		}

		protected override void Refresh(IWtcActiveMeetingView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				var activeConference = ActiveConferenceControl == null
					? null
					: ActiveConferenceControl.GetActiveConference() as IWebConference;
				
				view.SetShowHideCameraButtonState(ActiveConferenceControl != null && ActiveConferenceControl.CameraEnabled);
				view.SetLeaveMeetingButtonEnabled(activeConference != null);

				var participants = activeConference == null
					? Enumerable.Empty<IWebParticipant>().ToList()
					: activeConference.GetParticipants().ToList();

				// show "no participants" and invite button if there are no participants
				view.SetNoParticipantsLabelVisibility(!participants.Any());
				view.SetInviteButtonVisibility(!participants.Any());

				foreach (var presenter in m_PresenterFactory.BuildChildren(participants))
				{
					presenter.Selected = presenter == SelectedParticipant;
					presenter.ShowView(presenter.Participant != null);
					presenter.Refresh();
				}

				// this may change if other web conferences have meeting info to display
				var zoomConference = activeConference as CallComponent; 
				view.SetMeetingNumberLabelVisibility(zoomConference != null);
				view.SetMeetingNumberLabelText(zoomConference != null
					? string.Format("Meeting #: {0}", zoomConference.Number)
					: string.Empty); 

				// only hosts can kick/mute people
				bool kickMuteEnabled = SelectedParticipant != null && (zoomConference == null ? activeConference != null : zoomConference.AmIHost);
				m_ParticipantControls.ShowView(kickMuteEnabled);

				// only hosts can end meeting for everyone
				view.SetEndMeetingButtonEnabled(zoomConference != null && zoomConference.AmIHost);
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

		private void SubscribeSelected(IWtcReferencedParticipantPresenter presenter)
		{
			if (presenter.Participant != null)
				presenter.Participant.OnIsMutedChanged += ParticipantOnOnIsMutedChanged;
		}

		private void Unsubscribe(IWtcReferencedParticipantPresenter presenter)
		{
			presenter.OnPressed -= PresenterOnOnPressed;
		}

		private void UnsubscribeSelected(IWtcReferencedParticipantPresenter presenter)
		{
			if (presenter.Participant != null)
				presenter.Participant.OnIsMutedChanged -= ParticipantOnOnIsMutedChanged;
		}

		private void PresenterOnOnPressed(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IWtcReferencedParticipantPresenter;
			SelectedParticipant = SelectedParticipant == presenter ? null : presenter;
			RefreshIfVisible();
		}

		private void ParticipantOnOnIsMutedChanged(object sender, BoolEventArgs e)
		{
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
			control.OnCameraEnabledChanged += ControlOnCameraEnabledChanged;

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
			control.OnCameraEnabledChanged -= ControlOnCameraEnabledChanged;
			
			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);
		}

		private void ControlOnOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data as IWebConference);
			RefreshIfVisible();
		}

		private void ControlOnOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data as IWebConference);
			RefreshIfVisible();
		}

		private void ControlOnCameraEnabledChanged(object sender, BoolEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IWebConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnOnParticipantRemoved;
			conference.OnStatusChanged += ConferenceOnOnStatusChanged;
		}

		private void Unsubscribe(IWebConference conference)
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

		#region Participant Callbacks

		#endregion
		
		#region View Callbacks

		protected override void Subscribe(IWtcActiveMeetingView view)
		{
			base.Subscribe(view);

			view.OnEndMeetingButtonPressed += ViewOnOnEndMeetingButtonPressed;
			view.OnLeaveMeetingButtonPressed += ViewOnOnLeaveMeetingButtonPressed;
			view.OnShowHideCameraButtonPressed += ViewOnOnShowHideCameraButtonPressed;
			view.OnInviteButtonPressed += ViewOnOnInviteButtonPressed;
		}

		protected override void Unsubscribe(IWtcActiveMeetingView view)
		{
			base.Unsubscribe(view);

			view.OnEndMeetingButtonPressed -= ViewOnOnEndMeetingButtonPressed;
			view.OnLeaveMeetingButtonPressed -= ViewOnOnLeaveMeetingButtonPressed;
			view.OnShowHideCameraButtonPressed -= ViewOnOnShowHideCameraButtonPressed;
			view.OnInviteButtonPressed -= ViewOnOnInviteButtonPressed;
		}

		private void ViewOnOnShowHideCameraButtonPressed(object sender, EventArgs eventArgs)
		{
			ActiveConferenceControl.SetCameraEnabled(!ActiveConferenceControl.CameraEnabled);
		}

		private void ViewOnOnLeaveMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			var conference = ActiveConferenceControl.GetActiveConference() as IWebConference;
			if (conference == null)
				return;

			conference.LeaveConference();
			SelectedParticipant = null;
		}

		private void ViewOnOnEndMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			var conference = ActiveConferenceControl.GetActiveConference() as IWebConference;
			if (conference == null)
				return;

			conference.EndConference();
			SelectedParticipant = null;
		}

		private void ViewOnOnInviteButtonPressed(object sender, EventArgs e)
		{
			Navigation.NavigateTo<IWtcContactListPresenter>();
		}

		#endregion
	}
}