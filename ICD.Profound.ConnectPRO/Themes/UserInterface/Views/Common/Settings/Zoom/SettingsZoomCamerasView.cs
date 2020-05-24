using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Zoom
{
    [ViewBinding(typeof(ISettingsZoomCamerasView))]
    public sealed partial class SettingsZoomCamerasView : AbstractUiView, ISettingsZoomCamerasView
    {
        #region Events

        /// <summary>
        /// Raised when the user presses a camera device button from the list.
        /// </summary>
        public event EventHandler<UShortEventArgs> OnCameraDevicePressed;

        /// <summary>
        /// Raised when the user presses a Zoom Usb ID button from the list.
        /// </summary>
        public event EventHandler<UShortEventArgs> OnZoomUsbIdPressed;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="theme"></param>
        public SettingsZoomCamerasView(ISigInputOutput panel, ConnectProTheme theme) 
            : base(panel, theme)
        {
        }

        /// <summary>
        /// Release resources.
        /// </summary>
        public override void Dispose()
        {
            OnCameraDevicePressed = null;
            OnZoomUsbIdPressed = null;

            base.Dispose();
        }

        #region Methods

        public void SetCameraLabels(IEnumerable<string> labels)
        {
            m_CameraDeviceButtonList.SetItemLabels(labels.ToArray());
        }

        public void SetUsbIdLabels(IEnumerable<string> labels)
        {
            m_UsbIdButtonList.SetItemLabels(labels.ToArray());
        }

        /// <summary>
        /// Sets the selection state of the camera device button at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="selected"></param>
        public void SetCameraDeviceButtonSelected(ushort index, bool selected)
        {
            m_CameraDeviceButtonList.SetItemSelected(index, selected);
        }

        /// <summary>
        /// Sets the selection state of the Zoom Usb ID button at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="selected"></param>
        public void SetZoomUsbIdButtonSelected(ushort index, bool selected)
        {
            m_UsbIdButtonList.SetItemSelected(index, selected);
        }

        /// <summary>
        /// Sets the enabled state of the Zoom Usb ID button at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="enabled"></param>
        public void SetZoomUsbIdButtonEnabled(ushort index, bool enabled)
        {
            m_UsbIdButtonList.SetItemEnabled(index, enabled);
        }

        /// <summary>
        /// Sets the enabled state of the Zoom Usb ID buttons.
        /// </summary>
        /// <param name="enabled"></param>
        public void SetZoomUsbIdButtonsEnabled(bool enabled)
        {
            m_UsbIdButtonList.Enable(enabled);
        }

        #endregion

        #region Control Callbacks

        /// <summary>
        /// Subscribes to the view controls.
        /// </summary>
        protected override void SubscribeControls()
        {
            base.SubscribeControls();

            m_CameraDeviceButtonList.OnButtonClicked += CameraDeviceButtonListOnButtonClicked;
            m_UsbIdButtonList.OnButtonClicked += UsbIdButtonListOnButtonClicked;
        }

        /// <summary>
        /// Unsubscribes from the view controls.
        /// </summary>
        protected override void UnsubscribeControls()
        {
            base.UnsubscribeControls();

            m_CameraDeviceButtonList.OnButtonClicked -= CameraDeviceButtonListOnButtonClicked;
            m_UsbIdButtonList.OnButtonClicked -= UsbIdButtonListOnButtonClicked;
        }

        /// <summary>
        /// Called when the user clicks a camera device button from the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraDeviceButtonListOnButtonClicked(object sender, UShortEventArgs e)
        {
            OnCameraDevicePressed.Raise(this, new UShortEventArgs(e.Data));
        }

        /// <summary>
        /// Called when the user clicks a Zoom Usb ID button from the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsbIdButtonListOnButtonClicked(object sender, UShortEventArgs e)
        {
            OnZoomUsbIdPressed.Raise(this, new UShortEventArgs(e.Data));
        }

        #endregion
    }
}
