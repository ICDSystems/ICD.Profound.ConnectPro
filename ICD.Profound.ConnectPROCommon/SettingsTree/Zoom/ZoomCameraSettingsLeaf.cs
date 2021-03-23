using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Components.Camera;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Windows;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.Zoom
{
	public sealed class ZoomCameraSettingsLeaf : AbstractSettingsLeaf
	{
        public event EventHandler OnUsbCamerasChanged;
        public event EventHandler OnUsbIdsChanged;

        private readonly List<ZoomRoom> m_ZoomRooms;
        private readonly List<CameraComponent> m_CameraComponents;

		#region Properties

		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		public override bool Visible
		{
			get
			{
				return base.Visible &&
				       Room != null &&
				       Room.Originators.GetInstancesRecursive<ZoomRoom>().Any();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ZoomCameraSettingsLeaf()
		{
			m_ZoomRooms = new List<ZoomRoom>();
			m_CameraComponents = new List<CameraComponent>();

			Name = "Cameras";
			Icon = eSettingsIcon.ConferenceCamera;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
            OnUsbCamerasChanged = null;
            OnUsbIdsChanged = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the Usb Id associated with the camera device.
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="usbInfo"></param>
        public void SetUsbIdForCamera([NotNull] IDeviceBase camera, WindowsDevicePathInfo? usbInfo)
        {
			m_ZoomRooms.ForEach(z => z.SetUsbIdForCamera(camera, usbInfo));
			SetDirty(true);
        }

		/// <summary>
		/// Gets teh Usb Id associated with the camera device.
		/// </summary>
		/// <param name="camera"></param>
		/// <returns></returns>
        public WindowsDevicePathInfo? GetUsbIdForCamera([NotNull] IDeviceBase camera)
        {
            WindowsDevicePathInfo info;
            bool found = m_ZoomRooms.SelectMany(z => z.GetUsbCameras())
                                    .Where(kvp => kvp.Key == camera)
                                    .Select(kvp => kvp.Value)
                                    .TryFirst(out info);

            return found ? info : (WindowsDevicePathInfo?)null;

        }

        public string GetUsbDeviceName(WindowsDevicePathInfo usbInfo)
        {
            CameraInfo info;
            bool found = m_CameraComponents.SelectMany(cc => cc.GetCameras())
                                           .Where(c => new WindowsDevicePathInfo(c.UsbId).DeviceId == usbInfo.DeviceId)
                                           .TryFirst(out info);

	        return found
		        ? string.Format("{0} ({1})", info.Name, new WindowsDevicePathInfo(info.UsbId).DeviceId)
		        : string.Format("Unknown ({0})", usbInfo.DeviceId);
        }

		public IEnumerable<IDeviceBase> GetCameraDevices()
		{
			return Room == null
				       ? Enumerable.Empty<IDeviceBase>()
				       : Room.Originators
				             .GetInstancesRecursive<ICameraDevice>()
				             .Cast<IDeviceBase>();
		}

        public IEnumerable<WindowsDevicePathInfo> GetUsbIds()
        {
	        return m_ZoomRooms.SelectMany(z => z.GetUsbCameras())
	                          .Select(kvp => kvp.Value)
	                          .Concat(m_CameraComponents.SelectMany(cc => cc.GetCameras()
	                                                                        .Select(i => new WindowsDevicePathInfo(i.UsbId))))
	                          .Where(info => info != default(WindowsDevicePathInfo))
	                          .Distinct(d => d.DeviceId);
        }

        #endregion

		#region Private Methods

		private void Update()
		{
			UpdateUsbCameras();
		}

        private void UpdateUsbCameras()
        {
			OnUsbCamerasChanged.Raise(this);
			OnUsbIdsChanged.Raise(this);
        }

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			IEnumerable<ZoomRoom> zoomRooms =
				room == null
					? Enumerable.Empty<ZoomRoom>()
					: room.Originators.GetInstancesRecursive<ZoomRoom>();

			m_ZoomRooms.Clear();
			m_ZoomRooms.AddRange(zoomRooms);
			m_ZoomRooms.ForEach(Subscribe);

            IEnumerable<CameraComponent> cameraComponents =
                room == null
                    ? Enumerable.Empty<CameraComponent>()
                    : room.Originators
                          .GetInstancesRecursive<ZoomRoom>()
                          .Select(z => z.Components.GetComponent<CameraComponent>());

			m_CameraComponents.Clear();
			m_CameraComponents.AddRange(cameraComponents);
			m_CameraComponents.ForEach(Subscribe);

			Update();
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

            m_ZoomRooms.ForEach(Unsubscribe);
			m_ZoomRooms.Clear();

			m_CameraComponents.ForEach(Unsubscribe);
			m_CameraComponents.Clear();

			Update();
		}

		#endregion

        #region Camera Component Callbacks

		/// <summary>
		/// Subscribe to the camera component events.
		/// </summary>
		/// <param name="cameraComponent"></param>
        private void Subscribe(CameraComponent cameraComponent)
        {
			cameraComponent.OnCamerasUpdated += CameraComponentOnCamerasUpdated;
        }

		/// <summary>
		/// Unsubscribe from the camera component events.
		/// </summary>
		/// <param name="cameraComponent"></param>
        private void Unsubscribe(CameraComponent cameraComponent)
        {
            cameraComponent.OnCamerasUpdated -= CameraComponentOnCamerasUpdated;
        }

        private void CameraComponentOnCamerasUpdated(object sender, EventArgs e)
        {
			UpdateUsbCameras();
        }

        #endregion

		#region ZoomRoom Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="zoomRoom"></param>
		private void Subscribe(ZoomRoom zoomRoom)
		{
            zoomRoom.OnUsbCamerasChanged += ZoomRoomOnUsbCamerasChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="zoomRoom"></param>
		private void Unsubscribe(ZoomRoom zoomRoom)
		{
            zoomRoom.OnUsbCamerasChanged -= ZoomRoomOnUsbCamerasChanged;
		}

        private void ZoomRoomOnUsbCamerasChanged(object sender, EventArgs e)
        {
            UpdateUsbCameras();
        }

		#endregion
	}
}
