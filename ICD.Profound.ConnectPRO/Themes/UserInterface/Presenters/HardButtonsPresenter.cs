using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Panels.Controls;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	public sealed class HardButtonsPresenter : AbstractPresenter<IHardButtonsView>, IHardButtonsPresenter
	{
		private const int ADDRESS_POWER = 1;
		private const int ADDRESS_HOME = 2;
		private const int ADDRESS_LIGHT = 3;
		private const int ADDRESS_VOL_UP = 4;
		private const int ADDRESS_VOL_DOWN = 5;

		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public HardButtonsPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IHardButtonsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IHardButtonBacklightControl control = GetHardButtonBacklightControl();
				if (control == null)
					return;

				bool isInMeeting = Room != null && Room.IsInMeeting;

				control.SetBacklightEnabled(ADDRESS_POWER, isInMeeting);
				control.SetBacklightEnabled(ADDRESS_HOME, isInMeeting);
				control.SetBacklightEnabled(ADDRESS_LIGHT, false);
				control.SetBacklightEnabled(ADDRESS_VOL_UP, false);
				control.SetBacklightEnabled(ADDRESS_VOL_DOWN, false);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		/// <summary>
		/// Gets the hard button backlight control for the current panel.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IHardButtonBacklightControl GetHardButtonBacklightControl()
		{
			return ViewFactory.Panel.Controls.GetControl<IHardButtonBacklightControl>();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IHardButtonsView view)
		{
			base.Subscribe(view);

			view.OnPowerButtonPressed += ViewOnPowerButtonPressed;
			view.OnHomeButtonPressed += ViewOnHomeButtonPressed;
			view.OnLightsButtonPressed += ViewOnLightsButtonPressed;
			view.OnVolumeUpButtonPressed += ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed += ViewOnVolumeDownButtonPressed;
			view.OnVolumeButtonReleased += ViewOnVolumeButtonReleased;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IHardButtonsView view)
		{
			base.Unsubscribe(view);

			view.OnPowerButtonPressed -= ViewOnPowerButtonPressed;
			view.OnHomeButtonPressed -= ViewOnHomeButtonPressed;
			view.OnLightsButtonPressed -= ViewOnLightsButtonPressed;
			view.OnVolumeUpButtonPressed -= ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed -= ViewOnVolumeDownButtonPressed;
			view.OnVolumeButtonReleased -= ViewOnVolumeButtonReleased;
		}

		/// <summary>
		/// Called when the user presses the power button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPowerButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			Room.IsInMeeting = false;
		}

		/// <summary>
		/// Called when the user presses the home button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnHomeButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			Room.IsInMeeting = false;
		}

		/// <summary>
		/// Called when the user presses the lights button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnLightsButtonPressed(object sender, EventArgs eventArgs)
		{
		}

		/// <summary>
		/// Called when the user presses the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeUpButtonPressed(object sender, EventArgs eventArgs)
		{
		}

		/// <summary>
		/// Called when the user presses the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeDownButtonPressed(object sender, EventArgs eventArgs)
		{
		}

		/// <summary>
		/// Called when the user releases a volume button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeButtonReleased(object sender, EventArgs eventArgs)
		{
		}

		#endregion

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

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
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

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
