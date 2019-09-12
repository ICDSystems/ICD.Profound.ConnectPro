using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.Shared.Models;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Schedule;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Schedule
{
    [PresenterBinding(typeof(ISchedulePresenter))]
    public sealed class SchedulePresenter : AbstractTouchDisplayPresenter<IScheduleView>, ISchedulePresenter
    {
        private const int DEFAULT_REFRESH_TIME = 15 * 60 * 1000;
        private readonly ReferencedBookingPresenterFactory m_ChildrenFactory;

        private readonly SafeCriticalSection m_RefreshSection;
        private readonly SafeTimer m_RefreshTimer;

        private ICalendarControl m_CalendarControl;
        private IReferencedBookingPresenter m_SelectedBooking;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="views"></param>
        /// <param name="theme"></param>
        public SchedulePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
            ConnectProTheme theme) :
            base(nav, views, theme)
        {
            m_RefreshSection = new SafeCriticalSection();
            m_ChildrenFactory = new ReferencedBookingPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);

            m_RefreshTimer = new SafeTimer(RefreshIfVisible, DEFAULT_REFRESH_TIME);
        }

        protected override void Refresh(IScheduleView view)
        {
            base.Refresh(view);

            m_RefreshSection.Enter();

            try
            {
                if (m_CalendarControl == null)
                    return;

                var now = IcdEnvironment.GetLocalTime();
                var tomorrow = now.AddDays(1);

                var bookings =
                    m_CalendarControl.GetBookings()
                        .Where(b => b.EndTime > now && b.StartTime < tomorrow)
                        .OrderBy(b => b.StartTime)
                        .ToList();

                var roomName = Room == null ? string.Empty : Room.Name;

                var upcomingBookingsAndAvailability = new List<IBooking>();

                var firstBooking = bookings.FirstOrDefault();
                // find out if room is currently available
                if (firstBooking != null && firstBooking.StartTime - now > TimeSpan.FromMinutes(15))
                    upcomingBookingsAndAvailability.Add(new EmptyBooking
                    {
                        StartTime = DateTime.MinValue,
                        EndTime = firstBooking.StartTime
                    });
                // build list of bookings and available times
                for (var i = 0; i < bookings.Count && upcomingBookingsAndAvailability.Count < 7; i++)
                {
                    if (bookings[i] == null)
                        continue;

                    upcomingBookingsAndAvailability.Add(bookings[i]);

                    if (i + 1 >= bookings.Count)
                        upcomingBookingsAndAvailability.Add(new EmptyBooking
                        {
                            StartTime = bookings[i].EndTime,
                            EndTime = DateTime.MaxValue
                        });

                    // calculate availability between this booking and next
                    else if (bookings[i + 1].StartTime - bookings[i].EndTime >= TimeSpan.FromMinutes(30))
                        upcomingBookingsAndAvailability.Add(new EmptyBooking
                        {
                            StartTime = bookings[i].EndTime,
                            EndTime = bookings[i + 1].StartTime
                        });
                }

                // build presenters
                foreach (var presenter in m_ChildrenFactory.BuildChildren(upcomingBookingsAndAvailability.Skip(1)))
                {
                    presenter.ShowView(true);
                    presenter.SetSelected(presenter == m_SelectedBooking);
                    presenter.Refresh();
                }

                // display current room status
                var currentBooking = upcomingBookingsAndAvailability.FirstOrDefault();
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
                    m_RefreshTimer.Reset((long) (currentBooking.EndTime - now).TotalMilliseconds + 1000);
                }
                else
                {
                    view.SetCurrentBookingIcon("thumbsUp");
                    view.SetAvailabilityText("AVAILABLE");
                    view.SetCurrentBookingSubject(roomName);

                    if (currentBooking != null)
                    {
                        view.SetCurrentBookingTime(string.Format("Now - {0}",
                            FormatTime(currentBooking.EndTime)));
                        m_RefreshTimer.Reset((long) (currentBooking.EndTime - now - TimeSpan.FromMinutes(15))
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
            var dialers =
                Room == null
                    ? Enumerable.Empty<IConferenceDeviceControl>()
                    : Room.GetControlsRecursive<IConferenceDeviceControl>();

            switch (ConferencingBookingUtils.GetMeetingType(booking, dialers))
            {
                case eMeetingType.VideoConference:
                    return "videoConference";
                case eMeetingType.AudioConference:
                    return "audioConference";
                case eMeetingType.Presentation:
                    return "display";
            }

            return "display";
        }

        #region View Callbacks

        protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
        {
            base.ViewOnVisibilityChanged(sender, args);

            m_SelectedBooking = null;
        }

        #endregion

        #region Private Methods

        private IEnumerable<IReferencedBookingView> ItemFactory(ushort count)
        {
            return GetView().GetChildComponentViews(ViewFactory as ITouchDisplayViewFactory, count);
        }

        private static string FormatTime(DateTime time)
        {
            return ConnectProDateFormatting.GetShortTime(time);
        }

        #endregion

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

            m_SelectedBooking = presenter;
            RefreshIfVisible();
        }

        #endregion
    }
}