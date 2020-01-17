﻿using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Conferencing.Devices;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Components.Camera;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Cameras
{
	[PresenterBinding(typeof(ICameraActivePresenter))]
	public sealed class CameraActivePresenter : AbstractUiPresenter<ICameraActiveView>, ICameraActivePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private readonly List<ICameraDevice> m_Cameras;
		private readonly List<CameraInfo> m_ZoomCameras;
		
		[CanBeNull]
		private ICameraDevice m_SelectedCamera;

		[CanBeNull]
		private CameraInfo m_SelectedZoomCamera;

		[CanBeNull] private IVideoConferenceRouteControl m_VtcDestinationControl;

		[CanBeNull] private CameraComponent m_SubscribedCameraComponent;

		/// <summary>
		/// Gets the number of cameras.
		/// </summary>
		public int CameraCount { get { return m_RefreshSection.Execute(() => ZoomMode ? m_ZoomCameras.Count : m_Cameras.Count); } }

		public bool ZoomMode { get { return m_VtcDestinationControl != null && m_VtcDestinationControl.Parent is ZoomRoom; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CameraActivePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Cameras = new List<ICameraDevice>();
			m_ZoomCameras = new List<CameraInfo>();
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			CacheCameraList();

            RefreshIfVisible();
		}

		/// <summary>
		/// Sets the VTC routing control to route camera video to.
		/// </summary>
		/// <param name="value"></param>
		public void SetVtcDestinationControl(IVideoConferenceRouteControl value)
		{
			if (value == m_VtcDestinationControl)
				return;

			Unsubscribe(m_SubscribedCameraComponent);

			m_VtcDestinationControl = value;

			m_SubscribedCameraComponent = GetZoomCameraComponent();
			Subscribe(m_SubscribedCameraComponent);

			RouteSelectedCamera();

			CacheCameraList();
            RefreshIfVisible();
		}

		private void CacheCameraList()
		{
			// Zoom cameras
			IEnumerable<CameraInfo> zoomCameras = GetZoomCameras();
			m_ZoomCameras.Clear();
			m_ZoomCameras.AddRange(zoomCameras);

			// Camera devices
			IEnumerable<ICameraDevice> cameras = GetCameras();
			m_Cameras.Clear();
			m_Cameras.AddRange(cameras);

			// Clear the zoom camera selection
			CameraComponent cameraComponent = GetZoomCameraComponent();
			m_SelectedZoomCamera = cameraComponent == null ? null : cameraComponent.ActiveCamera;
		}

		private IEnumerable<CameraInfo> GetZoomCameras()
		{
			CameraComponent cameraComponent = GetZoomCameraComponent();
			return cameraComponent == null
				       ? Enumerable.Empty<CameraInfo>()
				       : cameraComponent.GetCameras();
		}

		private IEnumerable<ICameraDevice> GetCameras()
		{
			if (Room == null || m_VtcDestinationControl == null)
				return Enumerable.Empty<ICameraDevice>();

			EndpointInfo[] inputs =
				m_VtcDestinationControl.GetCodecInputs(eCodecInputType.Camera)
				                       .Select(i => m_VtcDestinationControl.GetInputEndpointInfo(i))
				                       .ToArray();

			// Get cameras that can be routed to the VTC camera inputs.
			return Room.Originators
			           .GetInstancesRecursive<ICameraDevice>()
					   .Where(c =>
					          {
						          IRouteSourceControl sourceControl = c.Controls.GetControl<IRouteSourceControl>();
						          return sourceControl != null && inputs.Any(d => Room.Routing.HasPath(sourceControl, d, eConnectionType.Video));
					          });
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ICameraActiveView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool combine = Room != null && Room.IsCombineRoom();

				// Cameras
				IEnumerable<string> cameraLabels = ZoomMode ? m_ZoomCameras.Select(z => z.Name) : m_Cameras.Select(c => c.GetName(combine));
				view.SetCameraLabels(cameraLabels);

				if (ZoomMode)
					for (ushort index = 0; index < m_ZoomCameras.Count; index++)
						view.SetCameraSelected(index, m_SelectedZoomCamera == m_ZoomCameras[index]);
				else
					for (ushort index = 0; index < m_Cameras.Count; index++)
						view.SetCameraSelected(index, m_SelectedCamera == m_Cameras[index]);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		/// <summary>
		/// Sets the currently selected camera.
		/// </summary>
		private void SetSelectedCamera(ICameraDevice camera)
		{
			if (camera == m_SelectedCamera)
				return;
			
			m_SelectedCamera = camera;
			m_SelectedZoomCamera = null;

			RefreshIfVisible();

			RouteSelectedCamera();
		}
		/// <summary>
		/// Sets the currently selected zoom camera.
		/// </summary>
		private void SetSelectedCamera(CameraInfo camera)
		{
			if (camera == m_SelectedZoomCamera)
				return;
			
			m_SelectedZoomCamera = camera;
			m_SelectedCamera = null;

			RefreshIfVisible();

			RouteSelectedCamera();
		}

		private void RouteSelectedCamera()
		{
			if (Room == null || (m_SelectedCamera == null && m_SelectedZoomCamera == null))
				return;

			if (m_VtcDestinationControl == null)
				Room.Logger.AddEntry(eSeverity.Error, "Unable to route selected camera - No VTC destination assigned");
			else if (ZoomMode && m_SelectedZoomCamera != null)
			{
				CameraComponent cameraComponent = GetZoomCameraComponent();
				cameraComponent.SetActiveCameraByUsbId(m_SelectedZoomCamera.UsbId);
			}				
			else if (m_SelectedCamera != null)
				Room.Routing.RouteCameraToVtc(m_SelectedCamera, m_VtcDestinationControl);
		}

		private CameraComponent GetZoomCameraComponent()
		{
			ZoomRoom zoomRoom = m_VtcDestinationControl == null ? null : m_VtcDestinationControl.Parent as ZoomRoom;
			CameraComponent component = zoomRoom == null ? null : zoomRoom.Components.GetComponent<CameraComponent>();
			return component;
		}
		
		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ICameraActiveView view)
		{
			base.Subscribe(view);
			
			view.OnCameraButtonPressed += ViewOnCameraButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ICameraActiveView view)
		{
			base.Unsubscribe(view);

			view.OnCameraButtonPressed -= ViewOnCameraButtonPressed;
		}

		private void ViewOnCameraButtonPressed(object sender, UShortEventArgs args)
		{
			m_RefreshSection.Enter();

			try
			{
				if (ZoomMode)
				{
					CameraInfo camera;
					m_ZoomCameras.TryElementAt(args.Data, out camera);

					SetSelectedCamera(camera);
				}
				else
				{
					ICameraDevice camera;
					m_Cameras.TryElementAt(args.Data, out camera);

					SetSelectedCamera(camera);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);
			
            if (!args.Data)
                return;

            if (ZoomMode && m_SelectedZoomCamera == null)
            {
                CameraInfo defaultCamera = m_ZoomCameras.FirstOrDefault();
                if (defaultCamera != null)
                    SetSelectedCamera(defaultCamera);
            }
			else if (!ZoomMode && m_SelectedCamera == null)
			{
				ICameraDevice defaultCamera = m_Cameras.FirstOrDefault();
				if (defaultCamera != null)
					SetSelectedCamera(defaultCamera);
			}
		}

		#endregion

		#region Camera Component Callbacks

		private void Subscribe(CameraComponent cameraComponent)
		{
			if (cameraComponent == null)
				return;

			cameraComponent.OnCamerasUpdated += CameraComponentOnCamerasUpdated;
			cameraComponent.OnActiveCameraUpdated += CameraComponentOnActiveCameraUpdated;
		}

		private void Unsubscribe(CameraComponent cameraComponent)
		{
			if (cameraComponent == null)
				return;
			
			cameraComponent.OnCamerasUpdated -= CameraComponentOnCamerasUpdated;
			cameraComponent.OnActiveCameraUpdated -= CameraComponentOnActiveCameraUpdated;
		}

		private void CameraComponentOnCamerasUpdated(object sender, System.EventArgs e)
		{
			CacheCameraList();
			RefreshIfVisible();
		}

		private void CameraComponentOnActiveCameraUpdated(object sender, System.EventArgs e)
		{
			if (m_SubscribedCameraComponent == null)
				return;

			m_SelectedZoomCamera = m_SubscribedCameraComponent.ActiveCamera;
			RefreshIfVisible();
		}

		#endregion
	}
}