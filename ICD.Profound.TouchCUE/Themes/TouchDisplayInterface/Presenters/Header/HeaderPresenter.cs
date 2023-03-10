using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.Shared.Models;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Background;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Notifications;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Schedule;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Header
{
	[PresenterBinding(typeof(IHeaderPresenter))]
	public sealed class HeaderPresenter : AbstractTouchDisplayPresenter<IHeaderView>, IHeaderPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_RefreshTimer;
		private bool m_Refreshing;

		private readonly ReferencedHeaderButtonPresenterFactory m_LeftButtonsFactory;
		private readonly ReferencedHeaderButtonPresenterFactory m_RightButtonsFactory;

		private readonly IcdSortedDictionary<HeaderButtonModel, object> m_LeftButtons;
		private readonly IcdSortedDictionary<HeaderButtonModel, object> m_RightButtons;

		private readonly HeaderButtonModel m_SettingsButton;
		private readonly HeaderButtonModel m_EndMeetingButton;

		private readonly IDeviceDrawerPresenter m_DeviceDrawerPresenter;
		private readonly ISchedulePresenter m_SchedulePresenter;
		private readonly IBackgroundPresenter m_BackgroundPresenter;

		private VibeBoardAppControl m_SubscribedAppControl;

		private bool m_BookingSelected;
		private bool m_Collapsed;

		public bool Collapsed
		{
			get { return m_Collapsed; }
			private set
			{
				if (m_Collapsed == value)
					return;

				m_Collapsed = value;

				var conferenceBase = Navigation.LazyLoadPresenter<IConferenceBasePresenter>();
				if (m_Collapsed)
				{
					Navigation.LazyLoadPresenter<IDeviceDrawerPresenter>().ShowView(false);
					Navigation.LazyLoadPresenter<IVolumePresenter>().ShowView(false);
					conferenceBase.ShowView(false);
				}
				else if (conferenceBase.ActiveConferenceControl != null && conferenceBase.ActiveConferenceControl.GetActiveConferences().Any())
					conferenceBase.ShowView(true);

				Refresh();
			}
		}

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public HeaderPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			// Refresh every second to update the time
			m_RefreshTimer = new SafeTimer(RefreshTime, 1000, 1000);

			m_LeftButtonsFactory = new ReferencedHeaderButtonPresenterFactory(nav, LeftButtonsViewFactory, EmptySub, EmptyUnsub);
			m_RightButtonsFactory = new ReferencedHeaderButtonPresenterFactory(nav, RightButtonsViewFactory, EmptySub, EmptyUnsub);

			m_LeftButtons = new IcdSortedDictionary<HeaderButtonModel, object>();
			m_RightButtons = new IcdSortedDictionary<HeaderButtonModel, object>();

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

			m_BackgroundPresenter = Navigation.LazyLoadPresenter<IBackgroundPresenter>();
			Subscribe(m_BackgroundPresenter);
			
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
			Unsubscribe(m_BackgroundPresenter);

			base.Dispose();
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
				m_Refreshing = true;

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

				view.SetCollapsed(Collapsed);

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
				m_Refreshing = false;
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

			if (m_Refreshing)
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

		#region Button Methods

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

		private IEnumerable<IReferencedHeaderButtonView> LeftButtonsViewFactory(ushort count)
		{
			return GetView().GetLeftButtonViews(ViewFactory as ITouchDisplayViewFactory, count);
		}

		private IEnumerable<IReferencedHeaderButtonView> RightButtonsViewFactory(ushort count)
		{
			return GetView().GetRightButtonViews(ViewFactory as ITouchDisplayViewFactory, count);
		}

		#endregion

		#region Room Callbacks

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
			UpdateButtons();

			VibeBoard vibeBoard = room.Originators.GetInstanceRecursive<VibeBoard>();
			m_SubscribedAppControl = vibeBoard == null ? null : vibeBoard.Controls.GetControl<VibeBoardAppControl>();
			if (m_SubscribedAppControl != null)
				m_SubscribedAppControl.OnAppLaunched += SubscribedAppControlOnOnAppLaunched;
		}

		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;

			if (m_SubscribedAppControl != null)
				m_SubscribedAppControl.OnAppLaunched -= SubscribedAppControlOnOnAppLaunched;
			m_SubscribedAppControl = null;
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

		private void SubscribedAppControlOnOnAppLaunched(object sender, EventArgs e)
		{
			Collapsed = true;
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
			view.OnCollapseButtonPressed += ViewOnCollapseButtonPressed;
		}

		protected override void Unsubscribe(IHeaderView view)
		{
			base.Unsubscribe(view);

			view.OnCenterButtonPressed -= ViewOnStartEndMeetingPressed;
			view.OnCollapseButtonPressed -= ViewOnCollapseButtonPressed;
		}

		private void ViewOnStartEndMeetingPressed(object sender, EventArgs e)
		{
			if (!Room.IsInMeeting)
				Room.StartMeeting(null, null);
			else if (Room.ConferenceManager.Dialers.IsInCall != eInCall.None)
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
			if (Room != null)
			{
				foreach (var activeConference in Room.ConferenceManager.Dialers.ActiveConferences)
				{
					if (activeConference.SupportsLeaveOrEnd())
						activeConference.LeaveOrEndConference();
				}
			}

			Navigation.LazyLoadPresenter<IConfirmEndMeetingPresenter>().ShowView(false);
			Navigation.NavigateTo<IDeviceDrawerPresenter>();
		}
		
		private void ViewOnCollapseButtonPressed(object sender, EventArgs e)
		{
			Collapsed = !Collapsed;
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
			if (schedule == null)
				return;

			schedule.OnSelectedBookingChanged += ScheduleOnSelectedBookingChanged;
		}

		private void Unsubscribe(ISchedulePresenter schedule)
		{
			if (schedule == null)
				return;

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

		#region Background Callbacks

		private void Subscribe(IBackgroundPresenter background)
		{
			if (background == null)
				return;

			background.OnViewVisibilityChanged += BackgroundOnOnViewVisibilityChanged;
		}

		private void Unsubscribe(IBackgroundPresenter background)
		{
			if (background == null)
				return;

			background.OnViewVisibilityChanged -= BackgroundOnOnViewVisibilityChanged;
		}

		private void BackgroundOnOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			if (e.Data)
				Collapsed = false;
		}

		#endregion
	}
}