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
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.Camera;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.Camera;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Conference.Camera
{
	[PresenterBinding(typeof(ICameraControlPresenter))]
	public sealed class CameraControlPresenter : AbstractTouchDisplayPresenter<ICameraControlView>, ICameraControlPresenter
	{
		private const long PRESET_STORED_VISIBILITY_MILLISECONDS = 1000;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly Dictionary<int, CameraPreset> m_CameraPresets;
		private readonly SafeTimer m_PresetStoredTimer;
		
		private readonly HeaderButtonModel m_HeaderButton;

		[CanBeNull]
		private ICameraDeviceControl m_SelectedCamera;

		[CanBeNull]
		private IConferenceManager m_SubscribedConferenceManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CameraControlPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_CameraPresets = new Dictionary<int, CameraPreset>();
			m_PresetStoredTimer = SafeTimer.Stopped(() => ShowPresetStoredLabel(false));

			m_HeaderButton = new HeaderButtonModel(0, 3, ToggleCameraControlVisibility)
			{
				LabelText = "Camera Controls",
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.ConferenceCamera, eTouchCueColor.White),
				Mode = eHeaderButtonMode.Blue
			};
		}

		private void ToggleCameraControlVisibility()
		{
			ShowView(!IsViewVisible);
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
				IEnumerable<ICameraDeviceControl> cameras =
					room == null
						? Enumerable.Empty<ICameraDeviceControl>()
						: room.GetControlsRecursive<ICameraDeviceControl>();

				SetSelectedCamera(cameras.FirstOrDefault());
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
				IEnumerable<CameraPreset> presets =
					m_SelectedCamera == null
						? Enumerable.Empty<CameraPreset>()
						: m_SelectedCamera.GetPresets();

				m_CameraPresets.Clear();
				foreach (CameraPreset preset in presets)
					m_CameraPresets[preset.PresetId] = preset;

				for (ushort index = 0; index < 6; index++)
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
				eCameraFeatures features = m_SelectedCamera == null ? eCameraFeatures.None : m_SelectedCamera.SupportedCameraFeatures;
				bool hasPanTilt = features.HasFlag(eCameraFeatures.PanTilt);
				bool hasZoom = features.HasFlag(eCameraFeatures.Zoom);
				bool hasPresets = features.HasFlag(eCameraFeatures.Presets);

				//view.SetDPadButtonsEnabled(hasPanTilt);
				view.SetPresetButtonsVisible(hasPresets);
				//view.SetZoomButtonsEnabled(hasZoom);
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
		private void SetSelectedCamera(ICameraDeviceControl camera)
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

			//view.SetPresetStoredLabelVisibility(visible);
		}

		private void Zoom(eCameraZoomAction action)
		{
			if (m_SelectedCamera == null || !m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Zoom))
				return;

			switch(action)
			{
				case eCameraZoomAction.ZoomIn:
					m_SelectedCamera.ZoomIn();
					break;
				case eCameraZoomAction.ZoomOut:
					m_SelectedCamera.ZoomOut();
					break;
				case eCameraZoomAction.Stop:
					m_SelectedCamera.ZoomStop();
					break;
			}
		}

		private void Pan(eCameraPanAction action)
		{
			if (m_SelectedCamera == null || !m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Pan))
				return;

			switch(action)
			{
				case eCameraPanAction.Left:
					m_SelectedCamera.PanLeft();
					break;
				case eCameraPanAction.Right:
					m_SelectedCamera.PanRight();
					break;
				case eCameraPanAction.Stop:
					m_SelectedCamera.PanStop();
					break;
			}
		}

		private void Tilt(eCameraTiltAction action)
		{
			if (m_SelectedCamera == null || !m_SelectedCamera.SupportedCameraFeatures.HasFlag(eCameraFeatures.Tilt))
				return;

			switch(action)
			{
				case eCameraTiltAction.Up:
					m_SelectedCamera.TiltUp();
					break;
				case eCameraTiltAction.Down:
					m_SelectedCamera.TiltDown();
					break;
				case eCameraTiltAction.Stop:
					m_SelectedCamera.TiltStop();
					break;
			}
		}

		#endregion

		#region Camera Callbacks

		/// <summary>
		/// Subscribe to the camera events.
		/// </summary>
		/// <param name="camera"></param>
		private void Subscribe(ICameraDeviceControl camera)
		{
			if (camera != null)
				camera.OnPresetsChanged += SubscribedPresetControlOnPresetsChanged;
		}

		/// <summary>
		/// Unsubscribe from the camera events.
		/// </summary>
		/// <param name="camera"></param>
		private void Unsubscribe(ICameraDeviceControl camera)
		{
			if (camera != null)
				camera.OnPresetsChanged -= SubscribedPresetControlOnPresetsChanged;
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

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.Routing.State.OnDisplaySourceChanged += StateOnDisplaySourceChanged;

			m_SubscribedConferenceManager = room.ConferenceManager;
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.Dialers.OnInCallChanged += SubscribedConferenceManagerOnInCallChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.Routing.State.OnDisplaySourceChanged -= StateOnDisplaySourceChanged;

			if (m_SubscribedConferenceManager != null)
				m_SubscribedConferenceManager.Dialers.OnInCallChanged -= SubscribedConferenceManagerOnInCallChanged;
			m_SubscribedConferenceManager = null;
		}

		private void SubscribedConferenceManagerOnInCallChanged(object sender, InCallEventArgs callEventArgs)
		{
			UpdateHeaderButtonVisibility();
		}

		/// <summary>
		/// Called when a source becomes routed/unrouted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StateOnDisplaySourceChanged(object sender, EventArgs eventArgs)
		{
			UpdateHeaderButtonVisibility();
		}

		/// <summary>
		/// Updates the visibility of this subpage.
		/// </summary>
		private void UpdateHeaderButtonVisibility()
		{
			bool show = Room != null &&
			            Room.Dialing.ConferenceActionsAvailable(eInCall.Video) && m_SelectedCamera != null;
			IHeaderPresenter header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			if (show)
				header.AddLeftButton(m_HeaderButton);
			else
				header.RemoveLeftButton(m_HeaderButton);
			header.Refresh();
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

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			ShowPresetStoredLabel(false);
			m_HeaderButton.Selected = args.Data;

			//if (args.Data && m_SelectedCamera == null)
			//{
			//	ICameraDevice defaultCamera = m_Cameras.FirstOrDefault();
			//	if (defaultCamera != null)
			//		SetSelectedCamera(defaultCamera);
			//}
		}

		#endregion
	}
}
