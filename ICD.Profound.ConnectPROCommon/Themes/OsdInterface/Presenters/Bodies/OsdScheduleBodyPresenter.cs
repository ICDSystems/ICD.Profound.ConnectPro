using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Calendaring.CalendarManagers;
using ICD.Connect.Calendaring.Utils;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Bodies;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.FooterNotifications;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Bodies;
using ICD.Profound.ConnectPROCommon.Themes.Shared.Models;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Bodies
{
	[PresenterBinding(typeof(IOsdScheduleBodyPresenter))]
	public sealed class OsdScheduleBodyPresenter : AbstractOsdPresenter<IOsdScheduleBodyView>, IOsdScheduleBodyPresenter
	{
		private const int DEFAULT_REFRESH_TIME = 15 * 60 * 1000;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedSchedulePresenterFactory m_ChildrenFactory;
		private readonly SafeTimer m_RefreshTimer;

		[CanBeNull] private ICalendarManager m_SubscribedCalendarManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdScheduleBodyPresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSchedulePresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);

			m_RefreshTimer = SafeTimer.Stopped(RefreshIfVisible);

			theme.DateFormatting.OnFormatChanged += DateFormattingOnFormatChanged;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IOsdScheduleBodyView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IBooking[] upcomingBookingsAndAvailability = GetUpcomingBookingsAndAvailability().ToArray();

				// Build booking presenters
				foreach (IReferencedSchedulePresenter presenter in
					m_ChildrenFactory.BuildChildren(upcomingBookingsAndAvailability.Skip(1)))
					presenter.ShowView(true);

				// Display current room status
				IBooking currentBooking = upcomingBookingsAndAvailability.FirstOrDefault();
				bool isCurrentlyAvailable = currentBooking is EmptyBooking;

				long refreshInterval = DEFAULT_REFRESH_TIME;
				string currentIcon = "thumbsUp";
				string currentSubject = Room == null ? string.Empty : Room.Name;
				string currentTime = " ";
				string currentAvailability = "AVAILABLE";

				if (currentBooking == null)
				{
					Navigation.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>().ClearMessages("Meeting");
				}
				else if (isCurrentlyAvailable)
				{
					currentTime = string.Format("Now - {0}", FormatTime(currentBooking.EndTime));
					refreshInterval = (long)(currentBooking.EndTime -
					                         IcdEnvironment.GetUtcTime() -
					                         TimeSpan.FromMinutes(15)).TotalMilliseconds + 1000;

					if (currentBooking.EndTime - IcdEnvironment.GetUtcTime() < TimeSpan.FromMinutes(15))
						Navigation.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>()
						          .PushMessage("Meeting", "There is a meeting about to begin in this room");
				}
				else
				{
					bool allDay = (currentBooking.EndTime - currentBooking.StartTime).TotalHours >= 23;

					currentIcon = GetBookingIcon(currentBooking);
					currentSubject = currentBooking.IsPrivate ? "Private Meeting" : currentBooking.MeetingName;
					currentTime = allDay
						              ? "All Day"
						              : string.Format("{0} - {1}", FormatTime(currentBooking.StartTime),
						                              FormatTime(currentBooking.EndTime));
					currentAvailability = "RESERVED";
					refreshInterval =
						(long)(currentBooking.EndTime - IcdEnvironment.GetUtcTime()).TotalMilliseconds +
						1000;

					Navigation.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>().ClearMessages("Meeting");
				}

				view.SetCurrentBookingIcon(currentIcon);
				view.SetCurrentBookingSubject(currentSubject);
				view.SetCurrentBookingTime(currentTime);
				view.SetAvailabilityText(currentAvailability);

				// Refresh again when a booking expires
				if (Room != null)
					m_RefreshTimer.Reset(refreshInterval);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private IEnumerable<IBooking> GetUpcomingBookingsAndAvailability()
		{
			DateTime now = IcdEnvironment.GetUtcTime();
			DateTime tomorrow = now.AddDays(1);

			List<BookingGroup> bookings =
				m_SubscribedCalendarManager == null
					? new List<BookingGroup>()
					: m_SubscribedCalendarManager.GetBookings()
					                   .Where(b => CalendarUtils.IsInRange(b, now, tomorrow))
					                   .OrderBy(b => b.StartTime)
					                   .ToList();

			int count = 0;

			// Add an empty booking (room available) if there isn't a meeting for at least 15 minutes
			IBooking firstBooking = bookings.FirstOrDefault();
			if (firstBooking != null && firstBooking.StartTime - now > TimeSpan.FromMinutes(15))
			{
				yield return new EmptyBooking
				{
					StartTime = DateTime.MinValue,
					EndTime = firstBooking.StartTime
				};
				count++;
			}

			// Build list of bookings and available times
			for (int i = 0; i < bookings.Count && count < 7; i++)
			{
				yield return bookings[i];
				count++;

				// If we've run out of bookings we add an available slot to the end
				if (i + 1 >= bookings.Count)
				{
					yield return new EmptyBooking
					{
						StartTime = bookings[i].EndTime,
						EndTime = DateTime.MaxValue
					};
					count++;
				}
				// If there are at least 30 minutes to the next booking insert an available slot
				else if (bookings[i + 1].StartTime - bookings[i].EndTime >= TimeSpan.FromMinutes(30))
				{
					yield return new EmptyBooking
					{
						StartTime = bookings[i].EndTime,
						EndTime = bookings[i + 1].StartTime
					};
					count++;
				}
			}
		}

		private string GetBookingIcon(IBooking booking)
		{
			IEnumerable<IConferenceDeviceControl> dialers =
				Room == null
					? Enumerable.Empty<IConferenceDeviceControl>()
					: Room.GetControlsRecursive<IConferenceDeviceControl>();

			switch (ConferencingBookingUtils.GetMeetingType(booking, dialers))
			{
				case eMeetingType.VideoConference:
					return "videoConference";
				case eMeetingType.AudioConference:
					return "audioConference";
				default:
					return "display";
			}
		}

		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			Unsubscribe(m_SubscribedCalendarManager);
			m_SubscribedCalendarManager = room == null ? null : room.CalendarManager;
			Subscribe(m_SubscribedCalendarManager);

			Refresh();
		}

		#region Private Methods

		private IEnumerable<IReferencedScheduleView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory as IOsdViewFactory, count);
		}

		private string FormatTime(DateTime time)
		{
			return Theme.DateFormatting.GetShortTime(time.ToLocalTime());
		}

		#endregion

		#region Theme Callbacks

		private void DateFormattingOnFormatChanged(object sender, EventArgs eventArgs)
		{
			Refresh();
		}

		#endregion

		#region CalendarControl Callbacks

		private void Subscribe(ICalendarManager calendarManager)
		{
			if (calendarManager == null)
				return;

			calendarManager.OnBookingsChanged += CalendarManagerOnBookingsChanged;
		}

		private void Unsubscribe(ICalendarManager calendarManager)
		{
			if (calendarManager == null)
				return;

			calendarManager.OnBookingsChanged -= CalendarManagerOnBookingsChanged;
		}

		private void CalendarManagerOnBookingsChanged(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Child Callbacks

		private void Subscribe(IReferencedSchedulePresenter presenter)
		{
		}

		private void Unsubscribe(IReferencedSchedulePresenter presenter)
		{
		}

		#endregion
	}
}
