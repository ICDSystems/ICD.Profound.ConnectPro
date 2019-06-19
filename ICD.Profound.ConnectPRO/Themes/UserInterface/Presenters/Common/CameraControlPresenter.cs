﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Cameras;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.UI.Attributes;
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

		private readonly Dictionary<int, CameraPreset> m_CameraPresets;

		private readonly SafeTimer m_PresetStoredTimer;

		[CanBeNull]
		private ICameraDevice m_Camera;

		[CanBeNull]
		private IPresetControl m_SubscribedPresetControl;

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
				IPresetControl presetControl = m_Camera == null ? null : m_Camera.Controls.GetControl<IPresetControl>();

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

		private void ShowPresetStoredLabel(bool visible)
		{
			ICameraControlView view = GetView();
			if (view == null)
				return;

			view.SetPresetStoredLabelVisibility(visible);
		}

		private void Zoom(eCameraZoomAction action)
		{
			if (m_Camera == null)
				return;

			IZoomControl zoom = m_Camera.Controls.GetControl<IZoomControl>();
			if (zoom != null)
				zoom.Zoom(action);
		}

		private void PanTilt(eCameraPanTiltAction action)
		{
			if (m_Camera == null)
				return;

			IPanTiltControl panTilt = m_Camera.Controls.GetControl<IPanTiltControl>();
			if (panTilt != null)
				panTilt.PanTilt(action);
		}

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
			if (m_Camera == null)
				return;

			IPresetControl cameraControl = m_Camera.Controls.GetControl<IPresetControl>();
			if (cameraControl == null)
				return;

			ushort index = (ushort)(eventArgs.Data + 1);

			CameraPreset preset;
			if (m_CameraPresets.TryGetValue(index, out preset))
				cameraControl.ActivatePreset(preset.PresetId);
		}

		private void ViewOnPresetButtonHeld(object sender, UShortEventArgs eventArgs)
		{
			if (m_Camera == null)
				return;

			IPresetControl cameraControl = m_Camera.Controls.GetControl<IPresetControl>();
			if (cameraControl == null)
				return;

			ushort index = (ushort)(eventArgs.Data + 1);
			cameraControl.StorePreset(index);

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