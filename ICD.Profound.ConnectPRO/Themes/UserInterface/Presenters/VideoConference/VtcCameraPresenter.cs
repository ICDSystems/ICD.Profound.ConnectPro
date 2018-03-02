using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Cameras;
using ICD.Connect.Cameras.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcCameraPresenter : AbstractPresenter<IVtcCameraView>, IVtcCameraPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private ICameraDeviceControl m_CameraControl;
		private CameraPreset[] m_CameraPresets;

		/// <summary>
		/// Gets/sets the current camera control.
		/// </summary>
		public ICameraDeviceControl CameraControl
		{
			get { return m_CameraControl; }
			set
			{
				if (value == m_CameraControl)
					return;

				m_CameraControl = value;

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
				IPresetControl presetControl = m_CameraControl as IPresetControl;

				IEnumerable<CameraPreset> presets =
					presetControl == null
						? Enumerable.Empty<CameraPreset>()
						: presetControl.GetPresets();

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
			IZoomControl zoom = m_CameraControl.Parent.Controls.GetControl<IZoomControl>();
			if (zoom != null)
				zoom.Zoom(action);
		}

		private void PanTilt(eCameraPanTiltAction action)
		{
			IPanTiltControl panTilt = m_CameraControl.Parent.Controls.GetControl<IPanTiltControl>();
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
			IPresetControl cameraControl = m_CameraControl as IPresetControl;
			if (cameraControl == null)
				return;

			ushort index = eventArgs.Data;
			if (index >= m_CameraPresets.Length)
				return;

			CameraPreset preset = m_CameraPresets[index];
			cameraControl.ActivatePreset(preset.PresetId);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (args.Data)
			{
				// Hide the conferencing subpages
				Navigation.LazyLoadPresenter<IVtcContactsPresenter>().ShowView(false);
				Navigation.LazyLoadPresenter<IVtcButtonListPresenter>().ShowView(false);
			}
			else
			{
				// Show the conferencing subpages
				Navigation.LazyLoadPresenter<IVtcButtonListPresenter>().ShowView(true);
			}
		}

		#endregion
	}
}
