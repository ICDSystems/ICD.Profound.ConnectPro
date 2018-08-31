using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class StartMeetingView : AbstractView, IStartMeetingView
	{
		private const ushort DEFAULT_SUBPAGE_VISIBILITY = 90;
		private const ushort BOOKINGS_SUBPAGE_VISIBILITY = 170;

		/// <summary>
		/// Raised when the user presses the start meeting button.
		/// </summary>
		public event EventHandler OnStartMeetingButtonPressed;

		/// <summary>
		/// Raised when the user presses the settings button.
		/// </summary>
		public event EventHandler OnSettingsButtonPressed;

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
			OnStartMeetingButtonPressed = null;
			OnSettingsButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the visibility of the bookings list.
		/// </summary>
		/// <param name="visible"></param>
		public void SetBookingsVisible(bool visible)
		{
			// This is a little hacky because we show two different subpages based on the availability of bookings
			ushort join = visible ? BOOKINGS_SUBPAGE_VISIBILITY : DEFAULT_SUBPAGE_VISIBILITY;
			if (join == m_Subpage.DigitalVisibilityJoin)
				return;

			bool oldVisible = m_Subpage.IsVisible;

			m_Subpage.Show(false);
			m_Subpage.DigitalVisibilityJoin = join;
			m_Subpage.Show(oldVisible);
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
			m_StartNewMeetingButton.Enable(enabled);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_StartMyMeetingButton.OnPressed += StartMeetingButtonOnPressed;
			m_StartNewMeetingButton.OnPressed += StartMeetingButtonOnPressed;
			m_SettingsButton.OnPressed += SettingsButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_StartMyMeetingButton.OnPressed -= StartMeetingButtonOnPressed;
			m_StartNewMeetingButton.OnPressed -= StartMeetingButtonOnPressed;
			m_SettingsButton.OnPressed -= SettingsButtonOnPressed;
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
		private void StartMeetingButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnStartMeetingButtonPressed.Raise(this);
		}

		#endregion
	}
}
