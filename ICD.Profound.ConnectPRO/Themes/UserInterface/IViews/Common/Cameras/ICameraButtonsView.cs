using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras
{
	public interface ICameraButtonsView : IPopupView
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
		/// Sets the visible state of a camera configuration button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetCameraConfigurationButtonVisible(ushort index, bool visible);
	}
}
