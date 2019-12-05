using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.EventArguments;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions
{
	[PresenterBinding(typeof(IFloatingActionVolumePresenter))]
	public sealed class FloatingActionVolumePresenter : AbstractFloatingActionPresenter<IFloatingActionVolumeView>,
	                                                    IFloatingActionVolumePresenter
	{
		private readonly IVolumePresenter m_Menu;

		private IVolumeDeviceControl m_SubscribedVolumeControl;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public FloatingActionVolumePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_Menu = Navigation.LazyLoadPresenter<IVolumePresenter>();
			Subscribe(m_Menu);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_Menu);
		}

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		protected override bool GetActive()
		{
			return m_SubscribedVolumeControl != null && m_SubscribedVolumeControl.IsMuted;
		}

		/// <summary>
		/// Override to get the enabled state for the button.
		/// </summary>
		/// <returns></returns>
		protected override bool GetEnabled()
		{
			return m_SubscribedVolumeControl == null || m_SubscribedVolumeControl.ControlAvailable;
		}

		/// <summary>
		/// Gets the volume control for the current room.
		/// </summary>
		/// <returns></returns>
		private static IVolumeDeviceControl GetVolumeControl(IConnectProRoom room)
		{
			return room == null ? null : room.GetVolumeControl();
		}

		/// <summary>
		/// Override to handle the button press.
		/// </summary>
		protected override void HandleButtonPress()
		{
			m_Menu.VolumeControl = m_SubscribedVolumeControl;
			m_Menu.ShowView(!m_Menu.IsViewVisible);
		}

		#region Navigation Callbacks

		/// <summary>
		/// Subscribe to the menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Subscribe(IVolumePresenter menu)
		{
			menu.OnViewVisibilityChanged += MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Unsubscribe(IVolumePresenter menu)
		{
			menu.OnViewVisibilityChanged -= MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Called when the menu visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void MenuOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
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

			m_SubscribedVolumeControl = GetVolumeControl(room);

			if (m_SubscribedVolumeControl != null)
			{
				m_SubscribedVolumeControl.OnControlAvailableChanged += SubscribedVolumeControlOnControlAvailableChanged;
				m_SubscribedVolumeControl.OnIsMutedChanged += SubscribedVolumeControlOnIsMutedChanged;
			}

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedVolumeControl != null)
			{
				m_SubscribedVolumeControl.OnControlAvailableChanged -= SubscribedVolumeControlOnControlAvailableChanged;
				m_SubscribedVolumeControl.OnIsMutedChanged -= SubscribedVolumeControlOnIsMutedChanged;
			}
			m_SubscribedVolumeControl = null;

			if (room != null)
				room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
		}

		private void SubscribedVolumeControlOnControlAvailableChanged(object sender, DeviceControlAvailableApiEventArgs e)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the room enters/leaves meeting state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs eventArgs)
		{
			ShowView(eventArgs.Data && m_SubscribedVolumeControl != null);
		}

		private void SubscribedVolumeControlOnIsMutedChanged(object sender,
		                                                             VolumeControlIsMutedChangedApiEventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
