using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.HeaderNotifications;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.HeaderNotifications;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.HeaderNotifications
{
	[PresenterBinding(typeof(IOsdUpcomingMeetingIndicatorPresenter))]
	public sealed class OsdUpcomingMeetingIndicatorPresenter : AbstractOsdPresenter<IOsdUpcomingMeetingIndicatorView>,
	                                                           IOsdUpcomingMeetingIndicatorPresenter
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
		public OsdUpcomingMeetingIndicatorPresenter(IOsdNavigationController nav, IOsdViewFactory views,
		                                            IConnectProTheme theme)
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
			TimeSpan timeToNextBooking =
				Room == null || Room.UpcomingBooking == null
					? TimeSpan.MaxValue
					: Room.UpcomingBooking.StartTime - IcdEnvironment.GetUtcTime();

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
			room.OnIsInMeetingChanged  += RoomOnOnIsInMeetingChanged;
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
			room.OnIsInMeetingChanged -= RoomOnOnIsInMeetingChanged;
		}

		private void RoomOnUpcomingMeeting(object sender, GenericEventArgs<IBooking> genericEventArgs)
		{
			if (genericEventArgs.Data == null)
				ShowView(false);
			else
				RestartUpcomingBookingTimer();
		}

		private void RoomOnOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			RestartUpcomingBookingTimer();
		}

		#endregion
	}
}
