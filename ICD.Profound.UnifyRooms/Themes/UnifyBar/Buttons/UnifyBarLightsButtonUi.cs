using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Lighting;
using ICD.Connect.Lighting.RoomInterface;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons
{
	public sealed class UnifyBarLightsButtonUi : AbstractUnifyBarButtonUi
	{
		[CanBeNull]
		private ILightingRoomInterfaceDevice m_LightingInterface;

		/// <summary>
		/// Gets/sets the lighting interface.
		/// </summary>
		[CanBeNull]
		private ILightingRoomInterfaceDevice LightingInterface
		{
			get { return m_LightingInterface; }
			set
			{
				if (value == m_LightingInterface)
					return;

				Unsubscribe(m_LightingInterface);
				m_LightingInterface = value;
				Subscribe(m_LightingInterface);

				UpdateLightingPreset();
				UpdateVisibility();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyBarLightsButtonUi()
		{
			Icon = eMainButtonIcon.Lights;
			Type = eMainButtonType.Normal;
			Label = "LIGHTS";

			UpdateLightingPreset();
		}

		#region Private Methods

		/// <summary>
		/// Sets the room.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(ICommercialRoom room)
		{
			base.SetRoom(room);

			LightingInterface = room == null ? null : room.Originators.GetInstanceRecursive<ILightingRoomInterfaceDevice>();
		}

		/// <summary>
		/// Override to implement the button press behaviour.
		/// </summary>
		/// <param name="pressed"></param>
		protected override void HandleButtonPress(bool pressed)
		{
			if (!pressed)
				return;

			if (m_LightingInterface == null)
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
		/// Pushes the current lighting preset state to the device.
		/// </summary>
		private void UpdateLightingPreset()
		{
			LightingProcessorControl[] presets =
				m_LightingInterface == null ? null : m_LightingInterface.GetPresets().ToArray();
			bool lights = presets != null &&
			              presets.Length == 2 &&
			              presets[1].Id == m_LightingInterface.GetPreset();

			Enabled = presets != null;
			Selected = lights;
		}

		/// <summary>
		/// Updates the visibility of the button.
		/// </summary>
		private void UpdateVisibility()
		{
			Visible = m_LightingInterface != null;
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
			UpdateLightingPreset();
		}

		/// <summary>
		/// Called when the lighting presets change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightingDeviceOnPresetChanged(object sender, GenericEventArgs<int?> args)
		{
			UpdateLightingPreset();
		}

		#endregion
	}
}
