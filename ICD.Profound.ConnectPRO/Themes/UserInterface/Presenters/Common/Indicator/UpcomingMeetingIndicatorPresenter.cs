using ICD.Common.Utils.EventArguments;
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
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public UpcomingMeetingIndicatorPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) 
			: base(nav, views, theme)
		{
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

			room.OnUpcomingMeeting -= RoomOnUpcomingMeeting;
			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
		}

		private void RoomOnUpcomingMeeting(object sender, GenericEventArgs<IBooking> genericEventArgs)
		{
			ShowView(true);
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			ShowView(false);
		}

		#endregion
	}
}