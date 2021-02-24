using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.Repeaters;
using ICD.Connect.Audio.Utils;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons
{
	public sealed class UnifyBarMuteButtonUi : AbstractVolumeUnifyBarButtonUi
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		/// <param name="volumeRepeater"></param>
		public UnifyBarMuteButtonUi([NotNull] VolumePointHelper volumePointHelper,
		                            [NotNull] VolumeRepeater volumeRepeater)
			: base(volumePointHelper, volumeRepeater)
		{
			Icon = eMainButtonIcon.VolumeMute;
			Type = eMainButtonType.Mute;
			Label = "VOL MUTE";

			UpdateState();
		}

		#region Private Methods

		/// <summary>
		/// Override to implement the button press behaviour.
		/// </summary>
		/// <param name="pressed"></param>
		protected override void HandleButtonPress(bool pressed)
		{
			if (pressed)
				VolumePointHelper.ToggleIsMuted();
		}

		/// <summary>
		/// Pushes the volume state to the device.
		/// </summary>
		protected override void UpdateState()
		{
			base.UpdateState();

			Enabled = VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Mute);
			Selected = VolumePointHelper.IsMuted;
		}

		#endregion

		#region VolumePointHelper Callbacks

		/// <summary>
		/// Subscribe to the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		protected override void Subscribe(VolumePointHelper volumePointHelper)
		{
			base.Subscribe(volumePointHelper);

			volumePointHelper.OnVolumeControlIsMutedChanged += VolumePointHelperOnVolumeControlIsMutedChanged;
		}

		/// <summary>
		/// Unsubscribe from the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		protected override void Unsubscribe(VolumePointHelper volumePointHelper)
		{
			base.Unsubscribe(volumePointHelper);

			volumePointHelper.OnVolumeControlIsMutedChanged -= VolumePointHelperOnVolumeControlIsMutedChanged;
		}

		/// <summary>
		/// Called when the volume control mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VolumePointHelperOnVolumeControlIsMutedChanged(object sender, BoolEventArgs e)
		{
			UpdateState();
		}

		#endregion
	}
}
