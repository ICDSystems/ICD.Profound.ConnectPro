using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Cameras
{
	[ViewBinding(typeof(ICameraControlView))]
	public sealed partial class CameraControlView : AbstractUiView, ICameraControlView
	{
		public event EventHandler OnCameraHomeButtonPressed;
		public event EventHandler OnCameraMoveUpButtonPressed;
		public event EventHandler OnCameraMoveLeftButtonPressed;
		public event EventHandler OnCameraMoveRightButtonPressed;
		public event EventHandler OnCameraMoveDownButtonPressed;
		public event EventHandler OnCameraZoomInButtonPressed;
		public event EventHandler OnCameraZoomOutButtonPressed;
		public event EventHandler OnCameraPtzButtonReleased;

		public event EventHandler<UShortEventArgs> OnPresetButtonReleased;
		public event EventHandler<UShortEventArgs> OnPresetButtonHeld;
		public event EventHandler<UShortEventArgs> OnCameraButtonPressed;

		/// <summary>
		/// Sets the enabled state of the directional buttons.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetDPadButtonsEnabled(bool enabled)
		{
			m_DPad.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the preset buttons.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetPresetButtonsEnabled(bool enabled)
		{
			// Preset buttons share the same enable join.
			m_PresetButton1.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the zoom buttons.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetZoomButtonsEnabled(bool enabled)
		{
			// Zoom buttons share the same enable join.
			m_ZoomInButton.Enable(enabled);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public CameraControlView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCameraHomeButtonPressed = null;
			OnCameraMoveUpButtonPressed = null;
			OnCameraMoveLeftButtonPressed = null;
			OnCameraMoveRightButtonPressed = null;
			OnCameraMoveDownButtonPressed = null;
			OnCameraZoomInButtonPressed = null;
			OnCameraZoomOutButtonPressed = null;
			OnCameraPtzButtonReleased = null;
			OnPresetButtonReleased = null;
			OnPresetButtonHeld = null;
			OnCameraButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the label for the preset button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		public void SetPresetButtonLabel(ushort index, string label)
		{
			m_PresetButtons.GetValue(index).SetLabelText(label);
		}

		/// <summary>
		/// Sets the visibility of the "Preset Stored" label.
		/// </summary>
		/// <param name="visible"></param>
		public void SetPresetStoredLabelVisibility(bool visible)
		{
			m_PresetStoredLabel.Show(visible);
		}

		/// <summary>
		/// Sets the camera selection list labels.
		/// </summary>
		/// <param name="labels"></param>
		public void SetCameraLabels(IEnumerable<string> labels)
		{
			if (labels == null)
				throw new ArgumentNullException("labels");

			string[] labelsArray = labels.ToArray();

			m_CameraList.SetItemLabels(labelsArray);
		}

		/// <summary>
		/// Sets the selection state of the camera button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetCameraSelected(ushort index, bool selected)
		{
			m_CameraList.SetItemSelected(index, selected);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DPad.OnButtonPressed += DPadOnButtonPressed;
			m_DPad.OnButtonReleased += DPadOnButtonReleased;
			m_ZoomInButton.OnPressed += ZoomInButtonOnPressed;
			m_ZoomInButton.OnReleased += ZoomInButtonOnReleased;
			m_ZoomOutButton.OnPressed += ZoomOutButtonOnPressed;
			m_ZoomOutButton.OnReleased += ZoomOutButtonOnReleased;
			m_CameraList.OnButtonClicked += CameraListOnButtonClicked;

			foreach (VtProButton button in m_PresetButtons.Values)
			{
				button.OnHeld += PresetButtonOnHeld;
				button.OnReleased += PresetButtonOnReleased;
			}
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DPad.OnButtonPressed -= DPadOnButtonPressed;
			m_DPad.OnButtonReleased -= DPadOnButtonReleased;
			m_ZoomInButton.OnPressed -= ZoomInButtonOnPressed;
			m_ZoomInButton.OnReleased -= ZoomInButtonOnReleased;
			m_ZoomOutButton.OnPressed -= ZoomOutButtonOnPressed;
			m_ZoomOutButton.OnReleased -= ZoomOutButtonOnReleased;
			m_CameraList.OnButtonClicked -= CameraListOnButtonClicked;

			foreach (VtProButton button in m_PresetButtons.Values)
			{
				button.OnHeld -= PresetButtonOnHeld;
				button.OnReleased -= PresetButtonOnReleased;
			}
		}

		private void ZoomOutButtonOnReleased(object sender, EventArgs eventArgs)
		{
			OnCameraPtzButtonReleased.Raise(this);
		}

		private void ZoomOutButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCameraZoomOutButtonPressed.Raise(this);
		}

		private void ZoomInButtonOnReleased(object sender, EventArgs eventArgs)
		{
			OnCameraPtzButtonReleased.Raise(this);
		}

		private void ZoomInButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCameraZoomInButtonPressed.Raise(this);
		}

		private void DPadOnButtonReleased(object sender, DPadEventArgs eventArgs)
		{
			OnCameraPtzButtonReleased.Raise(this);
		}

		private void DPadOnButtonPressed(object sender, DPadEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case DPadEventArgs.eDirection.Up:
					OnCameraMoveUpButtonPressed.Raise(this);
					break;
				case DPadEventArgs.eDirection.Down:
					OnCameraMoveDownButtonPressed.Raise(this);
					break;
				case DPadEventArgs.eDirection.Left:
					OnCameraMoveLeftButtonPressed.Raise(this);
					break;
				case DPadEventArgs.eDirection.Right:
					OnCameraMoveRightButtonPressed.Raise(this);
					break;
				case DPadEventArgs.eDirection.Center:
					OnCameraHomeButtonPressed.Raise(this);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void PresetButtonOnReleased(object sender, EventArgs eventArgs)
		{
			VtProButton button = sender as VtProButton;
			if (button == null)
				return;

			ushort index = m_PresetButtons.GetKey(button);
			OnPresetButtonReleased.Raise(this, new UShortEventArgs(index));
		}

		private void PresetButtonOnHeld(object sender, EventArgs eventArgs)
		{
			VtProButton button = sender as VtProButton;
			if (button == null)
				return;

			ushort index = m_PresetButtons.GetKey(button);
			OnPresetButtonHeld.Raise(this, new UShortEventArgs(index));
		}

		/// <summary>
		/// Called when the user presses a camera list button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CameraListOnButtonClicked(object sender, UShortEventArgs args)
		{
			OnCameraButtonPressed.Raise(this, new UShortEventArgs(args.Data));
		}

		#endregion
	}
}
