using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Camera
{
	public interface ICameraControlView : ITouchDisplayView
	{
		#region Events

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
		void SetPresetButtonsVisible(bool enabled);

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

		#endregion
	}
}
