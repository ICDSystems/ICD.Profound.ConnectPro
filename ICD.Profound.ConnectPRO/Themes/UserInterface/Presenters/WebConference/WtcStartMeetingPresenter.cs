using System;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Controls;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public sealed class WtcStartMeetingPresenter : AbstractWtcPresenter<IWtcStartMeetingView>, IWtcStartMeetingPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly StringBuilder m_Builder;

		public WtcStartMeetingPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_Builder = new StringBuilder();
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IWtcStartMeetingView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				bool inConference = IsInConference;
				view.SetMeetNowButtonEnabled(!inConference);
				view.SetJoinByIdButtonEnabled(true);
				
				view.SetMeetingIdText(m_Builder.ToString());
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

		protected override void Subscribe(IWtcStartMeetingView view)
		{
			base.Subscribe(view);

			view.OnMeetNowButtonPressed += ViewOnOnMeetNowButtonPressed;
			view.OnJoinByIdButtonPressed += ViewOnJoinByIdButtonPressed;
			view.OnTextEntered += ViewOnOnTextEntered;
			view.OnBackButtonPressed += ViewOnOnBackButtonPressed;
			view.OnClearButtonPressed += ViewOnOnClearButtonPressed;
			view.OnKeypadButtonPressed += ViewOnOnKeypadButtonPressed;
		}

		protected override void Unsubscribe(IWtcStartMeetingView view)
		{
			base.Unsubscribe(view);

			view.OnMeetNowButtonPressed -= ViewOnOnMeetNowButtonPressed;
			view.OnJoinByIdButtonPressed -= ViewOnJoinByIdButtonPressed;
			view.OnTextEntered -= ViewOnOnTextEntered;
			view.OnBackButtonPressed -= ViewOnOnBackButtonPressed;
			view.OnClearButtonPressed -= ViewOnOnClearButtonPressed;
			view.OnKeypadButtonPressed -= ViewOnOnKeypadButtonPressed;
		}

		private void ViewOnOnMeetNowButtonPressed(object sender, EventArgs eventArgs)
		{
			// TODO make generic if other web conference platforms support personal meeting
			var zoomControl = ActiveConferenceControl as ZoomRoomConferenceControl;
			if(zoomControl != null)
				zoomControl.StartPersonalMeeting();
		}

		private void ViewOnJoinByIdButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			ActiveConferenceControl.Dial(new ZoomDialContext { DialString = m_Builder.ToString() });
			RefreshIfVisible();
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_Builder.Clear();
			RefreshIfVisible();
		}

		private void ViewOnOnKeypadButtonPressed(object sender, CharEventArgs e)
		{
			m_Builder.Append(e.Data);
			RefreshIfVisible();
		}

		private void ViewOnOnClearButtonPressed(object sender, EventArgs e)
		{
			m_Builder.Clear();
			RefreshIfVisible();
		}

		private void ViewOnOnBackButtonPressed(object sender, EventArgs e)
		{
			m_Builder.Remove(m_Builder.Length - 1, 1);
			RefreshIfVisible();
		}

		private void ViewOnOnTextEntered(object sender, StringEventArgs e)
		{
			m_Builder.Clear();
			m_Builder.Append(e.Data);
		}

		#endregion
	}
}