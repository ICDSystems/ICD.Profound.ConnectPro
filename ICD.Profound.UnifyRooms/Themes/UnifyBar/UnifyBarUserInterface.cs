using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.Repeaters;
using ICD.Connect.Audio.Utils;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Lighting;
using ICD.Connect.Lighting.RoomInterface;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar
{
	public sealed class UnifyBarUserInterface : AbstractUserInterface
	{
		private readonly UnifyBarDevice m_UnifyBar;
		private readonly UnifyRoomsTheme m_Theme;
		private readonly VolumeRepeater m_VolumeRepeater;
		private readonly VolumePointHelper m_VolumePointHelper;

		private ICommercialRoom m_Room;
		private IConferenceManager m_SubscribedConferenceManager;
		private ILightingRoomInterfaceDevice m_LightingInterface;

		#region Properties

		public UnifyBarDevice UnifyBar { get { return m_UnifyBar; } }

		/// <summary>
		/// Gets the room attached to this UI.
		/// </summary>
		public override IRoom Room { get { return m_Room; } }

		/// <summary>
		/// Gets the target instance attached to this UI.
		/// </summary>
		public override object Target { get { return m_UnifyBar; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="unifyBar"></param>
		/// <param name="theme"></param>
		public UnifyBarUserInterface([NotNull] UnifyBarDevice unifyBar, [NotNull] UnifyRoomsTheme theme)
		{
			if (unifyBar == null)
				throw new ArgumentNullException("unifyBar");

			if (theme == null)
				throw new ArgumentNullException("theme");

			m_UnifyBar = unifyBar;
			m_Theme = theme;
			m_VolumeRepeater = new VolumeRepeater();
			m_VolumePointHelper = new VolumePointHelper();

			Subscribe(m_UnifyBar);
			Subscribe(m_VolumePointHelper);
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			Unsubscribe(m_UnifyBar);
			SetRoom(null);

			Unsubscribe(m_VolumePointHelper);
			m_VolumePointHelper.Dispose();
		}

		/// <summary>
		/// Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IRoom room)
		{
			SetRoom(room as ICommercialRoom);
		}

		/// <summary>
		/// Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(ICommercialRoom room)
		{
			if (room == m_Room)
				return;

			Unsubscribe(m_Room);
			m_Room = room;
			Subscribe(m_Room);

			UpdateVolumePoint();
			UpdateUnifyBar();
		}

		/// <summary>
		/// Tells the UI that it should be considered ready to use.
		/// For example updating the online join on a panel or starting a long-running process that should be delayed.
		/// </summary>
		public override void Activate()
		{
			UpdateUnifyBar();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Pushes feedback to the Unify Bar device.
		/// </summary>
		private void UpdateUnifyBar()
		{
			if (!m_UnifyBar.IsConnected)
				return;

			bool? isPowered = m_Room == null ? (bool?)null : m_Room.IsAwake;

			bool? isPrivacyMuted =
				m_SubscribedConferenceManager == null //|| !m_SubscribedConferenceManager.CanPrivacyMute()
					? (bool?)null
					: m_SubscribedConferenceManager.PrivacyMuted;

			bool? isMuted =
				m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Mute)
					? m_VolumePointHelper.IsMuted
					: (bool?)null;

			LightingProcessorControl[] presets =
				m_LightingInterface == null ? null : m_LightingInterface.GetPresets().ToArray();
			bool? lights =
				presets == null || presets.Length != 2
					? (bool?)null
					: presets[1].Id == m_LightingInterface.GetPreset();

			bool volumeEnabled = m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment);

			m_UnifyBar.SetIsPoweredFeedback(isPowered);
			m_UnifyBar.SetIsMutedFeedback(isMuted);
			m_UnifyBar.SetVolumeRampEnabled(volumeEnabled);
			m_UnifyBar.SetIsPrivacyMutedFeedback(isPrivacyMuted);
			m_UnifyBar.SetLightsFeedback(lights);
		}

		/// <summary>
		/// Updates the volume point that is being manipulated by the UI.
		/// </summary>
		private void UpdateVolumePoint()
		{
			m_VolumePointHelper.VolumePoint = Room == null ? null : Room.GetContextualVolumePoints().FirstOrDefault();
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Subscribe([CanBeNull] ICommercialRoom room)
		{
			if (room == null)
				return;

			room.OnIsAwakeStateChanged += RoomOnIsAwakeStateChanged;

			m_SubscribedConferenceManager = room.ConferenceManager;
			if (m_SubscribedConferenceManager != null)
				m_SubscribedConferenceManager.OnPrivacyMuteStatusChange += ConferenceManagerOnPrivacyMuteStatusChange;

			m_LightingInterface = room.Originators.GetInstanceRecursive<ILightingRoomInterfaceDevice>();
			Subscribe(m_LightingInterface);
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Unsubscribe([CanBeNull] ICommercialRoom room)
		{
			if (room == null)
				return;

			room.OnIsAwakeStateChanged -= RoomOnIsAwakeStateChanged;

			if (m_SubscribedConferenceManager != null)
				m_SubscribedConferenceManager.OnPrivacyMuteStatusChange -= ConferenceManagerOnPrivacyMuteStatusChange;
			m_SubscribedConferenceManager = null;

			Unsubscribe(m_LightingInterface);
			m_LightingInterface = null;
		}

		/// <summary>
		/// Called when the room awake state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RoomOnIsAwakeStateChanged(object sender, BoolEventArgs e)
		{
			UpdateUnifyBar();
		}

		/// <summary>
		/// Called when the 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ConferenceManagerOnPrivacyMuteStatusChange(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateUnifyBar();
		}

		#endregion

		#region Unify Bar Callbacks

		/// <summary>
		/// Subscribe to the unify bar events.
		/// </summary>
		/// <param name="unifyBar"></param>
		private void Subscribe([NotNull] UnifyBarDevice unifyBar)
		{
			if (unifyBar == null)
				throw new ArgumentNullException("unifyBar");

			unifyBar.OnIsConnectedChanged += UnifyBarOnIsConnectedChanged;
			unifyBar.OnPowerButtonPressed += UnifyBarOnPowerButtonPressed;
			unifyBar.OnVolumeUpButtonPressed += UnifyBarOnVolumeUpButtonPressed;
			unifyBar.OnVolumeDownButtonPressed += UnifyBarOnVolumeDownButtonPressed;
			unifyBar.OnVolumeMuteButtonPressed += UnifyBarOnVolumeMuteButtonPressed;
			unifyBar.OnPrivacyMuteButtonPressed += UnifyBarOnPrivacyMuteButtonPressed;
			unifyBar.OnLightsButtonPressed += UnifyBarOnLightsButtonPressed;
			unifyBar.OnPageChanged += UnifyBarOnPageChanged;
		}

		/// <summary>
		/// Unsubscribe from the unify bar events.
		/// </summary>
		/// <param name="unifyBar"></param>
		private void Unsubscribe(UnifyBarDevice unifyBar)
		{
			if (unifyBar == null)
				throw new ArgumentNullException("unifyBar");

			unifyBar.OnIsConnectedChanged -= UnifyBarOnIsConnectedChanged;
			unifyBar.OnPowerButtonPressed -= UnifyBarOnPowerButtonPressed;
			unifyBar.OnVolumeUpButtonPressed -= UnifyBarOnVolumeUpButtonPressed;
			unifyBar.OnVolumeDownButtonPressed -= UnifyBarOnVolumeDownButtonPressed;
			unifyBar.OnVolumeMuteButtonPressed -= UnifyBarOnVolumeMuteButtonPressed;
			unifyBar.OnPrivacyMuteButtonPressed -= UnifyBarOnPrivacyMuteButtonPressed;
			unifyBar.OnLightsButtonPressed -= UnifyBarOnLightsButtonPressed;
			unifyBar.OnPageChanged -= UnifyBarOnPageChanged;
		}

		/// <summary>
		/// Called when we connect/disconnect to the unify bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnifyBarOnIsConnectedChanged(object sender, BoolEventArgs e)
		{
			UpdateUnifyBar();
		}

		/// <summary>
		/// Called when the user presses the power button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnifyBarOnPowerButtonPressed(object sender, BoolEventArgs e)
		{
			if (!e.Data)
				return;

			if (m_Room == null)
				return;

			if (m_Room.IsAwake)
				m_Room.Sleep();
			else
				m_Room.Wake();
		}

		/// <summary>
		/// Called when the user presses the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnifyBarOnVolumeUpButtonPressed(object sender, BoolEventArgs e)
		{
			if (e.Data)
			{
				if (m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment))
					m_VolumeRepeater.VolumeUpHold(m_VolumePointHelper.VolumePoint);
			}
			else
			{
				m_VolumeRepeater.Release();
			}
		}

		/// <summary>
		/// Called when the user presses the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnifyBarOnVolumeDownButtonPressed(object sender, BoolEventArgs e)
		{
			if (e.Data)
			{
				if (m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment))
					m_VolumeRepeater.VolumeDownHold(m_VolumePointHelper.VolumePoint);
			}
			else
			{
				m_VolumeRepeater.Release();
			}
		}

		/// <summary>
		/// Called when the user presses the volume mute button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnifyBarOnVolumeMuteButtonPressed(object sender, BoolEventArgs e)
		{
			if (e.Data)
				m_VolumePointHelper.ToggleIsMuted();
		}

		/// <summary>
		/// Called when the user presses the privacy mute button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnifyBarOnPrivacyMuteButtonPressed(object sender, BoolEventArgs e)
		{
			if (m_SubscribedConferenceManager == null)
				return;

			if (e.Data)
				m_SubscribedConferenceManager.TogglePrivacyMute();
		}

		/// <summary>
		/// Called when the user presses the lights button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnifyBarOnLightsButtonPressed(object sender, BoolEventArgs e)
		{
			if (m_LightingInterface == null)
				return;

			if (!e.Data)
				return;

			LightingProcessorControl[] presets = m_LightingInterface.GetPresets().ToArray();
			if (presets.Length != 2)
				return;

			LightingProcessorControl off = presets[0];
			LightingProcessorControl on = presets[1];
			
			if (m_LightingInterface.GetPreset() == on.Id)
				m_LightingInterface.SetPreset(off.Id);
			else
				m_LightingInterface.SetPreset(on.Id);
		}

		/// <summary>
		/// Called when the user changes pages.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnifyBarOnPageChanged(object sender, GenericEventArgs<UnifyBarDevice.ePage> e)
		{
		}

		#endregion

		#region Volume Point Helper Callbacks

		/// <summary>
		/// Subscribe to the volume point helper.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		private void Subscribe(VolumePointHelper volumePointHelper)
		{
			volumePointHelper.OnVolumeControlIsMutedChanged += VolumePointHelperOnVolumeControlIsMutedChanged;
			volumePointHelper.OnVolumeControlSupportedVolumeFeaturesChanged += VolumePointHelperOnVolumeControlSupportedVolumeFeaturesChanged;
		}

		/// <summary>
		/// Unsubscribe from the volume point helper.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		private void Unsubscribe(VolumePointHelper volumePointHelper)
		{
			volumePointHelper.OnVolumeControlIsMutedChanged -= VolumePointHelperOnVolumeControlIsMutedChanged;
			volumePointHelper.OnVolumeControlSupportedVolumeFeaturesChanged -= VolumePointHelperOnVolumeControlSupportedVolumeFeaturesChanged;
		}

		/// <summary>
		/// Called when the volume control mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void VolumePointHelperOnVolumeControlIsMutedChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateUnifyBar();
		}

		/// <summary>
		/// Called when the available volume features change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VolumePointHelperOnVolumeControlSupportedVolumeFeaturesChanged(object sender, GenericEventArgs<eVolumeFeatures> e)
		{
			UpdateUnifyBar();
		}

		#endregion

		#region LightingRoomInterface Callbacks

		/// <summary>
		/// Subscribe to the lighting device.
		/// </summary>
		/// <param name="device"></param>
		private void Subscribe(ILightingRoomInterfaceDevice device)
		{
			if (device == null)
				return;

			device.OnControlsChanged += LightingDeviceOnControlsChanged;
			device.OnPresetChanged += LightingDeviceOnPresetChanged;
		}

		/// <summary>
		/// Unsubscribe from the lighting device.
		/// </summary>
		/// <param name="device"></param>
		private void Unsubscribe(ILightingRoomInterfaceDevice device)
		{
			if (device == null)
				return;

			device.OnControlsChanged -= LightingDeviceOnControlsChanged;
			device.OnPresetChanged -= LightingDeviceOnPresetChanged;
		}

		/// <summary>
		/// Called when a lighting control is added to or removed from the lighting device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightingDeviceOnControlsChanged(object sender, EventArgs args)
		{
			UpdateUnifyBar();
		}

		/// <summary>
		/// Called when the lighting presets change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightingDeviceOnPresetChanged(object sender, GenericEventArgs<int?> args)
		{
			UpdateUnifyBar();
		}

		#endregion
	}
}
