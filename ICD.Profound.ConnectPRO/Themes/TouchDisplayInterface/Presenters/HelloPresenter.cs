using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Background;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Schedule;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters
{
	[PresenterBinding(typeof(IHelloPresenter))]
	public sealed class HelloPresenter : AbstractTouchDisplayPresenter<IHelloView>, IHelloPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly IConferenceBasePresenter m_ConferencePresenter;
		private readonly ISchedulePresenter m_SchedulePresenter;

		private ICalendarControl m_CalendarControl;

		public HelloPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ConferencePresenter = Navigation.LazyLoadPresenter<IConferenceBasePresenter>();
			m_ConferencePresenter.OnViewVisibilityChanged += ConferencePresenterOnViewVisibilityChanged;
			m_SchedulePresenter = Navigation.LazyLoadPresenter<ISchedulePresenter>();
			m_SchedulePresenter.OnRefreshed += SchedulePresenterOnRefreshed;
			Navigation.LazyLoadPresenter<IDeviceDrawerPresenter>().OnViewVisibilityChanged +=
				ConferencePresenterOnViewVisibilityChanged;
			Navigation.LazyLoadPresenter<IBackgroundPresenter>().OnViewPreVisibilityChanged += BackgroundOnViewPreVisibilityChanged;
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
					view.SetLabelText("Your conference is about to begin.");
				else if (Room.IsInMeeting)
					view.SetLabelText("Select a source from the Device Drawer to begin.");
				else if (nextBooking == null || nextBooking.StartTime - TimeSpan.FromMinutes(15) > now || Room.IsInMeeting)
					view.SetLabelText("Welcome to your meeting.");
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

			m_CalendarControl = room == null ? null : room.CalendarControl;

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
