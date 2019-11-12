using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Cameras
{
	[ViewBinding(typeof(ICameraLayoutView))]
	public sealed partial class CameraLayoutView : AbstractUiView, ICameraLayoutView
	{
		#region Events

		/// <summary>
		/// Raised when a size layout configuration button is pressed.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnLayoutSizeButtonPressed;

		/// <summary>
		/// Raised when a style layout configuration button is pressed.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnLayoutStyleButtonPressed;

		/// <summary>
		/// Raised when a share layout configuration button is pressed.
		/// </summary>
		public event EventHandler OnContentThumbnailButtonPressed;

		/// <summary>
		/// Raised when a self-view layout configuration button is pressed.
		/// </summary>
		public event EventHandler OnSelfviewCameraButtonPressed;

		/// <summary>
		/// Raised when a position layout configuration button is pressed.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnThumbnailPositionButtonPressed;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public CameraLayoutView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnLayoutSizeButtonPressed = null;
			OnLayoutStyleButtonPressed = null;
			OnContentThumbnailButtonPressed = null;
			OnSelfviewCameraButtonPressed = null;
			OnThumbnailPositionButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the selected state of a button for the size layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetLayoutSizeButtonSelected(ushort index, bool selected)
		{
			m_LayoutSizeButtonList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the style layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetLayoutStyleButtonSelected(ushort index, bool selected)
		{
			m_LayoutStyleButtonList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the share layout control.
		/// </summary>
		/// <param name="selected"></param>
		public void SetContentThumbnailButtonSelected(bool selected)
		{
			// The toggle button style is backwards
			m_ContentThumbnailButton.SetSelected(!selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the self-view layout control.
		/// </summary>
		/// <param name="selected"></param>
		public void SetSelfviewCameraButtonSelected(bool selected)
		{
			// The toggle button style is backwards
			m_SelfviewCameraButton.SetSelected(!selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the position layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetThumbnailPositionButtonSelected(ushort index, bool selected)
		{
			m_ThumbnailPositionButtonList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the enabled state of the size layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetLayoutSizeListEnabled(bool enabled)
		{
			m_LayoutSizeButtonList.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the style layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetLayoutStyleListEnabled(bool enabled)
		{
			m_LayoutStyleButtonList.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the share layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetContentThumbnailButtonEnabled(bool enabled)
		{
			m_ContentThumbnailButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the self-view layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetSelfviewCameraButtonEnabled(bool enabled)
		{
			m_SelfviewCameraButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the position layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetThumbnailPositionListEnabled(bool enabled)
		{
			m_ThumbnailPositionButtonList.Enable(enabled);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();
			
			m_LayoutSizeButtonList.OnButtonClicked += LayoutSizeButtonListOnButtonClicked;
			m_LayoutStyleButtonList.OnButtonClicked += LayoutStyleButtonListOnButtonClicked;
			m_ContentThumbnailButton.OnPressed += ContentThumbnailButtonOnPressed;
			m_SelfviewCameraButton.OnPressed += SelfviewCameraButtonOnPressed;
			m_ThumbnailPositionButtonList.OnButtonClicked += ThumbnailPositionButtonListOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();
			
			m_LayoutSizeButtonList.OnButtonClicked -= LayoutSizeButtonListOnButtonClicked;
			m_LayoutStyleButtonList.OnButtonClicked -= LayoutStyleButtonListOnButtonClicked;
			m_ContentThumbnailButton.OnPressed -= ContentThumbnailButtonOnPressed;
			m_SelfviewCameraButton.OnPressed -= SelfviewCameraButtonOnPressed;
			m_ThumbnailPositionButtonList.OnButtonClicked -= ThumbnailPositionButtonListOnButtonClicked;
		}

		private void LayoutSizeButtonListOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnLayoutSizeButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		private void LayoutStyleButtonListOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnLayoutStyleButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		private void ContentThumbnailButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnContentThumbnailButtonPressed.Raise(this);
		}

		private void SelfviewCameraButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSelfviewCameraButtonPressed.Raise(this);
		}

		private void ThumbnailPositionButtonListOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnThumbnailPositionButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		#endregion
	}
}
