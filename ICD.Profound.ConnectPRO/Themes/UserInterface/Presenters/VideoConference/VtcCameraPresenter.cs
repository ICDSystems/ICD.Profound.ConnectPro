using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcCameraPresenter : AbstractPresenter<IVtcCameraView>, IVtcCameraPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private ICameraDevice m_Camera;
		private CameraPreset[] m_CameraPresets;

		/// <summary>
		/// Gets/sets the current camera device.
		/// </summary>
		public ICameraDevice Camera
		{
			get { return m_Camera; }
			set
			{
				if (value == m_Camera)
					return;

				m_Camera = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcCameraPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_CameraPresets = new CameraPreset[0];
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcCameraView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				ICameraWithPresets cameraWithPresets = m_Camera as ICameraWithPresets;

				IEnumerable<CameraPreset> presets =
					cameraWithPresets == null
						? Enumerable.Empty<CameraPreset>()
						: cameraWithPresets.GetPresets();

				m_CameraPresets = presets.ToArray();

				for (ushort index = 0; index < 5; index++)
				{
					string name = index < m_CameraPresets.Length
						              ? m_CameraPresets[index].Name
						              : string.Empty;

					view.SetPresetButtonLabel(index, name);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void Zoom(eCameraZoomAction action)
		{
			ICameraWithZoom zoom = m_Camera as ICameraWithZoom;
			if (zoom != null)
				zoom.Zoom(action);
		}

		private void PanTilt(eCameraPanTiltAction action)
		{
			ICameraWithPanTilt panTilt = m_Camera as ICameraWithPanTilt;
			if (panTilt != null)
				panTilt.PanTilt(action);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcCameraView view)
		{
			base.Subscribe(view);

			view.OnCameraButtonReleased += ViewOnCameraButtonReleased;
			view.OnCameraMoveDownButtonPressed += ViewOnCameraMoveDownButtonPressed;
			view.OnCameraMoveLeftButtonPressed += ViewOnCameraMoveLeftButtonPressed;
			view.OnCameraMoveRightButtonPressed += ViewOnCameraMoveRightButtonPressed;
			view.OnCameraMoveUpButtonPressed += ViewOnCameraMoveUpButtonPressed;
			view.OnCameraZoomInButtonPressed += ViewOnCameraZoomInButtonPressed;
			view.OnCameraZoomOutButtonPressed += ViewOnCameraZoomOutButtonPressed;
			view.OnPresetButtonPressed += ViewOnPresetButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcCameraView view)
		{
			base.Unsubscribe(view);

			view.OnCameraButtonReleased -= ViewOnCameraButtonReleased;
			view.OnCameraMoveDownButtonPressed -= ViewOnCameraMoveDownButtonPressed;
			view.OnCameraMoveLeftButtonPressed -= ViewOnCameraMoveLeftButtonPressed;
			view.OnCameraMoveRightButtonPressed -= ViewOnCameraMoveRightButtonPressed;
			view.OnCameraMoveUpButtonPressed -= ViewOnCameraMoveUpButtonPressed;
			view.OnCameraZoomInButtonPressed -= ViewOnCameraZoomInButtonPressed;
			view.OnCameraZoomOutButtonPressed -= ViewOnCameraZoomOutButtonPressed;
			view.OnPresetButtonPressed -= ViewOnPresetButtonPressed;
		}

		private void ViewOnCameraButtonReleased(object sender, EventArgs eventArgs)
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

		private void ViewOnPresetButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			ICameraWithPresets camera = m_Camera as ICameraWithPresets;
			if (camera == null)
				return;

			ushort index = eventArgs.Data;
			if (index >= m_CameraPresets.Length)
				return;

			CameraPreset preset = m_CameraPresets[index];
			camera.ActivatePreset(preset.PresetId);
		}

		#endregion
	}
}
