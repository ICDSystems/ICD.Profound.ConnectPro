using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras
{
	public interface ICameraControlView : IUiView
	{
		#region Events

		/// <summary>
		/// Raised when the center D-Pad button is released.
		/// </summary>
		event EventHandler OnCameraHomeButtonReleased;

		/// <summary>
		/// Raised when the center D-Pad button is held.
		/// </summary>
		event EventHandler OnCameraHomeButtonHeld;

		/// <summary>
		/// Raised when the up button is pressed.
		/// </summary>
		event EventHandler OnCameraMoveUpButtonPressed;

		/// <summary>
		/// Raised when the left button is pressed.
		/// </summary>
		event EventHandler OnCameraMoveLeftButtonPressed;

		/// <summary>
		/// Raised when the right button is pressed.
		/// </summary>
		event EventHandler OnCameraMoveRightButtonPressed;

		/// <summary>
		/// Raised when the down button is pressed.
		/// </summary>
		event EventHandler OnCameraMoveDownButtonPressed;

		/// <summary>
		/// Raised when the zoom in button is pressed.
		/// </summary>
		event EventHandler OnCameraZoomInButtonPressed;

		/// <summary>
		/// Raised when the zoom out button is pressed.
		/// </summary>
		event EventHandler OnCameraZoomOutButtonPressed;

		/// <summary>
		/// Raised when any camera PTZ button is released (i.e. stop the camera).
		/// </summary>
		event EventHandler OnCameraPtzButtonReleased;

		/// <summary>
		/// Raised when a preset button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnPresetButtonReleased;

		/// <summary>
		/// Raised when a preset button is held.
		/// </summary>
		event EventHandler<UShortEventArgs> OnPresetButtonHeld;

		/// <summary>
		/// Raised when a camera button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnCameraButtonPressed;

		#endregion

		#region Methods

		/// <summary>
		/// Sets the enabled state of the directional buttons.
		/// </summary>
		/// <param name="enabled"></param>
		void SetDPadButtonsEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the preset buttons.
		/// </summary>
		/// <param name="enabled"></param>
		void SetPresetButtonsEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the zoom buttons.
		/// </summary>
		/// <param name="enabled"></param>
		void SetZoomButtonsEnabled(bool enabled);

		/// <summary>
		/// Sets the label for the preset button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		void SetPresetButtonLabel(ushort index, string label);

		/// <summary>
		/// Sets the visibility of the "Preset Stored" label.
		/// </summary>
		/// <param name="visible"></param>
		void SetPresetStoredLabelVisibility(bool visible);

		/// <summary>
		/// Sets the camera selection list labels.
		/// </summary>
		/// <param name="labels"></param>
		void SetCameraLabels(IEnumerable<string> labels);

		/// <summary>
		/// Sets the selection state of the camera button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetCameraSelected(ushort index, bool selected);

		#endregion
	}
}
