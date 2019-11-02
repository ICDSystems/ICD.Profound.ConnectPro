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
		public event EventHandler<UShortEventArgs> OnCameraConfigurationButtonPressed;

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
		/// Sets the enabled state of a camera configuration button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="enabled"></param>
		public void SetCameraConfigurationButtonEnabled(ushort index, bool enabled)
		{
			m_CameraConfigurationButtonList.SetItemEnabled(index, enabled);
		}

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CameraConfigurationButtonList.OnButtonClicked += CameraConfigurationButtonListOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CameraConfigurationButtonList.OnButtonClicked -= CameraConfigurationButtonListOnButtonClicked;
		}

		private void CameraConfigurationButtonListOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnCameraConfigurationButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}
	}
}
