using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Header
{
	[PresenterBinding(typeof(IHeaderPresenter))]
	public sealed class HeaderPresenter : AbstractTouchDisplayPresenter<IHeaderView>, IHeaderPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_RefreshTimer;

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public HeaderPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			// Refresh every second to update the time
			m_RefreshTimer = new SafeTimer(RefreshTime, 1000, 1000);
			
			theme.DateFormatting.OnFormatChanged += DateFormattingOnFormatChanged;
		}

		/// <summary>
		///     Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_RefreshTimer.Dispose();

			base.Dispose();
		}

		/// <summary>
		///     Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IHeaderView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				var roomName = Room == null ? string.Empty : Room.Name;
				view.SetRoomName(roomName);

				string icon = Room != null && Room.IsInMeeting
					? "devicedrawer"
					: "instantmeeting";
				view.SetCenterButtonIcon(TouchCueIcons.GetIcon(icon));
				string text = Room != null && Room.IsInMeeting
					? "Device Drawer"
					: "Instant Meeting";
				view.SetCenterButtonText(text);

				RefreshTime();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		///     Updates the time label on the header.
		/// </summary>
		private void RefreshTime()
		{
			var view = GetView();
			if (view == null)
				return;

			if (!m_RefreshSection.TryEnter())
				return;

			try
			{
				view.SetTimeLabel(Theme.DateFormatting.ShortTime);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Room Callbacks

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
		}

		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			Refresh();
		}

		#endregion

		#region Theme Callbacks

		private void DateFormattingOnFormatChanged(object sender, EventArgs e)
		{
			RefreshTime();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IHeaderView view)
		{
			base.Subscribe(view);

			view.OnCenterButtonPressed += ViewOnStartEndMeetingPressed;
		}

		protected override void Unsubscribe(IHeaderView view)
		{
			base.Unsubscribe(view);

			view.OnCenterButtonPressed -= ViewOnStartEndMeetingPressed;
		}

		private void ViewOnStartEndMeetingPressed(object sender, EventArgs e)
		{
			if (!Room.IsInMeeting)
				Room.StartMeeting();
			else
			{
				var deviceDrawer = Navigation.LazyLoadPresenter<IDeviceDrawerPresenter>();
				deviceDrawer.ShowView(!deviceDrawer.IsViewVisible);
			}
		}

		#endregion
	}
}