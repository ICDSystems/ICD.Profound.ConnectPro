using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Calendaring.Comparers;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters
{
	[PresenterBinding(typeof(IOsdHelloPresenter))]
	public sealed class OsdHelloPresenter : AbstractOsdPresenter<IOsdHelloView>, IOsdHelloPresenter
	{
		public event EventHandler<BoolEventArgs> OnMainPageViewChanged;

		private const int DEFAULT_UPDATE_TIME = 15 * 60 * 1000;

		private readonly SafeTimer m_UpdateBookingsTimer;
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly List<IBooking> m_Bookings;
		private readonly List<string> m_Messages;

		private ICalendarControl m_CalendarControl;
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
		/// Gets the calendar control.
		/// </summary>
		[CanBeNull]
		public ICalendarControl CalendarControl
		{
			get { return m_CalendarControl; }
			private set
			{
				if (value == m_CalendarControl)
					return;

				Unsubscribe(m_CalendarControl);
				m_CalendarControl = value;
				Subscribe(m_CalendarControl);

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
		public OsdHelloPresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_UpdateBookingsTimer = new SafeTimer(UpdateBookings, DEFAULT_UPDATE_TIME);
			m_RefreshSection = new SafeCriticalSection();
			m_Bookings = new List<IBooking>();
			m_Messages = new List<string>();
		}

		protected override void Refresh(IOsdHelloView view)
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

				else if (m_Messages.Count > 0)
					labelText = m_Messages.Last();
				else if (nextBooking == null || nextBooking.StartTime - TimeSpan.FromMinutes(15) > now || Room.IsInMeeting)
					labelText = "Welcome to your meeting.";
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
		/// <param name="message"></param>
		public void PushMessage(string message)
		{
			m_RefreshSection.Enter();

			try
			{
				m_Messages.RemoveAll(m => m == message);
				m_Messages.Add(message);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		/// <summary>
		/// Removes the message from the stack and refreshes the view.
		/// </summary>
		/// <param name="message"></param>
		public void PopMessage(string message)
		{
			m_RefreshSection.Enter();

			try
			{
				m_Messages.RemoveAll(m => m == message);
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
				CalendarControl == null
					? new IBooking[0]
					: CalendarControl.GetBookings()
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
			MainPageView = Room != null && CalendarControl == null && !Room.IsInMeeting;
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

			CalendarControl = room == null ? null : room.GetCalendarControls().FirstOrDefault();

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
		/// Subscribe to the calendar control events.
		/// </summary>
		/// <param name="control"></param>
		private void Subscribe(ICalendarControl control)
		{
			if (control == null)
				return;

			control.OnBookingsChanged += ControlOnBookingsChanged;
		}

		/// <summary>
		/// Unsubscribe from the calendar control events.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(ICalendarControl control)
		{
			if (control == null)
				return;

			control.OnBookingsChanged -= ControlOnBookingsChanged;
		}

		/// <summary>
		/// Called when the calendar control bookings change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ControlOnBookingsChanged(object sender, EventArgs e)
		{
			UpdateBookings();
		}

		#endregion
	}
}
