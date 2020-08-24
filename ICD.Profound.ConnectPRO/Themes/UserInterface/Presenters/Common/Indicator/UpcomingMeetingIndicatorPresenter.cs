using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
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
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public UpcomingMeetingIndicatorPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IUpcomingMeetingIndicatorView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool playSound = Room != null && Room.UpcomingMeeting;
				view.PlaySound(playSound);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
			
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

			room.OnIsUpcomingMeetingChanged += RoomOnUpcomingMeeting;
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

			room.OnIsUpcomingMeetingChanged -= RoomOnUpcomingMeeting;
		}

		private void RoomOnUpcomingMeeting(object sender, BoolEventArgs eventArgs)
		{
			ShowView(eventArgs.Data);
			
			Refresh();
		}
		#endregion
	}
}