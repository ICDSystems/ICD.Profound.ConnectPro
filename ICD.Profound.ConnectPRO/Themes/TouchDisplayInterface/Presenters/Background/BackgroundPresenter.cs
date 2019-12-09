using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Background;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Background;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Background
{
	[PresenterBinding(typeof(IBackgroundPresenter))]
	public sealed class BackgroundPresenter : AbstractTouchDisplayPresenter<IBackgroundView>, IBackgroundPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		public BackgroundPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			Subscribe(theme);
		}

		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(Theme);
		}

		protected override void Refresh(IBackgroundView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				if (Room != null && Room.IsInMeeting && Room.Routing.State.GetSourceRoutedStates().Any(s => s.Value == eSourceState.Active))
					view.SetBackgroundMode(eTouchCueBackgroundMode.HdmiInput);
				else
					view.SetBackgroundMode(ConvertCueBackgroundMode(Theme.CueBackground));
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private eTouchCueBackgroundMode ConvertCueBackgroundMode(eCueBackgroundMode mode)
		{
			switch (mode)
			{
				case eCueBackgroundMode.Monthly:
					return eTouchCueBackgroundMode.Monthly;
				case eCueBackgroundMode.Neutral:
				default:
					return eTouchCueBackgroundMode.Neutral;
			}
		}

		#region Theme Callbacks

		private void Subscribe(ConnectProTheme theme)
		{
			theme.OnCueBackgroundChanged += ThemeOnCueBackgroundChanged;
		}

		private void Unsubscribe(ConnectProTheme theme)
		{
			theme.OnCueBackgroundChanged -= ThemeOnCueBackgroundChanged;
		}

		private void ThemeOnCueBackgroundChanged(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Room Callbacks

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);
			
			if (room == null)
				return;

			room.Routing.State.OnSourceRoutedChanged += RoomRoutingStateOnSourceRoutedChanged;
		}

		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.Routing.State.OnSourceRoutedChanged -= RoomRoutingStateOnSourceRoutedChanged;
		}

		private void RoomRoutingStateOnSourceRoutedChanged(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}