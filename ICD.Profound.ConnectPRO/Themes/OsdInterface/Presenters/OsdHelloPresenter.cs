﻿using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.CalendarControl;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	[PresenterBinding(typeof(IOsdHelloPresenter))]
	public sealed class OsdHelloPresenter : AbstractOsdPresenter<IOsdHelloView>, IOsdHelloPresenter
	{
		public event EventHandler<BoolEventArgs> OnMainPageViewChanged;

		private const int DEFAULT_REFRESH_TIME = 15 * 60 * 1000;

		private ICalendarControl m_CalendarControl;
		private readonly SafeTimer m_RefreshTimer;
		private readonly SafeCriticalSection m_RefreshSection;

		private bool m_MainPageView;
		public bool MainPageView
		{
			get { return m_MainPageView; }
			private set
			{
				if (m_MainPageView == value)
					return;
				m_MainPageView = value;
				OnMainPageViewChanged.Raise(this, new BoolEventArgs(value));
			} 
		}

		public OsdHelloPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshTimer = new SafeTimer(Refresh, DEFAULT_REFRESH_TIME);
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IOsdHelloView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				MainPageView = Room != null && Room.CalendarControl == null && !Room.IsInMeeting;
				view.SetMainPageView(MainPageView);

				// Corey Geiser on Slack at 10/10/18 12:00PM -
				// "So the first line we will use is 'Welcome to your meeting.'
				// Second line when a meeting is about to start will say 'Are you here for your meeting? Let's get started.'"

				DateTime now = IcdEnvironment.GetLocalTime();
				IBooking nextBooking = m_CalendarControl != null
					? m_CalendarControl.GetBookings().Where(b => b.EndTime > now)
						.OrderBy(b => b.StartTime).FirstOrDefault()
					: null;

				if (Room == null)
					view.SetLabelText(string.Empty);
				else if (nextBooking == null || nextBooking.StartTime - TimeSpan.FromMinutes(15) > now || Room.IsInMeeting)
					view.SetLabelText("Welcome to your meeting.");
				else
					view.SetLabelText("Are you here for your meeting? Let's get started.");

				if (nextBooking == null)
					m_RefreshTimer.Reset(DEFAULT_REFRESH_TIME);
				else if (nextBooking.StartTime - TimeSpan.FromMinutes(15) > now)
					m_RefreshTimer.Reset((long) (nextBooking.StartTime - now).TotalMilliseconds + 1000);
				else
					m_RefreshTimer.Reset((long) (nextBooking.EndTime - now).TotalMilliseconds + 1000);
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
	}
}