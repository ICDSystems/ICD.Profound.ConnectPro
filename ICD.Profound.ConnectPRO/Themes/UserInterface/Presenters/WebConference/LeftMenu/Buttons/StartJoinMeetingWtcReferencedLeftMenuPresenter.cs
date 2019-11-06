using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(IStartJoinMeetingWtcReferencedLeftMenuPresenter))]
	public sealed class StartJoinMeetingWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter,
	                                                                     IStartJoinMeetingWtcReferencedLeftMenuPresenter
	{
		private readonly IWtcStartMeetingPresenter m_StartMeetingPresenter;
		private readonly IWtcActiveMeetingPresenter m_ActiveMeetingPresenter;

		private bool IsInWebConference
		{
			get
			{
				return
					ActiveConferenceControl != null &&
					ActiveConferenceControl.GetActiveConference() != null;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public StartJoinMeetingWtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                                      ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_StartMeetingPresenter = Navigation.LazyLoadPresenter<IWtcStartMeetingPresenter>();
			Subscribe(m_StartMeetingPresenter);

			m_ActiveMeetingPresenter = Navigation.LazyLoadPresenter<IWtcActiveMeetingPresenter>();
			Subscribe(m_ActiveMeetingPresenter);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_StartMeetingPresenter);
			Unsubscribe(m_ActiveMeetingPresenter);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcReferencedLeftMenuView view)
		{
			Icon = "videoConference";
			Enabled = true;
			Selected = m_StartMeetingPresenter.IsViewVisible || m_ActiveMeetingPresenter.IsViewVisible;
			Label = IsInWebConference ? "Active Meeting" : "Start/Join Meeting";
			State = IsInWebConference;

			base.Refresh(view);
		}

		public override void HideSubpages()
		{
			m_StartMeetingPresenter.ShowView(false);
			m_ActiveMeetingPresenter.ShowView(false);
		}

		#region ActiveMeetingPresenter Callbacks

		private void Subscribe(IWtcActiveMeetingPresenter activeMeetingPresenter)
		{
			activeMeetingPresenter.OnViewVisibilityChanged += ActiveMeetingPresenterOnViewVisibilityChanged;
		}

		private void Unsubscribe(IWtcActiveMeetingPresenter activeMeetingPresenter)
		{
			activeMeetingPresenter.OnViewVisibilityChanged -= ActiveMeetingPresenterOnViewVisibilityChanged;
		}

		private void ActiveMeetingPresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region StartMeetingPresenter Callbacks

		private void Subscribe(IWtcStartMeetingPresenter startMeetingPresenter)
		{
			startMeetingPresenter.OnViewVisibilityChanged += StartMeetingPresenterOnViewVisibilityChanged;
		}

		private void Unsubscribe(IWtcStartMeetingPresenter startMeetingPresenter)
		{
			startMeetingPresenter.OnViewVisibilityChanged -= StartMeetingPresenterOnViewVisibilityChanged;
		}

		private void StartMeetingPresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (IWebConference conference in control.GetConferences())
				Subscribe(conference);
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			foreach (IWebConference conference in control.GetConferences())
				Unsubscribe(conference);
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data as IWebConference);
			RefreshIfVisible();
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data as IWebConference);
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IWebConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnParticipantRemoved;
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IWebConference conference)
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

		/// <summary>
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected override void HandleButtonPress()
		{
			if (IsInWebConference)
				Navigation.NavigateTo<IWtcActiveMeetingPresenter>();
			else
				Navigation.NavigateTo<IWtcStartMeetingPresenter>();
		}
	}
}
