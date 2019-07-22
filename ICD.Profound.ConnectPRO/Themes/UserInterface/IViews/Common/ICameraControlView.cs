using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface ICameraControlView : IUiView
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

		/// <summary>
		/// Raised when a camera button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnCameraButtonPressed;

		/// <summary>
		/// Raised when a tab button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnTabButtonPressed;

		#endregion

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
		
		/// <summary>
		/// Sets the selected state of a tab button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetTabSelected(ushort index, bool selected);
	}
}
