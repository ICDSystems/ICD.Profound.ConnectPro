using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Routing;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Background;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Schedule;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters
{
	[PresenterBinding(typeof(IHelloPresenter))]
	public sealed class HelloPresenter : AbstractTouchDisplayPresenter<IHelloView>, IHelloPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly IConferenceBasePresenter m_ConferencePresenter;
		private readonly ISchedulePresenter m_SchedulePresenter;

		private ICalendarControl m_CalendarControl;
		private bool m_BookingSelected;

		public HelloPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ConferencePresenter = Navigation.LazyLoadPresenter<IConferenceBasePresenter>();
			m_ConferencePresenter.OnViewVisibilityChanged += ConferencePresenterOnViewVisibilityChanged;
			m_SchedulePresenter = Navigation.LazyLoadPresenter<ISchedulePresenter>();
			m_SchedulePresenter.OnRefreshed += SchedulePresenterOnRefreshed;
			m_SchedulePresenter.OnSelectedBookingChanged += SchedulePresenterOnSelectedBookingChanged;
			Navigation.LazyLoadPresenter<IDeviceDrawerPresenter>().OnViewVisibilityChanged +=
				ConferencePresenterOnViewVisibilityChanged;
			Navigation.LazyLoadPresenter<IBackgroundPresenter>().OnViewPreVisibilityChanged += BackgroundOnViewPreVisibilityChanged;
		}

		private void SchedulePresenterOnSelectedBookingChanged(object sender, BookingEventArgs e)
		{
			m_BookingSelected = e.Data != null;
		}

		private void BackgroundOnViewPreVisibilityChanged(object sender, BoolEventArgs e)
		{
			ShowView(e.Data);
		}

		public override void Dispose()
		{
			m_ConferencePresenter.OnViewVisibilityChanged -= ConferencePresenterOnViewVisibilityChanged;
			m_SchedulePresenter.OnRefreshed -= SchedulePresenterOnRefreshed;

			base.Dispose();
		}

		protected override void Refresh(IHelloView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				bool mainPageVisible = Navigation.LazyLoadPresenters<IMainPagePresenter>()
					.Any(p => p.IsViewVisible);
				view.SetMainPageView(!mainPageVisible);

				// Corey Geiser on Slack at 10/10/18 12:00PM -
				// "So the first line we will use is 'Welcome to your meeting.'
				// Second line when a meeting is about to start will say 'Are you here for your meeting? Let's get started.'"

				DateTime now = IcdEnvironment.GetLocalTime();
				IBooking nextBooking = m_CalendarControl != null
					? m_CalendarControl.GetBookings().Where(b => b.EndTime > now)
						.OrderBy(b => b.StartTime).FirstOrDefault()
					: null;

				if (Room == null || Room.Routing.State.GetSourceRoutedStates().Any(s => s.Value != eSourceState.Inactive))
					view.SetLabelText(string.Empty);
				else if (m_ConferencePresenter.IsViewVisible)
					view.SetLabelText("Please use the controls above.");
				else if (Room.IsInMeeting)
					view.SetLabelText("Please choose from the device list above.");
				else if (m_BookingSelected)
					view.SetLabelText("Are you here for this meeting?");
				else if (nextBooking == null || nextBooking.StartTime - TimeSpan.FromMinutes(15) > now)
					view.SetLabelText("Hello.");
				else
					view.SetLabelText("Are you here for your meeting? Let's get started.");
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Room Callbacks

		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			if (m_CalendarControl != null)
				Unsubscribe(m_CalendarControl);

			m_CalendarControl = room == null ? null : room.GetCalendarControls().FirstOrDefault();

			if (m_CalendarControl != null)
				Subscribe(m_CalendarControl);

			Refresh();
		}

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnOnIsInMeetingChanged;
		}

		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnOnIsInMeetingChanged;
		}

		private void RoomOnOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			Refresh();
		}

		private void Subscribe(ICalendarControl control)
		{
			control.OnBookingsChanged += ControlOnBookingsChanged;
		}

		private void Unsubscribe(ICalendarControl control)
		{
			control.OnBookingsChanged -= ControlOnBookingsChanged;
		}

		private void ControlOnBookingsChanged(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Presenter Callbacks
		
		private void ConferencePresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}
		
		private void SchedulePresenterOnRefreshed(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
