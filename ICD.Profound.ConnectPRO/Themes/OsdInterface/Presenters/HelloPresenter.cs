using System;
using System.Collections.Generic;
using System.Text;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	public sealed class HelloPresenter : AbstractOsdPresenter<IHelloView>, IHelloPresenter
	{
		public event EventHandler<BoolEventArgs> OnMainPageViewChanged;

		private bool m_MainPageView = false;
		public bool MainPageView
		{
			get { return m_MainPageView; }
			private set
			{
				if (m_MainPageView == value)
					return;
				m_MainPageView = value;
				OnMainPageViewChanged.Raise(this, new BoolEventArgs(value));
			} 
		}

		public HelloPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		protected override void Refresh(IHelloView view)
		{
			base.Refresh(view);

			view.SetLabelText("Hello.");

			MainPageView = Room != null && Room.CalendarControl == null && !Room.IsInMeeting;
			view.SetMainPageView(MainPageView);
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
