using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Panels.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	[PresenterBinding(typeof(IHardButtonsPresenter))]
	public sealed class HardButtonsPresenter : AbstractUiPresenter<IHardButtonsView>, IHardButtonsPresenter
	{
		private const int ADDRESS_POWER = 1;
		private const int ADDRESS_HOME = 2;
		private const int ADDRESS_LIGHT = 3;
		private const int ADDRESS_VOL_UP = 4;
		private const int ADDRESS_VOL_DOWN = 5;

		private readonly SafeCriticalSection m_RefreshSection;

		private IVolumePresenter m_CachedVolumePresenter;
		private IPowerDeviceControl m_PowerControl;
		private readonly IHardButtonBacklightControl m_ButtonsControl;
		private IVolumeDeviceControl m_VolumeControl;

		/// <summary>
		/// Gets the popup volume presenter.
		/// </summary>
		private IVolumePresenter VolumePresenter
		{
			get
			{
				return m_CachedVolumePresenter ?? (m_CachedVolumePresenter = Navigation.LazyLoadPresenter<IVolumePresenter>());
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public HardButtonsPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_ButtonsControl = GetHardButtonBacklightControl(views);
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
				if (m_ButtonsControl == null)
					return;

				bool isInMeeting = Room != null && Room.IsInMeeting;
				bool hasVolumeControl = m_VolumeControl != null && (m_PowerControl == null || m_PowerControl.IsPowered);

				m_ButtonsControl.SetBacklightEnabled(ADDRESS_POWER, isInMeeting);
				m_ButtonsControl.SetBacklightEnabled(ADDRESS_HOME, isInMeeting);
				m_ButtonsControl.SetBacklightEnabled(ADDRESS_LIGHT, false);
				m_ButtonsControl.SetBacklightEnabled(ADDRESS_VOL_UP, hasVolumeControl);
				m_ButtonsControl.SetBacklightEnabled(ADDRESS_VOL_DOWN, hasVolumeControl);
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
		private static IHardButtonBacklightControl GetHardButtonBacklightControl(IUiViewFactory viewFactory)
		{
			return viewFactory.Panel.Controls.GetControl<IHardButtonBacklightControl>();
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
			if (Room != null)
				Navigation.NavigateTo<IConfirmEndMeetingPresenter>();
		}

		/// <summary>
		/// Called when the user presses the home button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnHomeButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Navigation.NavigateTo<IConfirmEndMeetingPresenter>();
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
			VolumePresenter.VolumeControl = m_VolumeControl;
			if (VolumePresenter.VolumeControl != null)
				VolumePresenter.VolumeUp();
		}

		/// <summary>
		/// Called when the user presses the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeDownButtonPressed(object sender, EventArgs eventArgs)
		{
			VolumePresenter.VolumeControl = m_VolumeControl;
			if (VolumePresenter.VolumeControl != null)
				VolumePresenter.VolumeDown();
		}

		/// <summary>
		/// Called when the user releases a volume button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeButtonReleased(object sender, EventArgs eventArgs)
		{
			VolumePresenter.Release();
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

			m_VolumeControl = room.GetVolumeControl();
			if (m_VolumeControl == null)
				return;

			IDeviceBase parent = m_VolumeControl.Parent;
			if (parent == null)
				return;

			m_PowerControl = parent.Controls.GetControl<IPowerDeviceControl>();
			if (m_PowerControl == null)
				return;

			m_PowerControl.OnIsPoweredChanged += PowerControlOnIsPoweredChanged;
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

			m_VolumeControl = null;

			if (m_PowerControl != null)
				m_PowerControl.OnIsPoweredChanged -= PowerControlOnIsPoweredChanged;
			m_PowerControl = null;
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		private void PowerControlOnIsPoweredChanged(object sender, PowerDeviceControlPowerStateApiEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
