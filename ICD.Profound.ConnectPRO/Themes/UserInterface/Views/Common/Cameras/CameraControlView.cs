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
		public event EventHandler OnCameraHomeButtonReleased;
		public event EventHandler OnCameraHomeButtonHeld;
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
			OnCameraHomeButtonReleased = null;
			OnCameraHomeButtonHeld = null;
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

		#region Methods

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

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DPad.OnButtonPressed += DPadOnButtonPressed;
			m_DPad.OnButtonReleased += DPadOnButtonReleased;
			m_DPad.OnButtonHeld += DPadOnButtonHeld;
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
			m_DPad.OnButtonHeld -= DPadOnButtonHeld;
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

		private void DPadOnButtonHeld(object sender, DPadEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case DPadEventArgs.eDirection.Center:
					OnCameraHomeButtonHeld.Raise(this);
					break;
			}
		}

		private void DPadOnButtonReleased(object sender, DPadEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case DPadEventArgs.eDirection.Up:
				case DPadEventArgs.eDirection.Down:
				case DPadEventArgs.eDirection.Left:
				case DPadEventArgs.eDirection.Right:
					OnCameraPtzButtonReleased.Raise(this);
					break;

				case DPadEventArgs.eDirection.Center:
					OnCameraHomeButtonReleased.Raise(this);
					break;
			}
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
