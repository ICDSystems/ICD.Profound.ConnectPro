using System;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	[PresenterBinding(typeof(IWtcStartMeetingPresenter))]
	public sealed class WtcStartMeetingPresenter : AbstractWtcPresenter<IWtcStartMeetingView>, IWtcStartMeetingPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly StringBuilder m_Builder;

		private bool IsInConference
		{
			get { return ActiveConferenceControl != null && ActiveConferenceControl.GetActiveConference() != null; }
		}

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
			view.OnTextEntered += ViewOnTextEntered;
			view.OnBackButtonPressed += ViewOnBackButtonPressed;
			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
		}

		protected override void Unsubscribe(IWtcStartMeetingView view)
		{
			base.Unsubscribe(view);

			view.OnMeetNowButtonPressed -= ViewOnOnMeetNowButtonPressed;
			view.OnJoinByIdButtonPressed -= ViewOnJoinByIdButtonPressed;
			view.OnTextEntered -= ViewOnTextEntered;
			view.OnBackButtonPressed -= ViewOnBackButtonPressed;
			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnKeypadButtonPressed -= ViewOnKeypadButtonPressed;
		}

		private void ViewOnOnMeetNowButtonPressed(object sender, EventArgs eventArgs)
		{
			ActiveConferenceControl.StartPersonalMeeting();
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

		private void ViewOnKeypadButtonPressed(object sender, CharEventArgs e)
		{
			m_Builder.Append(e.Data);
			RefreshIfVisible();
		}

		private void ViewOnClearButtonPressed(object sender, EventArgs e)
		{
			m_Builder.Clear();
			RefreshIfVisible();
		}

		private void ViewOnBackButtonPressed(object sender, EventArgs e)
		{
			if (m_Builder.Length == 0)
				return;

			m_Builder.Remove(m_Builder.Length - 1, 1);
			RefreshIfVisible();
		}

		private void ViewOnTextEntered(object sender, StringEventArgs e)
		{
			m_Builder.Clear();
			m_Builder.Append(e.Data);
		}

		#endregion
	}
}