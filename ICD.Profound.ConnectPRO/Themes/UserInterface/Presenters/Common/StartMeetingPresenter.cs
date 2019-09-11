using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

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
		private ICalendarControl m_CalendarControl;
		private List<IBooking> m_Bookings;

		private bool HasCalendarControl { get { return m_CalendarControl != null; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public StartMeetingPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSchedulePresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
			m_Bookings = new List<IBooking>();

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
				foreach (IReferencedSchedulePresenter presenter in m_ChildrenFactory.BuildChildren(m_Bookings))
				{
					presenter.SetSelected(presenter == m_SelectedBooking);
					presenter.ShowView(true);
					presenter.Refresh();
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
				// 14 May 2019 1:32p
				string dateTime = string.Format("{0} {1}", Theme.DateFormatting.LongDate, Theme.DateFormatting.ShortTime);

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

			Unsubscribe(m_CalendarControl);
			m_CalendarControl = Room == null ? null : Room.CalendarControl;
			Subscribe(m_CalendarControl);

			if (m_CalendarControl != null)
				m_CalendarControl.Refresh();

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
					m_CalendarControl == null
						? new List<IBooking>()
						: m_CalendarControl.GetBookings()
						                   .Where(b => b.EndTime > IcdEnvironment.GetLocalTime() &&
						                               b.StartTime < IcdEnvironment.GetLocalTime().AddDays(1))
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
				bool started = first.StartTime <= IcdEnvironment.GetLocalTime();
				DateTime nextRefresh = started ? first.EndTime : first.StartTime;
				long delta = (long)(nextRefresh - IcdEnvironment.GetLocalTime()).TotalMilliseconds + 1000;

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
					m_SelectedBooking.SetSelected(false);

				m_SelectedBooking = presenter;

				if (m_SelectedBooking != null)
					m_SelectedBooking.SetSelected(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();

			m_BookingSelectionTimeout.Reset(BOOKING_SELECTION_TIMEOUT);
		}

		private void BookingSelectionTimeout()
		{
			SetSelectedBooking(null);
		}

		#endregion

		#region Calendar Callbacks

		private void Subscribe(ICalendarControl calendarControl)
		{
			if (calendarControl == null)
				return;

			calendarControl.OnBookingsChanged += CalendarControlOnBookingsChanged;
		}

		private void Unsubscribe(ICalendarControl calendarControl)
		{
			if (calendarControl == null)
				return;

			calendarControl.OnBookingsChanged -= CalendarControlOnBookingsChanged;
		}

		private void CalendarControlOnBookingsChanged(object sender, EventArgs eventArgs)
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
			view.OnStartNewMeetingButtonPressed += ViewOnStartNewMeetingButtonPressed;
			view.OnSettingsButtonPressed += ViewOnSettingsButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IStartMeetingView view)
		{
			base.Unsubscribe(view);

			view.OnStartMyMeetingButtonPressed -= ViewOnStartMyMeetingButtonPressed;
			view.OnStartNewMeetingButtonPressed -= ViewOnStartNewMeetingButtonPressed;
			view.OnSettingsButtonPressed -= ViewOnSettingsButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the settings button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSettingsButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.LazyLoadPresenter<IPasscodePresenter>().ShowView(PasscodeSuccessCallback);
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

			if (!HasCalendarControl || m_SelectedBooking == null || m_SelectedBooking.Booking == null)
			{
				Room.StartMeeting();
				return;
			}

			IBooking booking = m_SelectedBooking.Booking;
			m_SelectedBooking = null;

			Room.StartMeeting(booking);
		}

		private void ViewOnStartNewMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Room.StartMeeting();
		}

		/// <summary>
		/// Called when the user successfully enters the passcode.
		/// </summary>
		/// <param name="sender"></param>
		private void PasscodeSuccessCallback(IPasscodePresenter sender)
		{
			Navigation.LazyLoadPresenter<IPasscodePresenter>().ShowView(false);

			Navigation.NavigateTo<ISettingsBasePresenter>();
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
