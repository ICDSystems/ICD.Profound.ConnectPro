using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	[PresenterBinding(typeof(IHeaderPresenter))]
	public sealed class HeaderPresenter : AbstractUiPresenter<IHeaderView>, IHeaderPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_RefreshTimer;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public HeaderPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			// Refresh every second to update the time
			m_RefreshTimer = new SafeTimer(RefreshTime, 1000, 1000);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_RefreshTimer.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IHeaderView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IRoom room = GetRoomForPanel();
				string roomName = room == null ? null : room.Name;

				if (Room != null && Room.IsCombineRoom())
					roomName += " (Combined)";

				view.SetRoomName(roomName);

				RefreshTime();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Updates the time label on the header.
		/// </summary>
		private void RefreshTime()
		{
			IHeaderView view = GetView();
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

		/// <summary>
		/// Gets the room for the current panel.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IRoom GetRoomForPanel()
		{
			if (Room == null)
				return null;

			IPanelDevice panel = ViewFactory.Panel;

			// Is the panel immediately in this room?
			if (Room.Originators.Contains(panel.Id))
				return Room;

			// Is the panel in one of our child rooms?
			return Room.GetRooms()
			           .FirstOrDefault(room => room.Originators.Contains(panel.Id));
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

			room.Originators.OnCollectionChanged += OriginatorsOnCollectionChanged;
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

			room.Originators.OnCollectionChanged -= OriginatorsOnCollectionChanged;
		}

		/// <summary>
		/// Called when the room contents change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void OriginatorsOnCollectionChanged(object sender, EventArgs eventArgs)
		{
			Refresh();
		}

		#endregion
	}
}
