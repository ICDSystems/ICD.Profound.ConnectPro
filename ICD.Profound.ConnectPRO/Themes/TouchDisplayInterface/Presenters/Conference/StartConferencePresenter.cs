using System;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IStartConferencePresenter))]
	public sealed class StartConferencePresenter : AbstractTouchDisplayPresenter<IStartConferenceView>, IStartConferencePresenter
	{
		private readonly StringBuilder m_Builder;
		private readonly SafeCriticalSection m_RefreshSection;

		private IWebConferenceDeviceControl m_ActiveConferenceControl;

		public StartConferencePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			ConnectProTheme theme) : base(nav, views, theme)
		{
			m_Builder = new StringBuilder();
			m_RefreshSection = new SafeCriticalSection();
		}

		private bool IsInConference =>
			ActiveConferenceControl != null && ActiveConferenceControl.GetActiveConference() != null;

		public IWebConferenceDeviceControl ActiveConferenceControl
		{
			get => m_ActiveConferenceControl;
			set
			{
				if (value == m_ActiveConferenceControl)
					return;

				Unsubscribe(m_ActiveConferenceControl);
				m_ActiveConferenceControl = value;
				Subscribe(m_ActiveConferenceControl);
			}
		}

		protected override void Refresh(IStartConferenceView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				var inConference = IsInConference;
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

		private void Subscribe(IWebConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;
		}

		private void Unsubscribe(IWebConferenceDeviceControl control)
		{
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

		protected override void Subscribe(IStartConferenceView view)
		{
			base.Subscribe(view);

			view.OnMeetNowButtonPressed += ViewOnOnMeetNowButtonPressed;
			view.OnJoinByIdButtonPressed += ViewOnJoinByIdButtonPressed;
			view.OnTextEntered += ViewOnTextEntered;
			view.OnBackButtonPressed += ViewOnBackButtonPressed;
			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
		}

		protected override void Unsubscribe(IStartConferenceView view)
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
			// TODO make generic if other web conference platforms support personal meeting
			var zoomControl = ActiveConferenceControl as ZoomRoomConferenceControl;
			if (zoomControl != null)
				zoomControl.StartPersonalMeeting();
		}

		private void ViewOnJoinByIdButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			ActiveConferenceControl.Dial(new ZoomDialContext {DialString = m_Builder.ToString()});
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