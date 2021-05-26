using System;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Components.Call;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Controls.Conferencing;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IStartConferencePresenter))]
	public sealed class StartConferencePresenter : AbstractConferencePresenter<IStartConferenceView>, IStartConferencePresenter
	{
		private readonly StringBuilder m_Builder;
		private readonly SafeCriticalSection m_RefreshSection;

		public StartConferencePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			TouchCueTheme theme) : base(nav, views, theme)
		{
			m_Builder = new StringBuilder();
			m_RefreshSection = new SafeCriticalSection();
		}

		private bool IsInConference
		{
			get { return ActiveConferenceControl != null && ActiveConferenceControl.GetActiveConference() != null; }
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

		#region Private Methods

		private void SubmitZoomPassword(string meetingNumber, string password)
		{
			if (Room == null)
				return;

			ZoomRoomConferenceControl zoomControl = ActiveConferenceControl as ZoomRoomConferenceControl;
			if (zoomControl == null)
				return;

			DialContext context = new DialContext
			{
				Protocol = eDialProtocol.Zoom,
				CallType = eCallType.Video,
				DialString = meetingNumber,
				Password = password
			};

			Room.Dialing.Dial(zoomControl, context);
		}

		#endregion

		#region Control Callbacks

		protected override void Subscribe(IConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			ZoomRoomConferenceControl zoomControl = control as ZoomRoomConferenceControl;
			CallComponent zoomCallComponent =
				zoomControl == null ? null : zoomControl.Parent.Components.GetComponent<CallComponent>();
			if (zoomCallComponent != null)
			{
				zoomCallComponent.OnPasswordRequired += ZoomControlOnPasswordRequired;
			}
		}

		protected override void Unsubscribe(IConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			ZoomRoomConferenceControl zoomControl = control as ZoomRoomConferenceControl;
			CallComponent zoomCallComponent =
				zoomControl == null ? null : zoomControl.Parent.Components.GetComponent<CallComponent>();
			if (zoomCallComponent != null)
			{
				zoomCallComponent.OnPasswordRequired -= ZoomControlOnPasswordRequired;
			}
		}

		/// <summary>
		/// Called when Zoom requests a password for a meeting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ZoomControlOnPasswordRequired(object sender, MeetingNeedsPasswordEventArgs args)
		{
			if (!args.NeedsPassword)
				return;

			string prompt = args.WrongAndRetry
				                ? string.Format("Please re-enter password for Zoom Meeting #{0} - Incorrect password", args.MeetingNumberFormatted)
				                : string.Format("Please enter password for Zoom Meeting #{0}", args.MeetingNumberFormatted);

			Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>()
			          .ShowView(prompt, null, p => SubmitZoomPassword(args.MeetingNumber, p), null, null);
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
			if (Room == null)
				return;

			if (ActiveConferenceControl != null)
				Room.Dialing.StartPersonalMeeting(ActiveConferenceControl);
		}

		private void ViewOnJoinByIdButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			if (ActiveConferenceControl != null)
				Room.Dialing.Dial(ActiveConferenceControl,
				                  new DialContext {Protocol = eDialProtocol.Zoom, DialString = m_Builder.ToString()});
                     
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