using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom
{
	public interface ISettingsZoomCamerasView : IUiView
    {
        /// <summary>
        /// Raised when the user presses a camera device button from the list.
        /// </summary>
        event EventHandler<UShortEventArgs> OnCameraDevicePressed;

        /// <summary>
        /// Raised when the user presses a Zoom Usb ID button from the list.
        /// </summary>
        event EventHandler<UShortEventArgs> OnZoomUsbIdPressed;

        /// <summary>
        /// Sets the labels for the camera device dynamic button list.
        /// </summary>
        /// <param name="labels"></param>
        void SetCameraLabels(IEnumerable<string> labels);

        /// <summary>
        /// Sets the labels for the Zoom Usb id dynamic button list.
        /// </summary>
        /// <param name="labels"></param>
        void SetUsbIdLabels(IEnumerable<string> labels);

        /// <summary>
        /// Sets the selection state of the camera device button at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="selected"></param>
        void SetCameraDeviceButtonSelected(ushort index, bool selected);

        /// <summary>
        /// Sets the selection state of the Zoom Usb ID button at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="selected"></param>
        void SetZoomUsbIdButtonSelected(ushort index, bool selected);

        /// <summary>
        /// Sets the enabled state of the Zoom Usb ID button at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="enabled"></param>
        void SetZoomUsbIdButtonEnabled(ushort index, bool enabled);

        /// <summary>
        /// Sets the enabled state of the Zoom Usb ID buttons.
        /// </summary>
        /// <param name="enabled"></param>
        void SetZoomUsbIdButtonsEnabled(bool enabled);
    }
}
