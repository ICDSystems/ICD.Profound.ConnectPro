using ICD.Connect.Audio.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common
{
	public interface IVolumePresenter : IUiPresenter<IVolumeView>
	{
		/// <summary>
		/// Gets/sets the volume control.
		/// </summary>
		IVolumeDeviceControl VolumeControl { get; set; }

		/// <summary>
		/// Begins ramping the device volume up.
		/// </summary>
		void VolumeUp();

		/// <summary>
		/// Begins ramping the device volume down.
		/// </summary>
		void VolumeDown();

		/// <summary>
		/// Stops ramping the device volume.
		/// </summary>
		void Release();
	}
}
