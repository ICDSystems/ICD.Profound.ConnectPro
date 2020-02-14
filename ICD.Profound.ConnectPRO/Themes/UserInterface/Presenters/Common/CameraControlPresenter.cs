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
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class CameraControlPresenter : AbstractUiPresenter<ICameraControlView>, ICameraControlPresenter
	{
		private const long PRESET_STORED_VISIBILITY_MILLISECONDS = 1000;

		private readonly SafeCriticalSection m_RefreshSection;

		private readonly Dictionary<int, CameraPreset> m_CameraPresets;

		private readonly SafeTimer m_PresetStoredTimer;

		[CanBeNull]
		private ICameraDevice m_Camera;

		/// <summary>
		/// Gets/sets the current camera control.
		/// </summary>
		public ICameraDevice Camera
		{
			get { return m_Camera; }
			set
			{
				if (value == m_Camera)
					return;

				Unsubscribe(m_Camera);
				m_Camera = value;
				Subscribe(m_Camera);

				RefreshIfVisible();
			}
		}

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
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ICameraControlView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<CameraPreset> presets =
					m_Camera == null
						? Enumerable.Empty<CameraPreset>()
						: m_Camera.GetPresets();

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

		private void ShowPresetStoredLabel(bool visible)
		{
			ICameraControlView view = GetView();
			if (view == null)
				return;

			view.SetPresetStoredLabelVisibility(visible);
		}

		private void Zoom(eCameraZoomAction action)
		{
			if (m_Camera != null && m_Camera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Zoom))
				m_Camera.Zoom(action);
		}

		private void Pan(eCameraPanAction action)
		{
			if (m_Camera != null && m_Camera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Pan))
				m_Camera.Pan(action);
		}

		private void Tilt(eCameraTiltAction action)
		{
			if (m_Camera != null && m_Camera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Tilt))
				m_Camera.Tilt(action);
		}

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

			view.OnCameraButtonReleased += ViewOnCameraButtonReleased;
			view.OnCameraMoveDownButtonPressed += ViewOnCameraMoveDownButtonPressed;
			view.OnCameraMoveLeftButtonPressed += ViewOnCameraMoveLeftButtonPressed;
			view.OnCameraMoveRightButtonPressed += ViewOnCameraMoveRightButtonPressed;
			view.OnCameraMoveUpButtonPressed += ViewOnCameraMoveUpButtonPressed;
			view.OnCameraZoomInButtonPressed += ViewOnCameraZoomInButtonPressed;
			view.OnCameraZoomOutButtonPressed += ViewOnCameraZoomOutButtonPressed;
			view.OnPresetButtonReleased += ViewOnPresetButtonReleased;
			view.OnPresetButtonHeld += ViewOnPresetButtonHeld;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ICameraControlView view)
		{
			base.Unsubscribe(view);

			view.OnCameraButtonReleased -= ViewOnCameraButtonReleased;
			view.OnCameraMoveDownButtonPressed -= ViewOnCameraMoveDownButtonPressed;
			view.OnCameraMoveLeftButtonPressed -= ViewOnCameraMoveLeftButtonPressed;
			view.OnCameraMoveRightButtonPressed -= ViewOnCameraMoveRightButtonPressed;
			view.OnCameraMoveUpButtonPressed -= ViewOnCameraMoveUpButtonPressed;
			view.OnCameraZoomInButtonPressed -= ViewOnCameraZoomInButtonPressed;
			view.OnCameraZoomOutButtonPressed -= ViewOnCameraZoomOutButtonPressed;
			view.OnPresetButtonReleased -= ViewOnPresetButtonReleased;
			view.OnPresetButtonHeld -= ViewOnPresetButtonHeld;
		}

		private void ViewOnCameraButtonReleased(object sender, EventArgs eventArgs)
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
			Pan(eCameraPanAction.Left);
		}

		private void ViewOnCameraMoveRightButtonPressed(object sender, EventArgs eventArgs)
		{
			Pan(eCameraPanAction.Right);
		}

		private void ViewOnCameraMoveUpButtonPressed(object sender, EventArgs eventArgs)
		{
			Tilt(eCameraTiltAction.Up);
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
			if (m_Camera == null || !m_Camera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Presets))
				return;

			ushort index = (ushort)(eventArgs.Data + 1);

			CameraPreset preset;
			if (m_CameraPresets.TryGetValue(index, out preset))
				m_Camera.ActivatePreset(preset.PresetId);
		}

		private void ViewOnPresetButtonHeld(object sender, UShortEventArgs eventArgs)
		{
			if (m_Camera == null || !m_Camera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Presets))
				return;

			ushort index = (ushort)(eventArgs.Data + 1);
			m_Camera.StorePreset(index);

			ShowPresetStoredLabel(true);
			m_PresetStoredTimer.Reset(PRESET_STORED_VISIBILITY_MILLISECONDS);
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
