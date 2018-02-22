using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcCameraView : IView
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
		/// Raised when any camera button is released (i.e. stop the camera).
		/// </summary>
		event EventHandler OnCameraButtonReleased;

		/// <summary>
		/// Raised when a preset button is pressed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnPresetButtonPressed;

		#endregion

		/// <summary>
		/// Sets the label for the preset button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		void SetPresetButtonLabel(ushort index, string label);
	}
}
