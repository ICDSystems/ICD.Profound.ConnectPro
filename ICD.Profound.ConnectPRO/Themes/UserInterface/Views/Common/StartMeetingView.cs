using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	[ViewBinding(typeof(IStartMeetingView))]
	public sealed partial class StartMeetingView : AbstractUiView, IStartMeetingView
	{
		private const ushort DEFAULT_SUBPAGE_VISIBILITY = 90;
		private const ushort BOOKINGS_SUBPAGE_VISIBILITY = 170;
		private const ushort BOOKING_NOMEETINGS_SUBPAGE_VISIBILITY = 89;

		/// <summary>
		/// Raised when the user presses the start my meeting button.
		/// </summary>
		public event EventHandler OnStartMyMeetingButtonPressed;

		/// <summary>
		/// Raised when the user pressed the start new meeting button.
		/// </summary>
		public event EventHandler OnInstantMeetingButtonPressed;

		/// <summary>
		/// Raised when the user presses the settings button.
		/// </summary>
		public event EventHandler OnSettingsButtonPressed;

		/// <summary>
		/// Raised when the user presses the room combine button.
		/// </summary>
		public event EventHandler OnRoomCombineButtonPressed;

		private readonly List<IReferencedScheduleView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public StartMeetingView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedScheduleView>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnStartMyMeetingButtonPressed = null;
			OnInstantMeetingButtonPressed = null;
			OnSettingsButtonPressed = null;
			OnRoomCombineButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the visibility of the bookings list.
		/// </summary>
		/// <param name="visible"></param>
		/// <param name="bookings"></param>
		public void SetBookingsVisible(bool visible, int bookings)
		{
			// This is a little hacky because we show two different subpages based on the availability of bookings
			ushort join;

			if (visible)
			{
				join = bookings > 0 ? BOOKINGS_SUBPAGE_VISIBILITY : BOOKING_NOMEETINGS_SUBPAGE_VISIBILITY;
			}
			else
			{
				join = DEFAULT_SUBPAGE_VISIBILITY;
			}

			if (join == m_Subpage.DigitalVisibilityJoin)
				return;

			bool oldVisible = m_Subpage.IsVisible;

			m_Subpage.Show(false);
			m_Subpage.DigitalVisibilityJoin = join;
			m_Subpage.Show(oldVisible);
		}

		/// <summary>
		/// Sets the time label on the splash screen.
		/// </summary>
		/// <param name="label"></param>
		public void SetSplashTimeLabel(string label)
		{
			m_SplashTimeLabel.SetLabelText(label);
		}

		/// <summary>
		/// Sets the visibility of the room combine button.
		/// </summary>
		/// <param name="visible"></param>
		public void SetRoomCombineButtonVisible(bool visible)
		{
			m_RoomCombineButton.Show(visible);
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IReferencedScheduleView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ScheduleList, m_ChildList, count);
		}

		/// <summary>
		/// Sets the image path for the logo.
		/// </summary>
		/// <param name="url"></param>
		public void SetLogoPath(string url)
		{
			m_Logo.SetImageUrl(url);
		}

		/// <summary>
		/// Sets the enabled state of the start my meeting button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetStartMyMeetingButtonEnabled(bool enabled)
		{
			m_StartMyMeetingButton.SetSelected(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the start new meeting button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetStartNewMeetingButtonEnabled(bool enabled)
		{
			m_InstantMeetingButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the no meetings button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetNoMeetingsButtonEnabled(bool enabled)
		{
			m_NoMeetingsButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the text of the no meetings label.
		/// </summary>
		/// <param name="text"></param>
		public void SetNoMeetingsLabel(string text)
		{
			m_NoMeetingsLabel.SetLabelText(text);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_StartMyMeetingButton.OnPressed += StartMyMeetingButtonOnPressed;
			m_InstantMeetingButton.OnPressed += InstantMeetingButtonOnPressed;
			m_SettingsButton.OnPressed += SettingsButtonOnPressed;
			m_RoomCombineButton.OnPressed += RoomCombineButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_StartMyMeetingButton.OnPressed -= StartMyMeetingButtonOnPressed;
			m_InstantMeetingButton.OnPressed -= InstantMeetingButtonOnPressed;
			m_SettingsButton.OnPressed -= SettingsButtonOnPressed;
			m_RoomCombineButton.OnPressed -= RoomCombineButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the room combine button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomCombineButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnRoomCombineButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the shutdown button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SettingsButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSettingsButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the start meeting button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StartMyMeetingButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnStartMyMeetingButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the instant meeting button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void InstantMeetingButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnInstantMeetingButtonPressed.Raise(this);
		}

		#endregion
	}
}
