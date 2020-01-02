using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common
{
	public interface IVolumePresenter : IUiPresenter<IVolumeView>
	{
		/// <summary>
		/// Raised when volume control availability changes.
		/// </summary>
		event EventHandler<BoolEventArgs> OnControlAvailableChanged;

		/// <summary>
		/// Raised when volume control mute state changes.
		/// </summary>
		event EventHandler<BoolEventArgs> OnControlIsMutedChanged;
		
		/// <summary>
		/// Gets/sets the volume control.
		/// </summary>
		IVolumeDeviceControl VolumeControl { get; }

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
