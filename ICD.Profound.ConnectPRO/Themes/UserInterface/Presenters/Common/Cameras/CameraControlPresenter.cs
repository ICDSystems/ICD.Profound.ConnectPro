using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Cameras;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Cameras
{
	[PresenterBinding(typeof(ICameraControlPresenter))]
	public sealed class CameraControlPresenter : AbstractUiPresenter<ICameraControlView>, ICameraControlPresenter
	{
		private const long PRESET_STORED_VISIBILITY_MILLISECONDS = 1000;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly List<ICameraDevice> m_Cameras;
		private readonly Dictionary<int, CameraPreset> m_CameraPresets;
		private readonly SafeTimer m_PresetStoredTimer;

		[CanBeNull]
		private ICameraDevice m_SelectedCamera;

        [CanBeNull]
        private IVideoConferenceRouteControl m_VtcDestinationControl;

		/// <summary>
		/// Gets the number of cameras.
		/// </summary>
		public int CameraCount { get { return m_RefreshSection.Execute(() => m_Cameras.Count); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CameraControlPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Cameras = new List<ICameraDevice>();
			m_CameraPresets = new Dictionary<int, CameraPreset>();
			m_PresetStoredTimer = SafeTimer.Stopped(() => ShowPresetStoredLabel(false));
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_PresetStoredTimer.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<ICameraDevice> cameras =
					room == null
						? Enumerable.Empty<ICameraDevice>()
						: room.Originators.GetInstancesRecursive<ICameraDevice>();

				m_Cameras.Clear();
				m_Cameras.AddRange(cameras);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

        /// <summary>
        /// Sets the VTC routing control to route camera video to.
        /// </summary>
        /// <param name="value"></param>
        public void SetVtcDestinationControl(IVideoConferenceRouteControl value)
        {
            if (value == m_VtcDestinationControl)
                return;

            m_VtcDestinationControl = value;

	        if (Room != null)
		        Room.SetActiveCamera(m_SelectedCamera, m_VtcDestinationControl);
        }

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ICameraControlView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool combine = Room != null && Room.IsCombineRoom();

				// Cameras
				IEnumerable<string> cameraLabels = m_Cameras.Select(c => c.GetName(combine));
				view.SetCameraLabels(cameraLabels);

				for (ushort index = 0; index < m_Cameras.Count; index++)
					view.SetCameraSelected(index, m_SelectedCamera == m_Cameras[index]);

				// Presets
				IEnumerable<CameraPreset> presets =
					m_SelectedCamera == null
						? Enumerable.Empty<CameraPreset>()
						: m_SelectedCamera.GetPresets();

				m_CameraPresets.Clear();
				foreach (CameraPreset preset in presets)
					m_CameraPresets[preset.PresetId] = preset;

				for (ushort index = 0; index < 5; index++)
				{
					string name = "Press and Hold";

					CameraPreset preset;
					if (m_CameraPresets.TryGetValue(index + 1, out preset))
					{
						name = preset.Name ?? string.Empty;
						name = name.Trim();

						if (string.IsNullOrEmpty(name))
							name = string.Format("Preset {0}", index + 1);
					}

					view.SetPresetButtonLabel(index, name);
				}

				// Enabled states
				bool hasPanTilt = m_SelectedCamera != null && m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.PanTilt);
				bool hasZoom = m_SelectedCamera != null && m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Zoom);
				bool hasPresets = m_SelectedCamera != null && m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Presets);

				view.SetDPadButtonsEnabled(hasPanTilt);
				view.SetPresetButtonsEnabled(hasPresets);
				view.SetZoomButtonsEnabled(hasZoom);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets/sets the current camera control.
		/// </summary>
		private void SetSelectedCamera([CanBeNull] ICameraDevice camera)
		{
			if (camera == m_SelectedCamera)
				return;

			Unsubscribe(m_SelectedCamera);
			m_SelectedCamera = camera;
			Subscribe(m_SelectedCamera);

			RefreshIfVisible();

			if (Room != null)
				Room.SetActiveCamera(m_SelectedCamera, m_VtcDestinationControl);
        }

        private void ShowPresetStoredLabel(bool visible)
		{
			ICameraControlView view = GetView();
			if (view == null)
				return;

			view.SetPresetStoredLabelVisibility(visible);
		}

		private void Zoom(eCameraZoomAction action)
		{
			if (m_SelectedCamera != null && m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Zoom))
				m_SelectedCamera.Zoom(action);
		}

		private void Pan(eCameraPanAction action)
		{
			if (m_SelectedCamera != null && m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Pan))
				m_SelectedCamera.Pan(action);
		}

		private void Tilt(eCameraTiltAction action)
		{
			if (m_SelectedCamera != null && m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Tilt))
				m_SelectedCamera.Tilt(action);
		}

		private void Home()
		{
			if (m_SelectedCamera != null && m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Home))
				m_SelectedCamera.ActivateHome();
		}

		private void StoreHome()
		{
			if (m_SelectedCamera == null || !m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Home))
				return;

			m_SelectedCamera.StoreHome();

			ShowPresetStoredLabel(true);
			m_PresetStoredTimer.Reset(PRESET_STORED_VISIBILITY_MILLISECONDS);
		}

		#endregion

		#region Camera Callbacks

		/// <summary>
		/// Subscribe to the camera events.
		/// </summary>
		/// <param name="camera"></param>
		private void Subscribe(ICameraDevice camera)
		{
			if (camera == null)
				return;

			camera.OnPresetsChanged += CameraOnPresetsChanged;
		}

		/// <summary>
		/// Unsubscribe from the camera events.
		/// </summary>
		/// <param name="camera"></param>
		private void Unsubscribe(ICameraDevice camera)
		{
			if (camera == null)
				return;

			camera.OnPresetsChanged -= CameraOnPresetsChanged;
		}

		/// <summary>
		/// Called when the presets change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void CameraOnPresetsChanged(object sender, GenericEventArgs<IEnumerable<CameraPreset>> eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ICameraControlView view)
		{
			base.Subscribe(view);

			view.OnCameraPtzButtonReleased += ViewOnCameraPtzButtonReleased;
			view.OnCameraMoveDownButtonPressed += ViewOnCameraMoveDownButtonPressed;
			view.OnCameraMoveLeftButtonPressed += ViewOnCameraMoveLeftButtonPressed;
			view.OnCameraMoveRightButtonPressed += ViewOnCameraMoveRightButtonPressed;
			view.OnCameraMoveUpButtonPressed += ViewOnCameraMoveUpButtonPressed;
			view.OnCameraHomeButtonReleased += ViewOnCameraHomeButtonReleased;
			view.OnCameraHomeButtonHeld += ViewOnCameraHomeButtonHeld;
			view.OnCameraZoomInButtonPressed += ViewOnCameraZoomInButtonPressed;
			view.OnCameraZoomOutButtonPressed += ViewOnCameraZoomOutButtonPressed;
			view.OnPresetButtonReleased += ViewOnPresetButtonReleased;
			view.OnPresetButtonHeld += ViewOnPresetButtonHeld;
			view.OnCameraButtonPressed += ViewOnCameraButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ICameraControlView view)
		{
			base.Unsubscribe(view);

			view.OnCameraPtzButtonReleased -= ViewOnCameraPtzButtonReleased;
			view.OnCameraMoveDownButtonPressed -= ViewOnCameraMoveDownButtonPressed;
			view.OnCameraMoveLeftButtonPressed -= ViewOnCameraMoveLeftButtonPressed;
			view.OnCameraMoveRightButtonPressed -= ViewOnCameraMoveRightButtonPressed;
			view.OnCameraMoveUpButtonPressed -= ViewOnCameraMoveUpButtonPressed;
			view.OnCameraHomeButtonReleased -= ViewOnCameraHomeButtonReleased;
			view.OnCameraHomeButtonHeld -= ViewOnCameraHomeButtonHeld;
			view.OnCameraZoomInButtonPressed -= ViewOnCameraZoomInButtonPressed;
			view.OnCameraZoomOutButtonPressed -= ViewOnCameraZoomOutButtonPressed;
			view.OnPresetButtonReleased -= ViewOnPresetButtonReleased;
			view.OnPresetButtonHeld -= ViewOnPresetButtonHeld;
			view.OnCameraButtonPressed -= ViewOnCameraButtonPressed;
		}

		private void ViewOnCameraPtzButtonReleased(object sender, EventArgs eventArgs)
		{
			Zoom(eCameraZoomAction.Stop);
			Pan(eCameraPanAction.Stop);
			Tilt(eCameraTiltAction.Stop);
		}

		private void ViewOnCameraMoveDownButtonPressed(object sender, EventArgs eventArgs)
		{
			Tilt(eCameraTiltAction.Down);
		}

		private void ViewOnCameraMoveLeftButtonPressed(object sender, EventArgs eventArgs)
		{
			// Invert left/right
			Pan(eCameraPanAction.Right);
		}

		private void ViewOnCameraMoveRightButtonPressed(object sender, EventArgs eventArgs)
		{
			// Invert left/right
			Pan(eCameraPanAction.Left);
		}

		private void ViewOnCameraMoveUpButtonPressed(object sender, EventArgs eventArgs)
		{
			Tilt(eCameraTiltAction.Up);
		}

		private void ViewOnCameraHomeButtonReleased(object sender, EventArgs eventArgs)
		{
			Home();
		}

		private void ViewOnCameraHomeButtonHeld(object sender, EventArgs eventArgs)
		{
			StoreHome();
		}

		private void ViewOnCameraZoomInButtonPressed(object sender, EventArgs eventArgs)
		{
			Zoom(eCameraZoomAction.ZoomIn);
		}

		private void ViewOnCameraZoomOutButtonPressed(object sender, EventArgs eventArgs)
		{
			Zoom(eCameraZoomAction.ZoomOut);
		}

		private void ViewOnPresetButtonReleased(object sender, UShortEventArgs eventArgs)
		{
			if (m_SelectedCamera == null || !m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Presets))
				return;

			ushort index = (ushort)(eventArgs.Data + 1);

			CameraPreset preset;
			if (m_CameraPresets.TryGetValue(index, out preset))
				m_SelectedCamera.ActivatePreset(preset.PresetId);
		}

		private void ViewOnPresetButtonHeld(object sender, UShortEventArgs eventArgs)
		{
			if (m_SelectedCamera == null || !m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Presets))
				return;

			ushort index = (ushort)(eventArgs.Data + 1);
			m_SelectedCamera.StorePreset(index);

			ShowPresetStoredLabel(true);
			m_PresetStoredTimer.Reset(PRESET_STORED_VISIBILITY_MILLISECONDS);
		}

		private void ViewOnCameraButtonPressed(object sender, UShortEventArgs args)
		{
			m_RefreshSection.Enter();

			try
			{
				ICameraDevice camera;
				m_Cameras.TryElementAt(args.Data, out camera);

				SetSelectedCamera(camera);
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

			ShowPresetStoredLabel(false);

			if (args.Data && m_SelectedCamera == null)
			{
				ICameraDevice defaultCamera = m_Cameras.FirstOrDefault();
				SetSelectedCamera(defaultCamera);
			}
		}

		#endregion
	}
}
