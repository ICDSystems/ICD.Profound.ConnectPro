using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras
{
	public interface ICameraButtonsView : IUiView
	{
		/// <summary>
		/// Raised when one of the camera configuration buttons are pressed (Control, Active, or Layout).
		/// </summary>
		event EventHandler<UShortEventArgs> OnCameraConfigurationButtonPressed;

		/// <summary>
		/// Sets the selected state of a camera configuration button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetCameraConfigurationButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the enabled state of a camera configuration button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="enabled"></param>
		void SetCameraConfigurationButtonEnabled(ushort index, bool enabled);
	}
}
