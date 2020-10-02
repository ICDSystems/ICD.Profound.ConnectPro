using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Indicator;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Indicator;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Indicator
{
	[PresenterBinding(typeof(IUpcomingMeetingIndicatorPresenter))]
	public sealed class UpcomingMeetingIndicatorPresenter : AbstractUiPresenter<IUpcomingMeetingIndicatorView>, IUpcomingMeetingIndicatorPresenter
	{
		private const ushort HIDE_TIME = 10 * 1000;
		private readonly SafeTimer m_VisibilityTimer;

		private readonly SafeTimer m_UpcomingBookingTimer;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public UpcomingMeetingIndicatorPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) 
			: base(nav, views, theme)
		{
			m_VisibilityTimer = SafeTimer.Stopped(() => ShowView(false));
			m_UpcomingBookingTimer = SafeTimer.Stopped(ShowIndicator);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			m_VisibilityTimer.Dispose();
			m_UpcomingBookingTimer.Dispose();
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			GetView().PlaySound(args.Data);
			ResetVisibilityTimer();
		}

		/// <summary>
		/// Resets the visibility timer.
		/// </summary>
		private void ResetVisibilityTimer()
		{
			m_VisibilityTimer.Reset(HIDE_TIME);
		}

		/// <summary>
		/// Resets the Upcoming Booking timer.
		/// </summary>
		private void RestartUpcomingBookingTimer()
		{
			IBooking nextBooking =
				Room == null || Room.UpcomingBooking == null
					? null
					: Room.UpcomingBooking;

			if (nextBooking == null)
			{
				m_UpcomingBookingTimer.Stop();
				return;
			}

			TimeSpan timeToNextBooking = nextBooking.StartTime - IcdEnvironment.GetUtcTime();

			// Raise 5 minutes early
			timeToNextBooking -= TimeSpan.FromMinutes(5);
			timeToNextBooking = timeToNextBooking > TimeSpan.Zero ? timeToNextBooking : TimeSpan.Zero;

			m_UpcomingBookingTimer.Reset((long)timeToNextBooking.TotalMilliseconds);
		}

		private void ShowIndicator()
		{
			if (Room != null && Room.CurrentBooking != Room.UpcomingBooking)
				ShowView(true);
			else
				ShowView(false);
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnUpcomingBookingChanged += RoomOnUpcomingMeeting;
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

			room.OnUpcomingBookingChanged -= RoomOnUpcomingMeeting;
			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
		}

		private void RoomOnUpcomingMeeting(object sender, GenericEventArgs<IBooking> genericEventArgs)
		{
			if (genericEventArgs.Data == null)
				ShowView(false);
			else
				RestartUpcomingBookingTimer();
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			RestartUpcomingBookingTimer();
		}

		#endregion
	}
}