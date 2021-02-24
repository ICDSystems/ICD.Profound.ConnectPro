using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.Repeaters;
using ICD.Connect.Audio.Utils;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons
{
	public sealed class UnifyBarVolumeDownButtonUi : AbstractVolumeUnifyBarButtonUi
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		/// <param name="volumeRepeater"></param>
		public UnifyBarVolumeDownButtonUi([NotNull] VolumePointHelper volumePointHelper,
		                                  [NotNull] VolumeRepeater volumeRepeater)
			: base(volumePointHelper, volumeRepeater)
		{
			Icon = eMainButtonIcon.VolumeDown;
			Type = eMainButtonType.Normal;
			Label = "VOL DOWN";

			UpdateState();
		}

		/// <summary>
		/// Pushes the volume state to the device.
		/// </summary>
		protected override void UpdateState()
		{
			base.UpdateState();

			Enabled = VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment);
		}

		/// <summary>
		/// Override to implement the button press behaviour.
		/// </summary>
		/// <param name="pressed"></param>
		protected override void HandleButtonPress(bool pressed)
		{
			if (pressed)
			{
				if (VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment))
					VolumeRepeater.VolumeDownHold(VolumePointHelper.VolumePoint);
			}
			else
			{
				VolumeRepeater.Release();
			}
		}
	}
}
