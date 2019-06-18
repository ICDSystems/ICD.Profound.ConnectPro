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
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
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
		private IPresetControl m_SubscribedPresetControl;

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

				SetSelectedCamera(m_Cameras.FirstOrDefault());
			}
			finally
			{
				m_RefreshSection.Leave();
			}
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
				IPresetControl presetControl =
					m_SelectedCamera == null ? null : m_SelectedCamera.Controls.GetControl<IPresetControl>();

				IEnumerable<CameraPreset> presets =
					presetControl == null
						? Enumerable.Empty<CameraPreset>()
						: presetControl.GetPresets();

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
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		/// <summary>
		/// Gets/sets the current camera control.
		/// </summary>
		private void SetSelectedCamera(ICameraDevice camera)
		{
			if (camera == m_SelectedCamera)
				return;

			Unsubscribe(m_SelectedCamera);
			m_SelectedCamera = camera;
			Subscribe(m_SelectedCamera);

			RefreshIfVisible();
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
			if (m_SelectedCamera == null)
				return;

			IZoomControl zoom = m_SelectedCamera.Controls.GetControl<IZoomControl>();
			if (zoom != null)
				zoom.Zoom(action);
		}

		private void PanTilt(eCameraPanTiltAction action)
		{
			if (m_SelectedCamera == null)
				return;

			IPanTiltControl panTilt = m_SelectedCamera.Controls.GetControl<IPanTiltControl>();
			if (panTilt != null)
				panTilt.PanTilt(action);
		}

		#endregion

		#region Camera Callbacks

		/// <summary>
		/// Subscribe to the camera events.
		/// </summary>
		/// <param name="camera"></param>
		private void Subscribe(ICameraDevice camera)
		{
			m_SubscribedPresetControl = camera == null ? null : camera.Controls.GetControl<IPresetControl>();
			if (m_SubscribedPresetControl != null)
				m_SubscribedPresetControl.OnPresetsChanged += SubscribedPresetControlOnPresetsChanged;
		}

		/// <summary>
		/// Unsubscribe from the camera events.
		/// </summary>
		/// <param name="camera"></param>
		private void Unsubscribe(ICameraDevice camera)
		{
			if (m_SubscribedPresetControl != null)
				m_SubscribedPresetControl.OnPresetsChanged -= SubscribedPresetControlOnPresetsChanged;

			m_SubscribedPresetControl = null;
		}

		/// <summary>
		/// Called when the presets change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SubscribedPresetControlOnPresetsChanged(object sender, EventArgs eventArgs)
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
			view.OnCameraZoomInButtonPressed -= ViewOnCameraZoomInButtonPressed;
			view.OnCameraZoomOutButtonPressed -= ViewOnCameraZoomOutButtonPressed;
			view.OnPresetButtonReleased -= ViewOnPresetButtonReleased;
			view.OnPresetButtonHeld -= ViewOnPresetButtonHeld;
			view.OnCameraButtonPressed -= ViewOnCameraButtonPressed;
		}

		private void ViewOnCameraPtzButtonReleased(object sender, EventArgs eventArgs)
		{
			Zoom(eCameraZoomAction.Stop);
			PanTilt(eCameraPanTiltAction.Stop);
		}

		private void ViewOnCameraMoveDownButtonPressed(object sender, EventArgs eventArgs)
		{
			PanTilt(eCameraPanTiltAction.Down);
		}

		private void ViewOnCameraMoveLeftButtonPressed(object sender, EventArgs eventArgs)
		{
			PanTilt(eCameraPanTiltAction.Left);
		}

		private void ViewOnCameraMoveRightButtonPressed(object sender, EventArgs eventArgs)
		{
			PanTilt(eCameraPanTiltAction.Right);
		}

		private void ViewOnCameraMoveUpButtonPressed(object sender, EventArgs eventArgs)
		{
			PanTilt(eCameraPanTiltAction.Up);
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
			if (m_SelectedCamera == null)
				return;

			IPresetControl cameraControl = m_SelectedCamera.Controls.GetControl<IPresetControl>();
			if (cameraControl == null)
				return;

			ushort index = (ushort)(eventArgs.Data + 1);

			CameraPreset preset;
			if (m_CameraPresets.TryGetValue(index, out preset))
				cameraControl.ActivatePreset(preset.PresetId);
		}

		private void ViewOnPresetButtonHeld(object sender, UShortEventArgs eventArgs)
		{
			if (m_SelectedCamera == null)
				return;

			IPresetControl cameraControl = m_SelectedCamera.Controls.GetControl<IPresetControl>();
			if (cameraControl == null)
				return;

			ushort index = (ushort)(eventArgs.Data + 1);
			cameraControl.StorePreset(index);

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

			// TODO remove if not needed after fixing
			//if (args.Data)
			//{
			//	// Hide the conferencing subpages
			//	Navigation.LazyLoadPresenter<IVtcContactsNormalPresenter>().ShowView(false);
			//	Navigation.LazyLoadPresenter<IVtcContactsPolycomPresenter>().ShowView(false);
			//	Navigation.LazyLoadPresenter<IVtcButtonListPresenter>().ShowView(false);
			//}
			//else
			//{
			//	// Show the conferencing subpages
			//	Navigation.LazyLoadPresenter<IVtcButtonListPresenter>().ShowView(true);
			//}
		}

		#endregion
	}
}
