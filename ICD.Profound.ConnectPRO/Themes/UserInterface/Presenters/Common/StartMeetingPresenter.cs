using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.CalendarControl;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Devices;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Controls;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class StartMeetingPresenter : AbstractPresenter<IStartMeetingView>, IStartMeetingPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedSchedulePresenterFactory m_ChildrenFactory;

		private IReferencedSchedulePresenter m_SelectedBooking;
		private ICalendarControl m_CalendarControl;

		private bool HasCalendarControl
		{
			get { return m_CalendarControl != null; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public StartMeetingPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSchedulePresenterFactory(nav, ItemFactory);
		}


		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			UnsubscribeChildren();
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IStartMeetingView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
			    IEnumerable<IBooking> bookings =
			        m_CalendarControl == null
			            ? Enumerable.Empty<IBooking>()
			            : m_CalendarControl.GetBookings().Where(b => b.EndTime > IcdEnvironment.GetLocalTime());

				foreach (IReferencedSchedulePresenter presenter in m_ChildrenFactory.BuildChildren(bookings, Subscribe, Unsubscribe))
				{
					presenter.SetSelected(presenter == m_SelectedBooking);
					presenter.ShowView(true);
					presenter.Refresh();
				}

				view.SetLogoPath(Theme.Logo);

				view.SetStartMyMeetingButtonEnabled(!HasCalendarControl || m_SelectedBooking != null);

				view.SetStartNewMeetingButtonEnabled(HasCalendarControl);
				view.SetBookingsVisible(HasCalendarControl);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}


		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			Unsubscribe(m_CalendarControl);
			m_CalendarControl = null;

			if (Room != null)
			{
				m_CalendarControl = Room.CalendarControl;
				Subscribe(m_CalendarControl);
				m_CalendarControl.Refresh();
			}

			RefreshIfVisible();
		}

		private void Subscribe(ICalendarControl calendarControl)
		{
			if (calendarControl == null)
				return;

		    calendarControl.OnBookingsChanged += CalendarControlOnBookingsChanged;
		}

		private void Unsubscribe(ICalendarControl calendarControl)
		{
			if (calendarControl == null) 
				return;

		    calendarControl.OnBookingsChanged -= CalendarControlOnBookingsChanged;
		}

		#region Private Methods

		private IEnumerable<IReferencedScheduleView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		/// <summary>
		/// Unsubscribes from all of the child presenters.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (IReferencedSchedulePresenter presenter in m_ChildrenFactory)
				Unsubscribe(presenter);
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Subscribe to the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Subscribe(IReferencedSchedulePresenter child)
		{
			if (child == null)
				return;

			child.OnPressed += ChildOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Unsubscribe(IReferencedSchedulePresenter child)
		{
			if (child == null)
				return;

			child.OnPressed -= ChildOnPressed;
		}

		/// <summary>
		/// Called when the user presses the child source.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnPressed(object sender, EventArgs eventArgs)
		{
			var booking = sender as IReferencedSchedulePresenter;
			if (booking == null)
				return;

			if (m_SelectedBooking == booking)
				m_SelectedBooking = null;
			else
				m_SelectedBooking = booking;
			RefreshIfVisible();
		}

		private void CalendarControlOnBookingsChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IStartMeetingView view)
		{
			base.Subscribe(view);

			view.OnStartMyMeetingButtonPressed += ViewOnStartMyMeetingButtonPressed;
			view.OnStartNewMeetingButtonPressed += ViewOnStartNewMeetingButtonPressed;
			view.OnSettingsButtonPressed += ViewOnSettingsButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IStartMeetingView view)
		{
			base.Unsubscribe(view);

			view.OnStartMyMeetingButtonPressed -= ViewOnStartMyMeetingButtonPressed;
			view.OnStartNewMeetingButtonPressed -= ViewOnStartNewMeetingButtonPressed;
			view.OnSettingsButtonPressed -= ViewOnSettingsButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the settings button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSettingsButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.LazyLoadPresenter<IPasscodePresenter>().ShowView(PasscodeSuccessCallback);
		}

		/// <summary>
		/// Called when the user presses the start meeting button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnStartMyMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			if(!HasCalendarControl)
				Room.StartMeeting();
			else if (m_SelectedBooking != null)
			{
				Room.StartMeeting(false);

				// check if booking exists
				var booking = m_SelectedBooking.Booking;
				if (booking == null)
					return;

				// check if we have any dialers
				var dialers = Room.GetControlsRecursive<IDialingDeviceControl>().ToList();
				if (dialers == null || !dialers.Any())
					return;

				// check if any dialers support the booking
				var preferredDialer = dialers.OrderByDescending(d => d.CanDial(booking)).FirstOrDefault();
				if (preferredDialer == null || preferredDialer.CanDial(booking) <= eBookingSupport.Unsupported)
					return;

				// route device to displays and/or audio destination
				var dialerDevice = preferredDialer.Parent;
				var routeControl = dialerDevice.Controls.GetControl<IRouteSourceControl>();
				if(dialerDevice is IVideoConferenceDevice)
					Room.Routing.RouteVtc(routeControl);
				else if (preferredDialer.Supports == eConferenceSourceType.Audio)
					Room.Routing.RouteAtc(routeControl);

				// dial booking
				preferredDialer.Dial(booking);
			}
		}

		private void ViewOnStartNewMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Room.StartMeeting();
		}

		/// <summary>
		/// Called when the user successfully enters the passcode.
		/// </summary>
		/// <param name="sender"></param>
		private void PasscodeSuccessCallback(IPasscodePresenter sender)
		{
			Navigation.LazyLoadPresenter<IPasscodePresenter>().ShowView(false);

			Navigation.NavigateTo<ISettingsBasePresenter>();
		}

		#endregion
	}
}
