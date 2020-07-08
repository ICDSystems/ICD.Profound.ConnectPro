using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Popups;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Popups;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Popups
{
	[PresenterBinding(typeof(IOsdTouchFreeTimerPresenter))]
	public sealed class OsdTouchFreeTimerPresenter : AbstractOsdPresenter<IOsdTouchFreeTimerView>, IOsdTouchFreeTimerPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdTouchFreeTimerPresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IOsdTouchFreeTimerView view)
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

			room.MeetingStartTimer.OnMillisecondsChanged += MeetingStartTimerOnMillisecondsChanged;
			room.MeetingStartTimer.OnIsRunningChanged += MeetingStartTimerOnIsRunningChanged ;
			room.MeetingStartTimer.OnElapsed += MeetingStartTimerOnElapsed ;
		}

		/// <summary>
		/// Unsubscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.MeetingStartTimer.OnMillisecondsChanged -= MeetingStartTimerOnMillisecondsChanged;
			room.MeetingStartTimer.OnIsRunningChanged -= MeetingStartTimerOnIsRunningChanged;
			room.MeetingStartTimer.OnElapsed -= MeetingStartTimerOnElapsed;
		}

		private void MeetingStartTimerOnElapsed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		private void MeetingStartTimerOnIsRunningChanged(object sender, BoolEventArgs eventArgs)
		{
			ShowView(eventArgs.Data);
		}

		private void MeetingStartTimerOnMillisecondsChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}