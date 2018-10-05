using System;
using System.Collections.Generic;
using System.Text;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	public sealed class HelloPresenter : AbstractOsdPresenter<IHelloView>, IHelloPresenter
	{
		public HelloPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		protected override void Refresh(IHelloView view)
		{
			base.Refresh(view);

			view.SetLabelText("Hello.");
			view.SetMainPageView(Room != null && Room.CalendarControl == null && !Room.IsInMeeting);
		}

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnOnIsInMeetingChanged;
		}

		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnOnIsInMeetingChanged;
		}

		private void RoomOnOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			Refresh();
		}
	}
}
