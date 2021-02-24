using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.Repeaters;
using ICD.Connect.Audio.Utils;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons
{
	public sealed class UnifyBarVolumeUpButtonUi : AbstractVolumeUnifyBarButtonUi
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		/// <param name="volumeRepeater"></param>
		public UnifyBarVolumeUpButtonUi([NotNull] VolumePointHelper volumePointHelper,
		                                [NotNull] VolumeRepeater volumeRepeater)
			: base(volumePointHelper, volumeRepeater)
		{
			Icon = eMainButtonIcon.VolumeUp;
			Type = eMainButtonType.Normal;
			Label = "VOL UP";

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
					VolumeRepeater.VolumeUpHold(VolumePointHelper.VolumePoint);
			}
			else
			{
				VolumeRepeater.Release();
			}
		}
	}
}
