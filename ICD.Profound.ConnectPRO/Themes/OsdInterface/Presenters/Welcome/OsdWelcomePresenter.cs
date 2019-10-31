﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Welcome;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Welcome;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Welcome
{
	[PresenterBinding(typeof(IOsdWelcomePresenter))]
	public sealed class OsdWelcomePresenter : AbstractOsdPresenter<IOsdWelcomeView>, IOsdWelcomePresenter
	{
		private const int DEFAULT_REFRESH_TIME = 15 * 60 * 1000;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedSchedulePresenterFactory m_ChildrenFactory;
		private readonly SafeTimer m_RefreshTimer;

		[CanBeNull] private ICalendarControl m_CalendarControl;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdWelcomePresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSchedulePresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);

			m_RefreshTimer = new SafeTimer(RefreshIfVisible, DEFAULT_REFRESH_TIME);

			theme.DateFormatting.OnFormatChanged += DateFormattingOnFormatChanged;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IOsdWelcomeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				DateTime now = IcdEnvironment.GetLocalTime();
				DateTime tomorrow = now.AddDays(1);

				List<IBooking> bookings =
					m_CalendarControl == null
						? new List<IBooking>()
						: m_CalendarControl.GetBookings()
						                   .Where(b => b.EndTime > now && b.StartTime < tomorrow)
						                   .OrderBy(b => b.StartTime)
						                   .ToList();

				List<IBooking> upcomingBookingsAndAvailability = new List<IBooking>();

				// Add an empty booking (room available) if there isn't a meeting for at least 15 minutes
				IBooking firstBooking = bookings.FirstOrDefault();
				if (firstBooking != null && firstBooking.StartTime - now > TimeSpan.FromMinutes(15))
				{
					upcomingBookingsAndAvailability.Add(new EmptyBooking
					{
						StartTime = DateTime.MinValue,
						EndTime = firstBooking.StartTime
					});
				}

				// Build list of bookings and available times
				for (int i = 0; i < bookings.Count && upcomingBookingsAndAvailability.Count < 7; i++)
				{
					upcomingBookingsAndAvailability.Add(bookings[i]);

					// If we've run out of bookings we add an available slot to the end
					if (i + 1 >= bookings.Count)
						upcomingBookingsAndAvailability.Add(new EmptyBooking
						{
							StartTime = bookings[i].EndTime,
							EndTime = DateTime.MaxValue
						});

					// If there are at least 30 minutes to the next booking insert an available slot
					else if (bookings[i + 1].StartTime - bookings[i].EndTime >= TimeSpan.FromMinutes(30))
						upcomingBookingsAndAvailability.Add(new EmptyBooking
						{
							StartTime = bookings[i].EndTime,
							EndTime = bookings[i + 1].StartTime
						});
				}

				// Build booking presenters
				foreach (IReferencedSchedulePresenter presenter in
					m_ChildrenFactory.BuildChildren(upcomingBookingsAndAvailability.Skip(1)))
					presenter.ShowView(true);

				// Display current room status
				IBooking currentBooking = upcomingBookingsAndAvailability.FirstOrDefault();
				if (currentBooking != null && !(currentBooking is EmptyBooking))
				{
					view.SetCurrentBookingIcon(GetBookingIcon(currentBooking));

					view.SetCurrentBookingSubject(currentBooking.IsPrivate
						                              ? "Private Meeting"
						                              : currentBooking.MeetingName);
					view.SetCurrentBookingTime(string.Format("{0} - {1}",
					                                         FormatTime(currentBooking.StartTime),
					                                         FormatTime(currentBooking.EndTime)));
					view.SetAvailabilityText("RESERVED");
					m_RefreshTimer.Reset((long)(currentBooking.EndTime - now).TotalMilliseconds + 1000);
				}
				else
				{
					view.SetCurrentBookingIcon("thumbsUp");
					view.SetAvailabilityText("AVAILABLE");

					string roomName = Room == null ? string.Empty : Room.Name;
					view.SetCurrentBookingSubject(roomName);

					if (currentBooking != null)
					{
						view.SetCurrentBookingTime(string.Format("Now - {0}",
						                                         FormatTime(currentBooking.EndTime)));
						m_RefreshTimer.Reset((long)(currentBooking.EndTime - now - TimeSpan.FromMinutes(15))
						                     .TotalMilliseconds + 1000);
					}
					else
					{
						view.SetCurrentBookingTime(" ");
						m_RefreshTimer.Reset(DEFAULT_REFRESH_TIME);
					}
				}
			}
			finally
			{
				m_RefreshSection.Leave();
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

			Unsubscribe(m_CalendarControl);
			m_CalendarControl = room == null ? null : room.CalendarControl;
			Subscribe(m_CalendarControl);

			Refresh();
		}

		#region Private Methods

		private IEnumerable<IReferencedScheduleView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory as IOsdViewFactory, count);
		}

		private string FormatTime(DateTime time)
		{
			return Theme.DateFormatting.GetShortTime(time);
		}

		#endregion

		#region Theme Callbacks

		private void DateFormattingOnFormatChanged(object sender, EventArgs eventArgs)
		{
			Refresh();
		}

		#endregion

		#region CalendarControl Callbacks

		private void Subscribe(ICalendarControl control)
		{
			if (control == null)
				return;

			control.OnBookingsChanged += ControlOnBookingsChanged;
		}

		private void Unsubscribe(ICalendarControl control)
		{
			if (control == null)
				return;

			control.OnBookingsChanged -= ControlOnBookingsChanged;
		}

		private void ControlOnBookingsChanged(object sender, EventArgs e)
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
