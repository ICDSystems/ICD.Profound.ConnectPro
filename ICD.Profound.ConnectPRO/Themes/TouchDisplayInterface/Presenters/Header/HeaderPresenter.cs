using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.Shared.Models;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Notifications;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Schedule;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Settings;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Header
{
	[PresenterBinding(typeof(IHeaderPresenter))]
	public sealed class HeaderPresenter : AbstractTouchDisplayPresenter<IHeaderView>, IHeaderPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_RefreshTimer;

		private readonly ReferencedHeaderButtonPresenterFactory m_LeftButtonsFactory;
		private readonly ReferencedHeaderButtonPresenterFactory m_RightButtonsFactory;

		private readonly IcdOrderedDictionary<HeaderButtonModel, object> m_LeftButtons;
		private readonly IcdOrderedDictionary<HeaderButtonModel, object> m_RightButtons;

		private readonly HeaderButtonModel m_SettingsButton;
		private readonly HeaderButtonModel m_EndMeetingButton;

		private IDeviceDrawerPresenter m_DeviceDrawerPresenter;
		private ISchedulePresenter m_SchedulePresenter;
		private bool m_BookingSelected;
		
		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public HeaderPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			// Refresh every second to update the time
			m_RefreshTimer = new SafeTimer(RefreshTime, 1000, 1000);

			m_LeftButtonsFactory = new ReferencedHeaderButtonPresenterFactory(nav, LeftButtonsViewFactory, EmptySub, EmptyUnsub);
			m_RightButtonsFactory = new ReferencedHeaderButtonPresenterFactory(nav, RightButtonsViewFactory, EmptySub, EmptyUnsub);

			m_LeftButtons = new IcdOrderedDictionary<HeaderButtonModel, object>();
			m_RightButtons = new IcdOrderedDictionary<HeaderButtonModel, object>();

			m_SettingsButton = new HeaderButtonModel(0, 0, OpenSettings)
			{
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.Settings, eTouchCueColor.White),
				LabelText = "Settings",
				Mode = eHeaderButtonMode.Blue
			};
			m_EndMeetingButton = new HeaderButtonModel(0, 1, ConfirmEndMeeting)
			{
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.Close, eTouchCueColor.White),
				LabelText = "End Meeting",
				Mode = eHeaderButtonMode.Red
			};

			m_DeviceDrawerPresenter = Navigation.LazyLoadPresenter<IDeviceDrawerPresenter>();
			Subscribe(m_DeviceDrawerPresenter);

			m_SchedulePresenter = Navigation.LazyLoadPresenter<ISchedulePresenter>();
			Subscribe(m_SchedulePresenter);
			
			theme.DateFormatting.OnFormatChanged += DateFormattingOnFormatChanged;
		}

		/// <summary>
		///     Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_RefreshTimer.Dispose();

			Unsubscribe(m_DeviceDrawerPresenter);
			Unsubscribe(m_SchedulePresenter);

			base.Dispose();
		}

		public bool ContainsLeftButton(HeaderButtonModel button)
		{
			return m_LeftButtons.ContainsKey(button);
		}

		public void AddLeftButton(HeaderButtonModel button)
		{
			if (ContainsLeftButton(button))
				return;

			m_LeftButtons.Add(button, null);
		}

		public void RemoveLeftButton(HeaderButtonModel button)
		{
			if (!ContainsLeftButton(button))
				return;

			m_LeftButtons.Remove(button);
		}

		public bool ContainsRightButton(HeaderButtonModel button)
		{
			return m_RightButtons.ContainsKey(button);
		}
		
		public void AddRightButton(HeaderButtonModel button)
		{
			if (ContainsRightButton(button))
				return;

			m_RightButtons.Add(button, null);
		}

		public void RemoveRightButton(HeaderButtonModel button)
		{
			if (!ContainsRightButton(button))
				return;

			m_RightButtons.Remove(button);
		}

		/// <summary>
		///     Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IHeaderView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				var roomName = Room == null ? string.Empty : Room.Name;
				view.SetRoomName(roomName);
				
				view.SetCenterButtonMode(Room != null && Room.IsInMeeting ? eCenterButtonMode.DeviceDrawer : eCenterButtonMode.InstantMeeting);
				view.SetCenterButtonSelected(Navigation.LazyLoadPresenter<IDeviceDrawerPresenter>().IsViewVisible);
				view.SetCenterButtonEnabled(!m_BookingSelected);
				string text = Room != null && Room.IsInMeeting
					? "Device Drawer"
					: "Instant Meeting";
				view.SetCenterButtonText(text);

				RefreshTime();

				foreach (IReferencedHeaderButtonPresenter button in m_LeftButtonsFactory.BuildChildren(m_LeftButtons.Keys))
				{
					button.ShowView(true);
					button.Refresh();
				}
				foreach (IReferencedHeaderButtonPresenter button in m_RightButtonsFactory.BuildChildren(m_RightButtons.Keys))
				{
					button.ShowView(true);
					button.Refresh();
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		///     Updates the time label on the header.
		/// </summary>
		private void RefreshTime()
		{
			var view = GetView();
			if (view == null)
				return;

			if (!m_RefreshSection.TryEnter())
				return;

			try
			{
				view.SetTimeLabel(Theme.DateFormatting.ShortTime);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private IEnumerable<IReferencedHeaderButtonView> LeftButtonsViewFactory(ushort count)
		{
			return GetView().GetLeftButtonViews(ViewFactory as ITouchDisplayViewFactory, count);
		}

		private IEnumerable<IReferencedHeaderButtonView> RightButtonsViewFactory(ushort count)
		{
			return GetView().GetRightButtonViews(ViewFactory as ITouchDisplayViewFactory, count);
		}

		#region Room Callbacks

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
			UpdateButtons();
		}

		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			UpdateButtons();
		}

		private void UpdateButtons()
		{
			RemoveLeftButton(m_SettingsButton);
			RemoveLeftButton(m_EndMeetingButton);

			if (Room != null)
				AddLeftButton(Room.IsInMeeting ? m_EndMeetingButton : m_SettingsButton);

			Refresh();
		}

		#endregion

		#region Theme Callbacks

		private void DateFormattingOnFormatChanged(object sender, EventArgs e)
		{
			RefreshTime();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IHeaderView view)
		{
			base.Subscribe(view);

			view.OnCenterButtonPressed += ViewOnStartEndMeetingPressed;
		}

		protected override void Unsubscribe(IHeaderView view)
		{
			base.Unsubscribe(view);

			view.OnCenterButtonPressed -= ViewOnStartEndMeetingPressed;
		}

		private void ViewOnStartEndMeetingPressed(object sender, EventArgs e)
		{
			if (!Room.IsInMeeting)
				Room.StartMeeting();
			else if (Room.ConferenceManager.IsInCall != eInCall.None)
				Navigation.LazyLoadPresenter<IConfirmEndMeetingPresenter>()
					.Show("Are you sure you would like to leave your conference?", EndCall);
			else
			{
				var deviceDrawer = Navigation.LazyLoadPresenter<IDeviceDrawerPresenter>();
				deviceDrawer.ShowView(!deviceDrawer.IsViewVisible);
			}
		}

		private void EndCall()
		{
			if (Room != null && Room.FocusSource != null)
			{
				var device = Room.Core.Originators.GetChild<IDevice>(Room.FocusSource.Device);
				var conferenceControl = device == null ? null : device.Controls.GetControl<IConferenceDeviceControl>();
				var activeConference = conferenceControl == null ? null : conferenceControl.GetActiveConference();
				if (activeConference is IWebConference)
					(activeConference as IWebConference).LeaveConference();
				if (activeConference is ITraditionalConference)
					(activeConference as ITraditionalConference).Hangup();
			}

			Navigation.LazyLoadPresenter<IConfirmEndMeetingPresenter>().ShowView(false);
			Navigation.NavigateTo<IDeviceDrawerPresenter>();
		}

		#endregion

		#region Child Callbacks

		private void EmptySub(IReferencedHeaderButtonPresenter presenter)
		{
		}

		private void EmptyUnsub(IReferencedHeaderButtonPresenter presenter)
		{
		}

		#endregion

		#region Header Button Callbacks

		private void OpenSettings()
		{
			Navigation.NavigateTo<ISettingsBasePresenter>();
		}

		private void ConfirmEndMeeting()
		{
			Navigation.LazyLoadPresenter<IConfirmEndMeetingPresenter>()
				.Show("Are you sure you would like to end your meeting?", EndMeeting);
		}

		private void EndMeeting()
		{
			if (Room == null)
				return;

			Room.EndMeeting();
		}

		#endregion

		#region Device Drawer Callbacks

		private void Subscribe(IDeviceDrawerPresenter deviceDrawer)
		{
			deviceDrawer.OnViewVisibilityChanged += DeviceDrawerOnViewVisibilityChanged;
		}

		private void Unsubscribe(IDeviceDrawerPresenter deviceDrawer)
		{
			deviceDrawer.OnViewVisibilityChanged -= DeviceDrawerOnViewVisibilityChanged;
		}

		private void DeviceDrawerOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			Refresh();
		}

		#endregion

		#region Schedule Callbacks

		private void Subscribe(ISchedulePresenter schedule)
		{
			schedule.OnSelectedBookingChanged += ScheduleOnSelectedBookingChanged;
		}

		private void Unsubscribe(ISchedulePresenter schedule)
		{
			schedule.OnSelectedBookingChanged -= ScheduleOnSelectedBookingChanged;
		}

		private void ScheduleOnSelectedBookingChanged(object sender, BookingEventArgs e)
		{
			if (e.Data == null || e.Data is EmptyBooking)
				m_BookingSelected = false;
			else
				m_BookingSelected = true;
			Refresh();
		}

		#endregion
	}
}