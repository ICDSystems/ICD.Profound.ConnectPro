using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Calendaring.CalendarManagers;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Calendaring.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPROCommon.SettingsTree;
using ICD.Profound.ConnectPROCommon.SettingsTree.RoomCombine;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	[PresenterBinding(typeof(IStartMeetingPresenter))]
	public sealed class StartMeetingPresenter : AbstractUiPresenter<IStartMeetingView>, IStartMeetingPresenter
	{
		private const string NO_MEETING_LABEL_TEXT = "No Meetings Scheduled at this Time";

		private const long BOOKING_SELECTION_TIMEOUT = 8 * 1000;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedSchedulePresenterFactory m_ChildrenFactory;
		private readonly SafeTimer m_BookingsRefreshTimer;
		private readonly SafeTimer m_TimeRefreshTimer;
		private readonly SafeTimer m_BookingSelectionTimeout;

		[CanBeNull]
		private IReferencedSchedulePresenter m_SelectedBooking;
		private ICalendarManager m_CalendarManager;
		private List<BookingGroup> m_Bookings;

		private bool HasCalendarControl
		{
			get { return m_CalendarManager != null && m_CalendarManager.GetProviders(eCalendarFeatures.ListBookings).Any(); }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public StartMeetingPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSchedulePresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
			m_Bookings = new List<BookingGroup>();

			m_BookingsRefreshTimer = SafeTimer.Stopped(UpdateBookings);
			m_TimeRefreshTimer = new SafeTimer(RefreshTime, 1000, 1000);
			m_BookingSelectionTimeout = SafeTimer.Stopped(BookingSelectionTimeout);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_BookingsRefreshTimer.Dispose();
			m_TimeRefreshTimer.Dispose();
			m_BookingSelectionTimeout.Dispose();

			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IStartMeetingView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				foreach (IReferencedSchedulePresenter presenter in m_ChildrenFactory.BuildChildren(m_Bookings.Cast<IBooking>()))
				{
					presenter.Selected = presenter == m_SelectedBooking;
					presenter.ShowView(true);
				}

				view.SetLogoPath(Theme.LogoAbsolutePath);

				view.SetStartMyMeetingButtonEnabled(!HasCalendarControl || m_SelectedBooking != null);
				view.SetStartNewMeetingButtonEnabled(HasCalendarControl);

				if (HasCalendarControl && m_Bookings.Count < 1)
				{
					view.SetNoMeetingsButtonEnabled(true);
					view.SetNoMeetingsLabel(NO_MEETING_LABEL_TEXT);
				}

				view.SetBookingsVisible(HasCalendarControl, m_Bookings.Count);

				// Room combine button
				IRootSettingsNode rootSettings = Navigation.LazyLoadPresenter<ISettingsBasePresenter>().RootNode;
				RoomCombineSettingsNode roomCombineSettings =
					rootSettings == null
						? null
						: rootSettings.GetChildren()
						              .OfType<RoomCombineSettingsNode>()
						              .First();
				bool roomCombineAvailable = roomCombineSettings != null && roomCombineSettings.Visible;
				view.SetRoomCombineButtonVisible(roomCombineAvailable);

				RefreshTime();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Updates the time label on the splash page.
		/// </summary>
		private void RefreshTime()
		{
			IStartMeetingView view = GetView();
			if (view == null)
				return;

			if (!m_RefreshSection.TryEnter())
				return;

			try
			{
				// 5/14/2019 1:32p
				string dateTime = string.Format("{0} {1}", Theme.DateFormatting.ShortDate, Theme.DateFormatting.ShortTime);

				view.SetSplashTimeLabel(dateTime);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			Unsubscribe(m_CalendarManager);
			m_CalendarManager = Room == null ? null: Room.CalendarManager;
			Subscribe(m_CalendarManager);

			UpdateBookings();

			RefreshIfVisible();
		}

		#region Private Methods

		private IEnumerable<IReferencedScheduleView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		private void UpdateBookings()
		{
			IBooking first;

			m_RefreshSection.Enter();

			try
			{
				m_Bookings =
					m_CalendarManager == null
						? new List<BookingGroup>()
						: m_CalendarManager.GetBookings()
						                   .Where(b =>
						                          CalendarUtils.IsInRange(b, IcdEnvironment.GetUtcTime(),
						                                                  IcdEnvironment.GetUtcTime().AddDays(1)))
						                   .ToList();

				first = m_Bookings.FirstOrDefault();
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			// Refresh when the next meeting starts or the current meeting ends.
			if (first != null)
			{
				bool started = first.StartTime <= IcdEnvironment.GetUtcTime();
				DateTime nextRefresh = started ? first.EndTime : first.StartTime;
				long delta = (long)(nextRefresh - IcdEnvironment.GetUtcTime()).TotalMilliseconds + 1000;

				if (delta > 0)
					m_BookingsRefreshTimer.Reset(delta);
			}

			RefreshIfVisible();
		}

		private void SetSelectedBooking(IReferencedSchedulePresenter presenter)
		{
			m_RefreshSection.Enter();

			try
			{
				if (presenter == m_SelectedBooking)
					return;

				if (m_SelectedBooking != null)
					m_SelectedBooking.Selected = false;

				m_SelectedBooking = presenter;

				if (m_SelectedBooking != null)
					m_SelectedBooking.Selected = true;
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			m_BookingSelectionTimeout.Reset(BOOKING_SELECTION_TIMEOUT);

			RefreshIfVisible();
		}

		private void BookingSelectionTimeout()
		{
			SetSelectedBooking(null);
		}

		#endregion

		#region Calendar Callbacks

		/// <summary>
		/// Subscribe to the calendar manager events.
		/// </summary>
		/// <param name="calendarManager"></param>
		private void Subscribe(ICalendarManager calendarManager)
		{
			if (calendarManager == null)
				return;

			calendarManager.OnBookingsChanged += CalendarManagerOnBookingsChanged;
		}

		/// <summary>
		/// Unsubscribe from the calendar manager events.
		/// </summary>
		/// <param name="calendarManager"></param>
		private void Unsubscribe(ICalendarManager calendarManager)
		{
			if (calendarManager == null)
				return;

			calendarManager.OnBookingsChanged -= CalendarManagerOnBookingsChanged;
		}

		private void CalendarManagerOnBookingsChanged(object sender, EventArgs eventArgs)
		{
			UpdateBookings();
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Subscribe to the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Subscribe(IReferencedSchedulePresenter child)
		{
			if (child == null)
				return;

			child.OnPressed += ChildOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Unsubscribe(IReferencedSchedulePresenter child)
		{
			if (child == null)
				return;

			child.OnPressed -= ChildOnPressed;
		}

		/// <summary>
		/// Called when the user presses the child source.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnPressed(object sender, EventArgs eventArgs)
		{
			IReferencedSchedulePresenter newBooking = sender as IReferencedSchedulePresenter;
			if (newBooking == null)
				return;

			m_RefreshSection.Enter();

			try
			{
				SetSelectedBooking(newBooking == m_SelectedBooking ? null : newBooking);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IStartMeetingView view)
		{
			base.Subscribe(view);

			view.OnStartMyMeetingButtonPressed += ViewOnStartMyMeetingButtonPressed;
			view.OnInstantMeetingButtonPressed += ViewOnInstantMeetingButtonPressed;
			view.OnSettingsButtonPressed += ViewOnSettingsButtonPressed;
			view.OnRoomCombineButtonPressed += ViewOnRoomCombineButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IStartMeetingView view)
		{
			base.Unsubscribe(view);

			view.OnStartMyMeetingButtonPressed -= ViewOnStartMyMeetingButtonPressed;
			view.OnInstantMeetingButtonPressed -= ViewOnInstantMeetingButtonPressed;
			view.OnSettingsButtonPressed -= ViewOnSettingsButtonPressed;
			view.OnRoomCombineButtonPressed -= ViewOnRoomCombineButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the room combine button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnRoomCombineButtonPressed(object sender, EventArgs eventArgs)
		{
			ISettingsBasePresenter settingsBase = Navigation.NavigateTo<ISettingsBasePresenter>();
			IRootSettingsNode root = settingsBase.RootNode;
			if (root == null)
				return;

			RoomCombineSettingsNode roomCombine =
				root.GetChildren()
				    .OfType<RoomCombineSettingsNode>()
				    .First();

			settingsBase.NavigateTo(roomCombine);
		}

		/// <summary>
		/// Called when the user presses the settings button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSettingsButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.NavigateTo<ISettingsBasePresenter>();
		}

		/// <summary>
		/// Called when the user presses the start meeting button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnStartMyMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			// If the room doesn't have a calendar control then start the meeting
			if (!HasCalendarControl)
			{
				Room.StartMeeting(null, null);
				return;
			}

			// Otherwise, the user needs to have selected a booking before starting the meeting
			IBooking booking = m_SelectedBooking == null ? null : m_SelectedBooking.Booking;
			m_SelectedBooking = null;

			if (booking != null)
				Room.StartMeeting(booking, null);
		}

		/// <summary>
		/// Called when the user presses the instant meeting button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnInstantMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Room.StartMeeting(null, null);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			// Clear the selection when we navigate away
			m_SelectedBooking = null;
		}

		#endregion
	}
}
