using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Cameras
{
	[ViewBinding(typeof(ICameraButtonsView))]
	public sealed partial class CameraButtonsView : AbstractUiView, ICameraButtonsView
	{
		/// <summary>
		/// Raised when one of the camera configuration buttons are pressed (Control, Active, or Layout).
		/// </summary>
		public event EventHandler<UShortEventArgs> OnCameraConfigurationButtonPressed;

		public event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public CameraButtonsView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release Resources.
		/// </summary>
		public override void Dispose()
		{
			OnCameraConfigurationButtonPressed = null;
			OnCloseButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the selected state of a camera configuration button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetCameraConfigurationButtonSelected(ushort index, bool selected)
		{
			m_CameraConfigurationButtonList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the visible state of a camera configuration button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetCameraConfigurationButtonVisible(ushort index, bool visible)
		{
			m_CameraConfigurationButtonList.SetItemVisible(index, visible);
		}

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CameraConfigurationButtonList.OnButtonClicked += CameraConfigurationButtonListOnButtonClicked;
			m_CloseButton.OnPressed += CloseButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CameraConfigurationButtonList.OnButtonClicked -= CameraConfigurationButtonListOnButtonClicked;
			m_CloseButton.OnPressed -= CloseButtonOnPressed;
		}

		private void CameraConfigurationButtonListOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnCameraConfigurationButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
		}
	}
}
