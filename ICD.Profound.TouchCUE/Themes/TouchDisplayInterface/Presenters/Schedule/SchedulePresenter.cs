using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.Shared.Models;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Schedule;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Schedule
{
	[PresenterBinding(typeof(ISchedulePresenter))]
	public sealed class SchedulePresenter : AbstractTouchDisplayPresenter<IScheduleView>, ISchedulePresenter
	{
		public event EventHandler OnRefreshed;
		public event EventHandler<BookingEventArgs> OnSelectedBookingChanged;

		private const int DEFAULT_CACHE_TIME = 15 * 60 * 1000;
		private readonly ReferencedBookingPresenterFactory m_ChildrenFactory;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly List<IBooking> m_CachedBookings;
		private readonly SafeTimer m_CacheTimer;

		private ICalendarControl m_CalendarControl;
		private IReferencedBookingPresenter m_SelectedBooking;

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SchedulePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			TouchCueTheme theme) :
			base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedBookingPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);

			m_CachedBookings = new List<IBooking>();
			m_CacheTimer = new SafeTimer(CacheBookings, DEFAULT_CACHE_TIME);
			theme.DateFormatting.OnFormatChanged += DateFormattingOnFormatChanged;
		}

		private IReferencedBookingPresenter SelectedBooking
		{
			get { return m_SelectedBooking; }
			set
			{
				if (m_SelectedBooking == value)
					return;

				m_SelectedBooking = value;

				OnSelectedBookingChanged.Raise(this, new BookingEventArgs(m_SelectedBooking == null ? null : m_SelectedBooking.Booking));
			}
		}

		protected override void Refresh(IScheduleView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				if (m_CachedBookings.Any())
					view.ShowSchedule(true);
				else
					view.ShowSchedule(false);

				var roomName = Room == null ? string.Empty : Room.Name;

				// build presenters
				foreach (var presenter in m_ChildrenFactory.BuildChildren(m_CachedBookings.Skip(1)))
				{
					presenter.ShowView(true);
					presenter.SetSelected(presenter == SelectedBooking);
					presenter.Refresh();
				}

				// display current room status
				IBooking currentBooking = 
					SelectedBooking != null && SelectedBooking.Booking != null 
						? SelectedBooking.Booking 
						: m_CachedBookings.FirstOrDefault();
				
				if (currentBooking == null || currentBooking is EmptyBooking)
				{
					view.SetCurrentBookingIcon(TouchCueIcons.GetIcon(eTouchCueIcon.ThumbsUp, eTouchCueColor.White));
					view.SetAvailabilityText("AVAILABLE");
					view.SetAvailabilityVisible(true);
					view.SetColorMode(eScheduleViewColorMode.Blue);
					view.SetCurrentBookingSubject(roomName);
					view.SetCloseButtonVisible(false);
					view.SetStartBookingButtonVisible(false);

					if (currentBooking != null)
						view.SetCurrentBookingTime(string.Format("Now - {0}", FormatTime(currentBooking.EndTime)));
					else
						view.SetCurrentBookingTime("");
				}
				else
				{
					view.SetCurrentBookingIcon(GetBookingIcon(currentBooking));

					view.SetCurrentBookingSubject(currentBooking.IsPrivate
						? "Private Meeting"
						: currentBooking.MeetingName);
					view.SetCurrentBookingTime(string.Format("{0} - {1}",
						FormatTime(currentBooking.StartTime),
						FormatTime(currentBooking.EndTime)));
					view.SetAvailabilityVisible(false);
					view.SetStartBookingButtonVisible(true);

					if (SelectedBooking != null && SelectedBooking.Booking == currentBooking)
					{
						view.SetColorMode(eScheduleViewColorMode.Green);
						view.SetCloseButtonVisible(true);
						view.SetStartBookingButtonSelected(false);
						view.SetStartBookingButtonText("START THIS MEETING EARLY");
					}
					else
					{
						view.SetColorMode(eScheduleViewColorMode.Red);
						view.SetCloseButtonVisible(false);
						view.SetStartBookingButtonSelected(true);
						view.SetStartBookingButtonText("START RESERVED MEETING");
					}
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			OnRefreshed.Raise(this);
		}

		private void CacheBookings()
		{
			// clear current contents
			m_CachedBookings.Clear();

			if (m_CalendarControl == null)
				return;

			var now = IcdEnvironment.GetLocalTime();
			var tomorrow = now.AddDays(1);

			var bookings =
				m_CalendarControl.GetBookings()
					.Where(b => b.EndTime > now && b.StartTime < tomorrow)
					.OrderBy(b => b.StartTime)
					.ToList();
			
			var firstBooking = bookings.FirstOrDefault();
			// find out if room is currently available
			if (firstBooking != null && firstBooking.StartTime - now > TimeSpan.FromMinutes(15))
				m_CachedBookings.Add(new EmptyBooking
				{
					StartTime = DateTime.MinValue,
					EndTime = firstBooking.StartTime
				});
			// build list of bookings and available times
			for (var i = 0; i < bookings.Count; i++)
			{
				if (bookings[i] == null)
					continue;

				m_CachedBookings.Add(bookings[i]);

				if (i + 1 >= bookings.Count)
					m_CachedBookings.Add(new EmptyBooking
					{
						StartTime = bookings[i].EndTime,
						EndTime = DateTime.MaxValue
					});

				// calculate availability between this booking and next
				else if (bookings[i + 1].StartTime - bookings[i].EndTime >= TimeSpan.FromMinutes(30))
					m_CachedBookings.Add(new EmptyBooking
					{
						StartTime = bookings[i].EndTime,
						EndTime = bookings[i + 1].StartTime
					});
			}

			IBooking currentBooking = m_CachedBookings.FirstOrDefault();
			if (currentBooking == null)
				m_CacheTimer.Reset(DEFAULT_CACHE_TIME);
			else if (currentBooking is EmptyBooking)
				m_CacheTimer.Reset((long) (currentBooking.EndTime - now - TimeSpan.FromMinutes(15)).TotalMilliseconds + 1000);
			else
				m_CacheTimer.Reset((long) (currentBooking.EndTime - now).TotalMilliseconds + 1000);

			SelectedBooking = null;
			Refresh();
		}

		private string GetBookingIcon(IBooking booking)
		{
			var dialers =
				Room == null
					? Enumerable.Empty<IConferenceDeviceControl>()
					: Room.GetControlsRecursive<IConferenceDeviceControl>();

			eTouchCueIcon icon = eTouchCueIcon.Display;
			switch (ConferencingBookingUtils.GetMeetingType(booking, dialers))
			{
				case eMeetingType.VideoConference:
					icon = eTouchCueIcon.VideoConference;
					break;
				case eMeetingType.AudioConference:
					icon = eTouchCueIcon.AudioConference;
					break;
			}

			return TouchCueIcons.GetIcon(icon, eTouchCueColor.White);
		}

		#region View Callbacks

		protected override void Subscribe(IScheduleView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
			view.OnStartBookingButtonPressed += ViewOnStartBookingPressed;
		}

		protected override void Unsubscribe(IScheduleView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
			view.OnStartBookingButtonPressed -= ViewOnStartBookingPressed;
		}

		private void ViewOnStartBookingPressed(object sender, EventArgs e)
		{
			if (SelectedBooking != null && SelectedBooking.Booking != null)
			{
				Room.StartMeeting(SelectedBooking.Booking);
				return;
			}

			var currentBooking = m_CachedBookings.FirstOrDefault();
			if (currentBooking != null && !(currentBooking is EmptyBooking))
				Room.StartMeeting(currentBooking);
		}

		private void ViewOnCloseButtonPressed(object sender, EventArgs e)
		{
			SelectedBooking = null;
			RefreshIfVisible();
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			SelectedBooking = null;
		}

		#endregion

		#region Private Methods

		private IEnumerable<IReferencedBookingView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory as ITouchDisplayViewFactory, count);
		}

		private string FormatTime(DateTime time)
		{
			return Theme.DateFormatting.GetShortTime(time);
		}

		#endregion

		#region Room Callbacks

		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			if (m_CalendarControl != null)
				Unsubscribe(m_CalendarControl);

			m_CalendarControl = room == null ? null : room.GetCalendarControls().FirstOrDefault();

			if (m_CalendarControl != null)
				Subscribe(m_CalendarControl);

			CacheBookings();
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
			CacheBookings();
		}

		#endregion

		#region Theme Callbacks

		private void DateFormattingOnFormatChanged(object sender, EventArgs eventArgs)
		{
			Refresh();
		}

		#endregion

		#region Child Callbacks

		private void Subscribe(IReferencedBookingPresenter presenter)
		{
			presenter.OnBookingPressed += PresenterOnBookingPressed;
		}

		private void Unsubscribe(IReferencedBookingPresenter presenter)
		{
			presenter.OnBookingPressed -= PresenterOnBookingPressed;
		}

		private void PresenterOnBookingPressed(object sender, EventArgs e)
		{
			var presenter = sender as IReferencedBookingPresenter;
			if (presenter == null)
				return;

			SelectedBooking = SelectedBooking == presenter ? null : presenter;
			RefreshIfVisible();
		}

		#endregion
	}
}