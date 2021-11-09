using System;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.Repeaters;
using ICD.Connect.Audio.Utils;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons
{
	public abstract class AbstractVolumeUnifyBarButtonUi : AbstractUnifyBarButtonUi
	{
		private readonly VolumePointHelper m_VolumePointHelper;
		private readonly VolumeRepeater m_VolumeRepeater;

		#region Properties

		/// <summary>
		/// Gets the volume point helper.
		/// </summary>
		protected VolumePointHelper VolumePointHelper { get { return m_VolumePointHelper; } }

		/// <summary>
		/// Gets the volume repeater.
		/// </summary>
		protected VolumeRepeater VolumeRepeater { get { return m_VolumeRepeater; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		/// <param name="volumeRepeater"></param>
		protected AbstractVolumeUnifyBarButtonUi([NotNull] VolumePointHelper volumePointHelper,
		                                         [NotNull] VolumeRepeater volumeRepeater)
		{
			if (volumePointHelper == null)
				throw new ArgumentNullException("volumePointHelper");

			if (volumeRepeater == null)
				throw new ArgumentNullException("volumeRepeater");

			m_VolumePointHelper = volumePointHelper;
			m_VolumeRepeater = volumeRepeater;

			Subscribe(m_VolumePointHelper);

			UpdateVisibility();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_VolumePointHelper);
		}

		#region Private Methods

		/// <summary>
		/// Pushes the volume state to the device.
		/// </summary>
		protected virtual void UpdateState()
		{
		}

		/// <summary>
		/// Updates the visibility of the button.
		/// </summary>
		private void UpdateVisibility()
		{
			Visible = m_VolumePointHelper.VolumeControl != null;
		}

		#endregion

		#region VolumePointHelper Callbacks

		/// <summary>
		/// Subscribe to the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		protected virtual void Subscribe(VolumePointHelper volumePointHelper)
		{
			volumePointHelper.OnVolumeControlChanged += VolumePointHelperOnVolumeControlChanged;
			volumePointHelper.OnVolumeControlAvailableChanged += VolumePointHelperOnVolumeControlAvailableChanged;
			volumePointHelper.OnVolumeControlSupportedVolumeFeaturesChanged += VolumePointHelperOnVolumeControlSupportedVolumeFeaturesChanged;
		}

		/// <summary>
		/// Unsubscribe from the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		protected virtual void Unsubscribe(VolumePointHelper volumePointHelper)
		{
			volumePointHelper.OnVolumeControlChanged -= VolumePointHelperOnVolumeControlChanged;
			volumePointHelper.OnVolumeControlAvailableChanged -= VolumePointHelperOnVolumeControlAvailableChanged;
			volumePointHelper.OnVolumeControlSupportedVolumeFeaturesChanged -= VolumePointHelperOnVolumeControlSupportedVolumeFeaturesChanged;
		}

		/// <summary>
		/// Called when the available volume features change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VolumePointHelperOnVolumeControlAvailableChanged(object sender, BoolEventArgs e)
		{
			UpdateState();
			UpdateVisibility();
		}

		/// <summary>
		/// Called when the underlying volume control changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void VolumePointHelperOnVolumeControlChanged(object sender, GenericEventArgs<IVolumeDeviceControl> eventArgs)
		{
			UpdateState();
			UpdateVisibility();
		}

		/// <summary>
		/// Called when the underlying volume control changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void VolumePointHelperOnVolumeControlSupportedVolumeFeaturesChanged(object sender, GenericEventArgs<eVolumeFeatures> eventArgs)
		{
			UpdateState();
			UpdateVisibility();
		}

		#endregion
	}
}
