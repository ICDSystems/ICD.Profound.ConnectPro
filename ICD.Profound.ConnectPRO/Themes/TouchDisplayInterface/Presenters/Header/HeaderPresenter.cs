using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
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

		private readonly ReferencedHeaderButtonPresenterFactory m_LeftButtonsFactory;
		private readonly ReferencedHeaderButtonPresenterFactory m_RightButtonsFactory;

		private readonly List<HeaderButtonModel> m_LeftButtons;
		private readonly List<HeaderButtonModel> m_RightButtons;

		private readonly HeaderButtonModel m_SettingsButton;
		private readonly HeaderButtonModel m_EndMeetingButton;
		
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

			m_LeftButtonsFactory = new ReferencedHeaderButtonPresenterFactory(nav, LeftButtonsViewFactory, EmptySub, EmptyUnsub);
			m_RightButtonsFactory = new ReferencedHeaderButtonPresenterFactory(nav, RightButtonsViewFactory, EmptySub, EmptyUnsub);

			m_LeftButtons = new List<HeaderButtonModel>();
			m_RightButtons = new List<HeaderButtonModel>();

			m_SettingsButton = new HeaderButtonModel(0, 0, OpenSettings)
			{
				Icon = TouchCueIcons.GetIcon("settings"),
				LabelText = "Settings",
				Mode = eHeaderButtonMode.Blue
			};
			m_EndMeetingButton = new HeaderButtonModel(0, 1, EndMeeting)
			{
				Icon = TouchCueIcons.GetIcon("close"),
				LabelText = "End Meeting",
				Mode = eHeaderButtonMode.Red
			};
			
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

		public bool ContainsLeftButton(HeaderButtonModel button)
		{
			return m_LeftButtons.ContainsSorted(button);
		}

		public void AddLeftButton(HeaderButtonModel button)
		{
			m_LeftButtons.AddSorted(button);
		}

		public void RemoveLeftButton(HeaderButtonModel button)
		{
			m_LeftButtons.RemoveSorted(button);
		}

		public bool ContainsRightButton(HeaderButtonModel button)
		{
			return m_RightButtons.ContainsSorted(button);
		}
		
		public void AddRightButton(HeaderButtonModel button)
		{
			m_RightButtons.AddSorted(button);
		}

		public void RemoveRightButton(HeaderButtonModel button)
		{
			m_RightButtons.RemoveSorted(button);
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

				foreach (IReferencedHeaderButtonPresenter button in m_LeftButtonsFactory.BuildChildren(m_LeftButtons))
				{
					button.ShowView(true);
					button.Refresh();
				}
				foreach (IReferencedHeaderButtonPresenter button in m_RightButtonsFactory.BuildChildren(m_RightButtons))
				{
					button.ShowView(true);
					button.Refresh();
				}
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

		private IEnumerable<IReferencedHeaderButtonView> LeftButtonsViewFactory(ushort count)
		{
			return GetView().GetLeftButtonViews(ViewFactory as ITouchDisplayViewFactory, count);
		}

		private IEnumerable<IReferencedHeaderButtonView> RightButtonsViewFactory(ushort count)
		{
			return GetView().GetRightButtonViews(ViewFactory as ITouchDisplayViewFactory, count);
		}

		#region Room Callbacks

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
			UpdateButtons();
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
			UpdateButtons();
		}

		private void UpdateButtons()
		{
			RemoveLeftButton(m_SettingsButton);
			RemoveLeftButton(m_EndMeetingButton);

			if (Room != null)
				AddLeftButton(Room.IsInMeeting ? m_EndMeetingButton : m_SettingsButton);

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

		#region Child Callbacks

		private void EmptySub(IReferencedHeaderButtonPresenter presenter)
		{
		}

		private void EmptyUnsub(IReferencedHeaderButtonPresenter presenter)
		{
		}

		#endregion

		#region Header Button Callbacks

		private void OpenSettings()
		{
			// open settings presenter
		}

		private void EndMeeting()
		{
			if (Room == null)
				return;

			Room.EndMeeting();
		}

		#endregion
	}
}