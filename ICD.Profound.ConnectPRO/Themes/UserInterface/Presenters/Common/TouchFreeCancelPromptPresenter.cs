using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
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
		public TouchFreeCancelPromptPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme, SafeCriticalSection refreshSection)
			: base(nav, views, theme)
		{
			m_RefreshSection = refreshSection;
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

			room.MeetingStartTimer.OnMillisecondsChanged += RoomOnMettingStartTimerChanged;
			room.MeetingStartTimer.OnIsRunningChanged += RoomOnMettingTimerStarted;
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

			room.MeetingStartTimer.OnMillisecondsChanged -= RoomOnMettingStartTimerChanged;
			room.MeetingStartTimer.OnIsRunningChanged -= RoomOnMettingTimerStarted;
		}

		private void RoomOnMettingStartTimerChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		private void RoomOnMettingTimerStarted(object sender, BoolEventArgs eventArgs)
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
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ITouchFreeCancelPromptView view)
		{
			base.Unsubscribe(view);

			view.OnCancelMeetingStartPressed -= ViewOnCancelMeetingStartPressed;
		}

		private void ViewOnCancelMeetingStartPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Room.MeetingStartTimer.Stop();
		}

		#endregion
	}
}