using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Windows;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Zoom;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom;
using ICD.Profound.ConnectPROCommon.SettingsTree.Zoom;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Zoom
{
    [PresenterBinding(typeof(ISettingsZoomCamerasPresenter))]
	public sealed class SettingsZoomCamerasPresenter : AbstractSettingsNodeBasePresenter<ISettingsZoomCamerasView, ZoomCameraSettingsLeaf>, ISettingsZoomCamerasPresenter
    {
        private readonly SafeCriticalSection m_RefreshSection;

        private IDeviceBase m_SelectedCamera;
        private IDeviceBase[] m_UsbCameras;
        private WindowsDevicePathInfo[] m_UsbIds;

        public IDeviceBase SelectedCamera
        {
            get { return m_SelectedCamera; }
            private set
            {
                if (value == m_SelectedCamera)
                    return;

                m_SelectedCamera = value;

                RefreshIfVisible();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="views"></param>
        /// <param name="theme"></param>
        public SettingsZoomCamerasPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme) 
            : base(nav, views, theme)
        {
            m_RefreshSection = new SafeCriticalSection();

            m_UsbCameras = new IDeviceBase[0];
            m_UsbIds = new WindowsDevicePathInfo[0];
        }

        protected override void Refresh(ISettingsZoomCamerasView view)
        {
            base.Refresh(view);

            m_RefreshSection.Enter();

            try
            {
                // Update labels
                view.SetCameraLabels(m_UsbCameras.Select(c => GetCameraLabel(c)));
                view.SetUsbIdLabels(m_UsbIds.Select(u => GetUsbLabel(u)));

                // Set Camera Device selection state.
                for (ushort index = 0; index < m_UsbCameras.Length; index++)
                    view.SetCameraDeviceButtonSelected(index, m_SelectedCamera == m_UsbCameras[index]);

                // Set the USB ID selection state.
                WindowsDevicePathInfo? selectedUsbId =
					Node == null || m_SelectedCamera == null ? null : Node.GetUsbIdForCamera(m_SelectedCamera);
                for (ushort index = 0; index < m_UsbIds.Length; index++)
                    view.SetZoomUsbIdButtonSelected(index, selectedUsbId == m_UsbIds[index]);

                // Set the USB ID enabled state.
                bool anyEnabled = false;
                for (ushort index = 0; index < m_UsbIds.Length; index++)
                {
                    bool enabled =
						Node != null &&
                        m_SelectedCamera != null &&
                        m_UsbCameras.Except(m_SelectedCamera)
									.All(c => Node.GetUsbIdForCamera(c) != m_UsbIds[index]);

                    anyEnabled |= enabled;

                    view.SetZoomUsbIdButtonEnabled(index, enabled);
                }

                view.SetZoomUsbIdButtonsEnabled(anyEnabled);
            }
            finally
            {
                m_RefreshSection.Leave();
            }
        }

        #region Private Methods

        private static string GetCameraLabel(IDeviceBase camera)
        {
            return string.IsNullOrEmpty(camera.CombineName) ? camera.Name : camera.CombineName;
        }

        private string GetUsbLabel(WindowsDevicePathInfo usbInfo)
        {
			return Node == null ? null : Node.GetUsbDeviceName(usbInfo);
        }

        private void UpdateUsbCameras()
        {
			m_UsbCameras = Node == null ? new IDeviceBase[0] : Node.GetCameraDevices().ToArray();
            RefreshIfVisible();
        }

        private void UpdateUsbIds()
        {
	        m_UsbIds =
		        Node == null
			        ? new WindowsDevicePathInfo[0]
			        : Node.GetUsbIds()
			              .OrderBy(u => GetUsbLabel(u))
			              .ToArray();
            RefreshIfVisible();
        }

        #endregion

        #region Settings Callbacks

        protected override void Subscribe(ZoomCameraSettingsLeaf node)
        {
            base.Subscribe(node);

            if (node == null)
                return;

            node.OnUsbCamerasChanged += SettingsOnUsbCamerasChanged;
            node.OnUsbIdsChanged += SettingsOnUsbIdsChanged;

            UpdateUsbCameras();
            UpdateUsbIds();
        }

		protected override void Unsubscribe(ZoomCameraSettingsLeaf node)
        {
            base.Unsubscribe(node);

            if (node == null)
                return;

            node.OnUsbCamerasChanged -= SettingsOnUsbCamerasChanged;
            node.OnUsbIdsChanged -= SettingsOnUsbIdsChanged;

            UpdateUsbCameras();
            UpdateUsbIds();
        }

        private void SettingsOnUsbCamerasChanged(object sender, EventArgs eventArgs)
        {
            UpdateUsbCameras();
            RefreshIfVisible();
        }

        private void SettingsOnUsbIdsChanged(object sender, EventArgs eventArgs)
        {
            UpdateUsbIds();
            RefreshIfVisible();
        }

        #endregion

        #region View Callbacks

        protected override void Subscribe(ISettingsZoomCamerasView view)
        {
            base.Subscribe(view);

            view.OnCameraDevicePressed += ViewOnCameraDevicePressed;
            view.OnZoomUsbIdPressed += ViewOnZoomUsbIdPressed;
        }

        protected override void Unsubscribe(ISettingsZoomCamerasView view)
        {
            base.Unsubscribe(view);

            view.OnCameraDevicePressed -= ViewOnCameraDevicePressed;
            view.OnZoomUsbIdPressed -= ViewOnZoomUsbIdPressed;
        }

        private void ViewOnCameraDevicePressed(object sender, UShortEventArgs e)
        {
            IDeviceBase camera;
            m_UsbCameras.TryElementAt(e.Data, out camera);
            SelectedCamera = camera == SelectedCamera ? null : camera;
        }

        private void ViewOnZoomUsbIdPressed(object sender, UShortEventArgs e)
        {
            if (Node == null || m_SelectedCamera == null)
                return;

            WindowsDevicePathInfo usbId;
            if (!m_UsbIds.TryElementAt(e.Data, out usbId))
                return;

			if (usbId == Node.GetUsbIdForCamera(m_SelectedCamera))
                Node.SetUsbIdForCamera(m_SelectedCamera, null);
            else
				Node.SetUsbIdForCamera(m_SelectedCamera, usbId);
        }

        protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
        {
            base.ViewOnVisibilityChanged(sender, args);

            SelectedCamera = null;
        }

        #endregion
    }
}
