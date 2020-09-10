using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Calendaring.CalendarManagers;
using ICD.Connect.Calendaring.Comparers;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.FooterNotifications;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.FooterNotifications;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.FooterNotifications
{
	[PresenterBinding(typeof(IOsdHelloFooterNotificationPresenter))]
	public sealed class OsdHelloFooterNotificationPresenter : AbstractOsdPresenter<IOsdHelloFooterNotificationView>, IOsdHelloFooterNotificationPresenter
	{
		public event EventHandler<BoolEventArgs> OnMainPageViewChanged;

		private const int DEFAULT_UPDATE_TIME = 15 * 60 * 1000;

		private readonly SafeTimer m_UpdateBookingsTimer;
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly List<IBooking> m_Bookings;
		private readonly List<KeyValuePair<string, string>> m_Messages;

		private ICalendarManager m_CalendarManager;
		private bool m_MainPageView;

		#region Properties

		public bool MainPageView
		{
			get { return m_MainPageView; }
			private set
			{
				if (m_MainPageView == value)
					return;

				m_MainPageView = value;

				RefreshIfVisible();

				OnMainPageViewChanged.Raise(this, new BoolEventArgs(value));
			} 
		}

		/// <summary>
		/// Gets the calendar manager.
		/// </summary>
		[CanBeNull]
		private ICalendarManager CalendarManager
		{
			get { return m_CalendarManager; }
			set
			{
				if (value == m_CalendarManager)
					return;

				Unsubscribe(m_CalendarManager);
				m_CalendarManager = value;
				Subscribe(m_CalendarManager);

				UpdateBookings();
				UpdateMainPageView();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdHelloFooterNotificationPresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_UpdateBookingsTimer = new SafeTimer(UpdateBookings, DEFAULT_UPDATE_TIME);
			m_RefreshSection = new SafeCriticalSection();
			m_Bookings = new List<IBooking>();
			m_Messages = new List<KeyValuePair<string, string>>();
		}

		protected override void Refresh(IOsdHelloFooterNotificationView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				DateTime now = IcdEnvironment.GetUtcTime();
				IBooking nextBooking = m_Bookings.FirstOrDefault();

				string labelText;

				if (Room == null)
					labelText = string.Empty;

				// There is a custom message taking precendence
				else if (m_Messages.Count > 0)
					labelText = m_Messages.Last().Value;
				// There is no booking starting in the next 15 minutes, or we're in a meeting already
				else if (nextBooking == null || nextBooking.StartTime - TimeSpan.FromMinutes(15) > now || Room.IsInMeeting)
					labelText = "Welcome to your meeting.";
				// There is a booking starting in 15 minutes
				else
					labelText = "Are you here for your meeting? Let's get started.";

				view.SetLabelText(labelText);
				view.SetMainPageView(MainPageView);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Methods

		/// <summary>
		/// Adds the message to the top of the stack and refreshes the view.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		public void PushMessage(string key, string message)
		{
			m_RefreshSection.Enter();

			try
			{
				m_Messages.RemoveAll(kvp => kvp.Key == key);
				m_Messages.Add(new KeyValuePair<string, string>(key, message));
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		/// <summary>
		/// Removes the messages from the stack with the given key and refreshes the view.
		/// </summary>
		/// <param name="key"></param>
		public void ClearMessages(string key)
		{
			m_RefreshSection.Enter();

			try
			{
				m_Messages.RemoveAll(kvp => kvp.Key == key);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the list of upcoming bookings and refreshes the page.
		/// </summary>
		private void UpdateBookings()
		{
			// Get the current list of bookings
			DateTime now = IcdEnvironment.GetUtcTime();
			IBooking[] bookings =
				CalendarManager == null
					? new IBooking[0]
					: CalendarManager.GetBookings()
					                 .Where(b => b.EndTime > now)
					                 .OrderBy(b => b.StartTime)
					                 .ToArray();

			// Update the timer to check bookings again later
			IBooking nextBooking = bookings.FirstOrDefault();
			long nextUpdate;

			if (nextBooking == null)
				nextUpdate = DEFAULT_UPDATE_TIME;
			else if (nextBooking.StartTime - TimeSpan.FromMinutes(15) > now)
				nextUpdate = (long)(nextBooking.StartTime - now).TotalMilliseconds + 1000;
			else
				nextUpdate = (long)(nextBooking.EndTime - now).TotalMilliseconds + 1000;

			m_UpdateBookingsTimer.Reset(nextUpdate);

			// Update the tracked bookings and refresh
			m_RefreshSection.Enter();

			try
			{
				if (bookings.SequenceEqual(m_Bookings, BookingEqualityComparer.Instance))
					return;

				m_Bookings.SetRange(bookings);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		private void UpdateMainPageView()
		{
			MainPageView = Room != null && CalendarManager == null && !Room.IsInMeeting;
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			CalendarManager = room == null ? null : room.CalendarManager;

			UpdateMainPageView();
		}

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
		}

		/// <summary>
		/// Called when the room enters/leaves a meeting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			UpdateMainPageView();
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

		/// <summary>
		/// Called when the calendar manager bookings change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CalendarManagerOnBookingsChanged(object sender, EventArgs e)
		{
			UpdateBookings();
		}

		#endregion
	}
}
