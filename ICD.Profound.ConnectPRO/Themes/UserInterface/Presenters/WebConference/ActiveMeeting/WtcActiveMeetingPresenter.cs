using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Components.Call;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.ActiveMeeting
{
	[PresenterBinding(typeof(IWtcActiveMeetingPresenter))]
	public sealed class WtcActiveMeetingPresenter : AbstractWtcPresenter<IWtcActiveMeetingView>, IWtcActiveMeetingPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly WtcReferencedParticipantPresenterFactory m_PresenterFactory;
		private readonly IWtcParticipantControlsPresenter m_ParticipantControls;

		private IWtcReferencedParticipantPresenter m_SelectedParticipant;

		private IWtcReferencedParticipantPresenter SelectedParticipant
		{
			get { return m_SelectedParticipant; }
			set
			{
				if (m_SelectedParticipant == value)
					return;

				UnsubscribeSelected(m_SelectedParticipant);
				m_SelectedParticipant = value;
				SubscribeSelected(m_SelectedParticipant);

				m_ParticipantControls.Participant = m_SelectedParticipant == null ? null : m_SelectedParticipant.Participant;
				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WtcActiveMeetingPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new WtcReferencedParticipantPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
			m_ParticipantControls = nav.LazyLoadPresenter<IWtcParticipantControlsPresenter>();
		}

		protected override void Refresh(IWtcActiveMeetingView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				var activeConference = ActiveConferenceControl == null
					? null
					: ActiveConferenceControl.GetActiveConference();
				
				view.SetShowHideCameraButtonState(ActiveConferenceControl != null && !ActiveConferenceControl.CameraMute);
				view.SetLeaveMeetingButtonEnabled(activeConference != null);

				List<IParticipant> sortedParticipants =
					activeConference == null
						? new List<IParticipant>()
						: activeConference.GetParticipants()
						                  .OrderByDescending(p => p.IsHost)
						                  .ThenByDescending(p => p.IsSelf)
						                  .ThenBy(p => p.Name)
						                  .ToList();

				foreach (IWtcReferencedParticipantPresenter presenter in m_PresenterFactory.BuildChildren(sortedParticipants))
				{
					presenter.Selected = presenter == SelectedParticipant;
					presenter.ShowView(true);
				}

				// Show "no participants" and invite button if there are no participants
				view.SetNoParticipantsLabelVisibility(sortedParticipants.Count == 0);
				view.SetInviteButtonVisibility(sortedParticipants.Count == 0);

				// This may change if other web conferences have meeting info to display
				ZoomRoom zoomRoom = ActiveConferenceControl == null ? null : ActiveConferenceControl.Parent as ZoomRoom;
				CallComponent component = zoomRoom == null ? null : zoomRoom.Components.GetComponent<CallComponent>();
				view.SetMeetingNumberLabelVisibility(component != null);
				view.SetMeetingNumberLabelText(component != null
					? string.Format("Meeting #: {0}", component.MeetingIdFormatted)
					: string.Empty);

				// Only hosts can kick/mute people
				bool isHost = ActiveConferenceControl != null && ActiveConferenceControl.AmIHost;
				bool isNotSelf = SelectedParticipant != null && SelectedParticipant.Participant != null && !SelectedParticipant.Participant.IsSelf;
				bool kickMuteEnabled = isHost && isNotSelf;
				m_ParticipantControls.ShowView(kickMuteEnabled);

				// Only hosts can end meeting for everyone
				view.SetEndMeetingButtonEnabled(component != null && component.AmIHost);

				// Call lock
				bool locked = ActiveConferenceControl != null && ActiveConferenceControl.CallLock;
				view.SetlockButtonSelected(locked);
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
			if (presenter != null)
				presenter.OnPressed += PresenterOnOnPressed;
		}

		private void Unsubscribe(IWtcReferencedParticipantPresenter presenter)
		{
			if (presenter != null)
				presenter.OnPressed -= PresenterOnOnPressed;
		}

		private void PresenterOnOnPressed(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IWtcReferencedParticipantPresenter;
			SelectedParticipant = SelectedParticipant == presenter ? null : presenter;
			RefreshIfVisible();
		}

		#endregion

		#region Selected Participant Callbacks

		private void SubscribeSelected(IWtcReferencedParticipantPresenter presenter)
		{
		}

		private void UnsubscribeSelected(IWtcReferencedParticipantPresenter presenter)
		{
		}

		#endregion

		#region Control Callbacks

		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;
			control.OnCameraMuteChanged += ControlOnCameraMuteChanged;
			control.OnAmIHostChanged += ControlOnOnAmIHostChanged;
			control.OnCallLockChanged += ControlOnCallLockChanged;

			foreach (IConference conference in control.GetConferences())
				Subscribe(conference);
		}

		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;
			control.OnCameraMuteChanged -= ControlOnCameraMuteChanged;
			control.OnAmIHostChanged -= ControlOnOnAmIHostChanged;
			control.OnCallLockChanged -= ControlOnCallLockChanged;
			
			foreach (IConference conference in control.GetConferences())
				Unsubscribe(conference);
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			RefreshIfVisible();
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			RefreshIfVisible();
		}

		private void ControlOnCameraMuteChanged(object sender, BoolEventArgs args)
		{
			RefreshIfVisible();
		}

		private void ControlOnCallLockChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		private void ControlOnOnAmIHostChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnParticipantRemoved;
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			conference.OnParticipantAdded -= ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved -= ConferenceOnParticipantRemoved;
			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnParticipantAdded(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnParticipantRemoved(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
		
		#region View Callbacks

		protected override void Subscribe(IWtcActiveMeetingView view)
		{
			base.Subscribe(view);

			view.OnEndMeetingButtonPressed += ViewOnEndMeetingButtonPressed;
			view.OnLeaveMeetingButtonPressed += ViewOnLeaveMeetingButtonPressed;
			view.OnShowHideCameraButtonPressed += ViewOnShowHideCameraButtonPressed;
			view.OnInviteButtonPressed += ViewOnInviteButtonPressed;
			view.OnLockButtonPressed += ViewOnLockButtonPressed;
		}

		protected override void Unsubscribe(IWtcActiveMeetingView view)
		{
			base.Unsubscribe(view);

			view.OnEndMeetingButtonPressed -= ViewOnEndMeetingButtonPressed;
			view.OnLeaveMeetingButtonPressed -= ViewOnLeaveMeetingButtonPressed;
			view.OnShowHideCameraButtonPressed -= ViewOnShowHideCameraButtonPressed;
			view.OnInviteButtonPressed -= ViewOnInviteButtonPressed;
			view.OnLockButtonPressed -= ViewOnLockButtonPressed;
		}

		private void ViewOnShowHideCameraButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl != null)
				ActiveConferenceControl.SetCameraMute(!ActiveConferenceControl.CameraMute);
		}

		private void ViewOnLeaveMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			var conference = ActiveConferenceControl.GetActiveConference();
			if (conference == null)
				return;

			conference.LeaveConference();
			SelectedParticipant = null;
		}

		private void ViewOnEndMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			var conference = ActiveConferenceControl.GetActiveConference();
			if (conference == null)
				return;

			conference.EndConference();
			SelectedParticipant = null;
		}

		private void ViewOnInviteButtonPressed(object sender, EventArgs e)
		{
			Navigation.NavigateTo<IWtcContactListPresenter>();
		}

		private void ViewOnLockButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			ActiveConferenceControl.EnableCallLock(!ActiveConferenceControl.CallLock);
		}

		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			SelectedParticipant = null;
			if (!args.Data)
				m_ParticipantControls.ShowView(false);
		}

		#endregion
	}
}