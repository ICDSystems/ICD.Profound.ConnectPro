using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public sealed class WtcMainPagePresenter : AbstractWtcPresenter<IWtcMainPageView>, IWtcMainPagePresenter
	{
		private readonly IWtcContactListPresenter m_ContactListPresenter;
		private readonly IWtcJoinByIdPresenter m_JoinByIdPresenter;
		private readonly IWtcActiveMeetingTogglePresenter m_TogglePresenter;
		private readonly SafeCriticalSection m_RefreshSection;

		public WtcMainPagePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_ContactListPresenter = nav.LazyLoadPresenter<IWtcContactListPresenter>();
			m_JoinByIdPresenter = nav.LazyLoadPresenter<IWtcJoinByIdPresenter>();
			m_TogglePresenter = nav.LazyLoadPresenter<IWtcActiveMeetingTogglePresenter>();

			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IWtcMainPageView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				bool inConference = IsInConference;
				
				view.SetMeetNowButtonEnabled(!inConference);
				view.SetJoinByIdButtonEnabled(!inConference);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private bool IsInConference
		{
			get { return ActiveConferenceControl != null && ActiveConferenceControl.GetActiveConference() != null; }
		}

		#region Control Callbacks

		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;
		}

		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			RefreshIfVisible();
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged += ConferenceOnOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged -= ConferenceOnOnStatusChanged;
		}

		private void ConferenceOnOnStatusChanged(object sender, ConferenceStatusEventArgs conferenceStatusEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IWtcMainPageView view)
		{
			base.Subscribe(view);

			view.OnMeetNowButtonPressed += ViewOnOnMeetNowButtonPressed;
			view.OnContactsButtonPressed += ViewOnContactsButtonPressed;
			view.OnJoinByIdButtonPressed += ViewOnJoinByIdButtonPressed;
		}

		protected override void Unsubscribe(IWtcMainPageView view)
		{
			base.Unsubscribe(view);

			view.OnMeetNowButtonPressed -= ViewOnOnMeetNowButtonPressed;
			view.OnContactsButtonPressed -= ViewOnContactsButtonPressed;
			view.OnJoinByIdButtonPressed -= ViewOnJoinByIdButtonPressed;
		}

		private void ViewOnOnMeetNowButtonPressed(object sender, EventArgs eventArgs)
		{
			// TODO make generic if other web conference platforms support personal meeting
			var zoomControl = ActiveConferenceControl as ZoomRoomConferenceControl;
			if(zoomControl != null)
				zoomControl.StartPersonalMeeting();
		}

		private void ViewOnContactsButtonPressed(object sender, EventArgs eventArgs)
		{
			m_ContactListPresenter.ShowView(true);
			m_JoinByIdPresenter.ShowView(false);
		}

		private void ViewOnJoinByIdButtonPressed(object sender, EventArgs eventArgs)
		{
			m_ContactListPresenter.ShowView(false);
			m_JoinByIdPresenter.ShowView(true);
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_ContactListPresenter.ShowView(args.Data);
			m_JoinByIdPresenter.ShowView(false);
			m_TogglePresenter.ShowView(args.Data && IsInConference);
		}

		#endregion
	}
}