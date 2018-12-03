﻿using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.Controls.Mute;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Options;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Options
{
	public sealed class OptionVolumePresenter : AbstractOptionPresenter<IOptionVolumeView>, IOptionVolumePresenter
	{
		private readonly IVolumePresenter m_Menu;

		private IVolumeDeviceControl m_SubscribedVolumeControl;
		private IVolumeMuteFeedbackDeviceControl m_SubscribedMuteFeedbackControl;
		private IPowerDeviceControl m_SubscribedPowerControl;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OptionVolumePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
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
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IOptionVolumeView view)
		{
			base.Refresh(view);

			view.SetButtonEnabled(m_SubscribedPowerControl == null || m_SubscribedPowerControl.IsPowered);
		}

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		protected override bool GetActive()
		{
			return m_SubscribedMuteFeedbackControl != null && m_SubscribedMuteFeedbackControl.VolumeIsMuted;
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
		/// Gets the volume control for the current room.
		/// </summary>
		/// <returns></returns>
		private static IVolumeMuteFeedbackDeviceControl GetMuteFeedbackControl(IConnectProRoom room)
		{
			IVolumeDeviceControl volumeControl = GetVolumeControl(room);
			if (volumeControl == null)
				return null;

			if (volumeControl is IVolumeMuteFeedbackDeviceControl)
				return volumeControl as IVolumeMuteFeedbackDeviceControl;

			return volumeControl.Parent.Controls.GetControl<IVolumeMuteFeedbackDeviceControl>();
		}

		#region View Callbacks

		/// <summary>
		/// Called when the user presses the option button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Menu.VolumeControl = m_SubscribedVolumeControl;
			m_Menu.ShowView(!m_Menu.IsViewVisible);
		}

		#endregion

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

			m_SubscribedMuteFeedbackControl = GetMuteFeedbackControl(room);
			if (m_SubscribedMuteFeedbackControl != null)
				m_SubscribedMuteFeedbackControl.OnMuteStateChanged += SubscribedMuteFeedbackControlOnMuteStateChanged;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;

			IDeviceBase parent = m_SubscribedVolumeControl == null ? null : m_SubscribedVolumeControl.Parent;
			m_SubscribedPowerControl = parent == null ? null : parent.Controls.GetControl<IPowerDeviceControl>();
			if (m_SubscribedPowerControl != null)
				m_SubscribedPowerControl.OnIsPoweredChanged += SubscribedPowerControlOnIsPoweredChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			m_SubscribedVolumeControl = null;

			if (m_SubscribedMuteFeedbackControl != null)
				m_SubscribedMuteFeedbackControl.OnMuteStateChanged -= SubscribedMuteFeedbackControlOnMuteStateChanged;
			m_SubscribedMuteFeedbackControl = null;

			if (m_SubscribedPowerControl != null)
				m_SubscribedPowerControl.OnIsPoweredChanged -= SubscribedPowerControlOnIsPoweredChanged;
			m_SubscribedPowerControl = null;

			if (room != null)
				room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
		}

		private void SubscribedPowerControlOnIsPoweredChanged(object sender, PowerDeviceControlPowerStateApiEventArgs powerDeviceControlPowerStateApiEventArgs)
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

		private void SubscribedMuteFeedbackControlOnMuteStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
