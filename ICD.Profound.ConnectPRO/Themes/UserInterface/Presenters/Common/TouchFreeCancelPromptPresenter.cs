using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Calendaring.CalendarManagers;
using ICD.Connect.Partitioning.Commercial;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	[PresenterBinding(typeof(ITouchFreeCancelPromptPresenter))]
	public sealed class TouchFreeCancelPromptPresenter : AbstractUiPresenter<ITouchFreeCancelPromptView>,
	                                                     ITouchFreeCancelPromptPresenter
	{

		private readonly SafeCriticalSection m_RefreshSection;



		protected override void Refresh(ITouchFreeCancelPromptView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				int seconds = Room == null ? 0 : (int)Room.MeetingStartTimer.RemainingSeconds;
				view.SetTimer(seconds);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
			
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public TouchFreeCancelPromptPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
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

			room.MeetingStartTimer.OnMillisecondsChanged += RoomOnMeetingStartTimerChanged;
			room.MeetingStartTimer.OnIsRunningChanged += RoomOnMeetingTimerStarted;
			room.MeetingStartTimer.OnElapsed += MeetingStartTimerOnElapsed;
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

			room.MeetingStartTimer.OnMillisecondsChanged -= RoomOnMeetingStartTimerChanged;
			room.MeetingStartTimer.OnIsRunningChanged -= RoomOnMeetingTimerStarted;
			room.MeetingStartTimer.OnElapsed -= MeetingStartTimerOnElapsed;
		}

		private void MeetingStartTimerOnElapsed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		private void RoomOnMeetingStartTimerChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		private void RoomOnMeetingTimerStarted(object sender, BoolEventArgs eventArgs)
		{
			ShowView(eventArgs.Data);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ITouchFreeCancelPromptView view)
		{
			base.Subscribe(view);

			view.OnCancelMeetingStartPressed += ViewOnCancelMeetingStartPressed;
			view.OnStartMeetingNowPressed += ViewOnOnStartMeetingNowPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ITouchFreeCancelPromptView view)
		{
			base.Unsubscribe(view);

			view.OnCancelMeetingStartPressed -= ViewOnCancelMeetingStartPressed;
			view.OnStartMeetingNowPressed -= ViewOnOnStartMeetingNowPressed;
		}

		private void ViewOnCancelMeetingStartPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Room.MeetingStartTimer.Stop();
		}

		private void ViewOnOnStartMeetingNowPressed(object sender, EventArgs e)
		{
			if (Room == null)
				return;
			Room.StartAutoMeeting();
		}

		#endregion
	}
}