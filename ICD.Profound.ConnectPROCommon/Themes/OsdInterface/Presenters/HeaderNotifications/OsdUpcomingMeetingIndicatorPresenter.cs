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
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			m_VisibilityTimer.Dispose();
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

			room.OnUpcomingMeeting += RoomOnUpcomingMeeting;
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

			room.OnUpcomingMeeting -= RoomOnUpcomingMeeting;
		}

		private void RoomOnUpcomingMeeting(object sender, GenericEventArgs<IBooking> genericEventArgs)
		{
			ShowView(genericEventArgs.Data != null);
		}

		#endregion
	}
}
