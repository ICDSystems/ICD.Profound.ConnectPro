using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Camera;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Conference.Camera
{
	[ViewBinding(typeof(ICameraControlView))]
	public sealed partial class CameraControlView : AbstractTouchDisplayView, ICameraControlView
	{
		public event EventHandler OnCameraMoveUpButtonPressed;
		public event EventHandler OnCameraMoveLeftButtonPressed;
		public event EventHandler OnCameraMoveRightButtonPressed;
		public event EventHandler OnCameraMoveDownButtonPressed;
		public event EventHandler OnCameraZoomInButtonPressed;
		public event EventHandler OnCameraZoomOutButtonPressed;
		public event EventHandler OnCameraPtzButtonReleased;

		public event EventHandler<UShortEventArgs> OnPresetButtonReleased;
		public event EventHandler<UShortEventArgs> OnPresetButtonHeld;

		/// <summary>
		/// Sets the enabled state of the directional buttons.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetDPadButtonsEnabled(bool enabled)
		{
			m_DPad.Enable(enabled);
		}

		/// <summary>
		/// Sets the visibility of the preset buttons.
		/// </summary>
		/// <param name="visible"></param>
		public void SetPresetButtonsVisible(bool visible)
		{
			m_PresetButtonPage.Show(visible);
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
		public CameraControlView(ISigInputOutput panel, TouchCueTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCameraMoveUpButtonPressed = null;
			OnCameraMoveLeftButtonPressed = null;
			OnCameraMoveRightButtonPressed = null;
			OnCameraMoveDownButtonPressed = null;
			OnCameraZoomInButtonPressed = null;
			OnCameraZoomOutButtonPressed = null;
			OnCameraPtzButtonReleased = null;
			OnPresetButtonReleased = null;
			OnPresetButtonHeld = null;

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
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void PresetButtonOnReleased(object sender, EventArgs eventArgs)
		{
			ushort index = m_PresetButtons.GetKey(sender as VtProButton);
			OnPresetButtonReleased.Raise(this, new UShortEventArgs(index));
		}

		private void PresetButtonOnHeld(object sender, EventArgs eventArgs)
		{
			ushort index = m_PresetButtons.GetKey(sender as VtProButton);
			OnPresetButtonHeld.Raise(this, new UShortEventArgs(index));
		}

		#endregion
	}
}
