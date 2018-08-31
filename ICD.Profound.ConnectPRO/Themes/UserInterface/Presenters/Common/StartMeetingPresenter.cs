using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Calendaring.CalendarControl;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Connect.Devices;
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
		private ICalendarControl m_CalendarControl;

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
				IEnumerable<IBooking> bookings = m_CalendarControl == null ? Enumerable.Empty<IBooking>() : m_CalendarControl.GetBookings();

				foreach (IReferencedSchedulePresenter presenter in m_ChildrenFactory.BuildChildren(bookings, Subscribe, Unsubscribe))
				{
					presenter.ShowView(true);
				}

				view.SetLogoPath(Theme.Logo);

				// TODO - This will be handled by scheduling features
				view.SetStartMyMeetingButtonEnabled(true);

				bool hasCalendarControl = m_CalendarControl != null;

				view.SetStartNewMeetingButtonEnabled(hasCalendarControl);
				view.SetBookingsVisible(hasCalendarControl);
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

			ICalendarControl calendarControl = room == null
				? null
				: room.Originators.GetInstancesRecursive<IDeviceBase>()
					.SelectMany(o => o.Controls.GetControls<ICalendarControl>())
					.FirstOrDefault();

			SetCalendarControl(calendarControl);
		}

		private void SetCalendarControl(ICalendarControl calendarControl)
		{
			if (calendarControl == m_CalendarControl)
				return;

			Unsubscribe(m_CalendarControl);
			m_CalendarControl = calendarControl;
			Subscribe(m_CalendarControl);

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
			throw new NotImplementedException();
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

			view.OnStartMeetingButtonPressed += ViewOnStartMeetingButtonPressed;
			view.OnSettingsButtonPressed += ViewOnSettingsButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IStartMeetingView view)
		{
			base.Unsubscribe(view);

			view.OnStartMeetingButtonPressed -= ViewOnStartMeetingButtonPressed;
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
		private void ViewOnStartMeetingButtonPressed(object sender, EventArgs eventArgs)
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
