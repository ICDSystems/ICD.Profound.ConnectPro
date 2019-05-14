using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.CalendarControl;
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
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedSchedulePresenterFactory m_ChildrenFactory;
	    private readonly SafeTimer m_RefreshTimer;

		private ICalendarControl m_CalendarControl;

	    private const int DEFAULT_REFRESH_TIME = 15 * 60 * 1000;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdWelcomePresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) :
			base(nav, views, theme)
		{
            m_RefreshSection = new SafeCriticalSection();
		    m_ChildrenFactory = new ReferencedSchedulePresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);

		    m_RefreshTimer = new SafeTimer(RefreshIfVisible, DEFAULT_REFRESH_TIME);
		}

		protected override void Refresh(IOsdWelcomeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				if (m_CalendarControl == null)
					return;

		        DateTime now = IcdEnvironment.GetLocalTime();

		        List<IBooking> bookings = m_CalendarControl.GetBookings().Where(b => b.EndTime > now)
		                    .OrderBy(b => b.StartTime).ToList();

		        string roomName = Room == null ? string.Empty : Room.Name;

		        List<IBooking> upcomingBookingsAndAvailability = new List<IBooking>();

		        IBooking firstBooking = bookings.FirstOrDefault();
				// find out if room is currently available
                if (firstBooking != null && firstBooking.StartTime - now > TimeSpan.FromMinutes(15))
                {
                    upcomingBookingsAndAvailability.Add(new EmptyBooking
                    {
                        StartTime = DateTime.MinValue,
                        EndTime = firstBooking.StartTime
                    });
                }
				// build list of bookings and available times
		        for (int i = 0; i < bookings.Count && upcomingBookingsAndAvailability.Count < 7; i++)
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
		        foreach (IReferencedSchedulePresenter presenter in m_ChildrenFactory.BuildChildren(upcomingBookingsAndAvailability.Skip(1)))
		        {
		            presenter.ShowView(true);
		            presenter.Refresh();
		        }

				// display current room status
		        IBooking currentBooking = upcomingBookingsAndAvailability.FirstOrDefault();
		        if (currentBooking != null && !(currentBooking is EmptyBooking))
		        {
			        var firstNumber = currentBooking.GetBookingNumbers().FirstOrDefault();
			        view.SetCurrentBookingIcon(GetBookingIcon(firstNumber == null
				        ? eMeetingType.Presentation
				        : firstNumber.Protocol.ToMeetingType()));

			        view.SetCurrentBookingSubject(currentBooking.IsPrivate ? "Private Meeting" : currentBooking.MeetingName);
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
		            view.SetCurrentBookingSubject(roomName);

		            if (currentBooking != null)
		            {
		                view.SetCurrentBookingTime(string.Format("Now - {0}",
		                    FormatTime(currentBooking.EndTime)));
                        m_RefreshTimer.Reset((long)(currentBooking.EndTime - now - TimeSpan.FromMinutes(15)).TotalMilliseconds + 1000);
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

	    private string GetBookingIcon(eMeetingType currentBookingType)
        {
            switch (currentBookingType)
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

	    #region Private Methods

	    private IEnumerable<IReferencedScheduleView> ItemFactory(ushort count)
	    {
	        return GetView().GetChildComponentViews(ViewFactory as IOsdViewFactory, count);
	    }

        private static string FormatTime(DateTime time)
        {
	        return time.ToString(ConnectProTheme.ThemeCulture.DateTimeFormat.ShortTimePattern);
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

		private void Subscribe(IReferencedSchedulePresenter presenter)
		{
		}

		private void Unsubscribe(IReferencedSchedulePresenter presenter)
		{
		}

		#endregion
	}
}
