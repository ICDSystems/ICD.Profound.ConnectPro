using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.Camera;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference.Camera
{
	[ViewBinding(typeof(ICameraLayoutView))]
	public sealed partial class CameraLayoutView : AbstractTouchDisplayView, ICameraLayoutView
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
			m_LayoutSizes[index].SetSelected(selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the style layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetLayoutStyleButtonSelected(ushort index, bool selected)
		{
			m_LayoutStyles[index].SetSelected(selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the share layout control.
		/// </summary>
		/// <param name="selected"></param>
		public void SetContentThumbnailButtonSelected(bool selected)
		{
			m_ContentThumbnailButton.SetSelected(selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the self-view layout control.
		/// </summary>
		/// <param name="selected"></param>
		public void SetSelfviewCameraButtonSelected(bool selected)
		{
			m_SelfviewCameraButton.SetSelected(selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the position layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetThumbnailPositionButtonSelected(ushort index, bool selected)
		{
			m_LayoutPositions[index].SetSelected(selected);
		}

		/// <summary>
		/// Sets the visibility of a button for the position layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetThumbnailPositionButtonVisibility(ushort index, bool visible)
		{
			m_LayoutPositions[index].Show(visible);
		}

		public void SetLayoutSizeListEnabled(bool enabled)
		{
			foreach (var button in m_LayoutSizes.Values)
				button.Enable(enabled);
		}

		public void SetLayoutStyleListEnabled(bool enabled)
		{
			foreach (var button in m_LayoutStyles.Values)
				button.Enable(enabled);
		}

		public void SetLayoutStyleGalleryItemEnabled(bool enabled)
		{
			VtProButton button;
			m_LayoutStyles.TryGetValue(0, out button);
			if (button != null)
				button.Enable(enabled);
		}

		public void SetLayoutStyleSpeakerItemEnabled(bool enabled)
		{
			VtProButton button;
			m_LayoutStyles.TryGetValue(1, out button);
			if (button != null)
				button.Enable(enabled);
		}

		public void SetLayoutStyleStripItemEnabled(bool enabled)
		{
			VtProButton button;
			m_LayoutStyles.TryGetValue(2, out button);
			if (button != null)
				button.Enable(enabled);
		}

		public void SetLayoutStyleShareAllItemEnabled(bool enabled)
		{
			VtProButton button;
			m_LayoutStyles.TryGetValue(3, out button);
			if (button != null)
				button.Enable(enabled);
		}

		public void SetContentThumbnailButtonEnabled(bool enabled)
		{
			m_ContentThumbnailButton.Enable(enabled);
		}

		public void SetSelfviewCameraButtonEnabled(bool enabled)
		{
			m_SelfviewCameraButton.Enable(enabled);
		}

		public void SetThumbnailPositionListEnabled(bool enabled)
		{
			foreach (var button in m_LayoutPositions.Values)
				button.Enable(enabled);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ContentThumbnailButton.OnPressed += ContentThumbnailButtonOnPressed;
			m_SelfviewCameraButton.OnPressed += SelfviewCameraButtonOnPressed;

			foreach (var button in m_LayoutSizes)
			{
				button.Value.OnPressed += LayoutSizeButtonListOnButtonClicked;
			}

			foreach (var button in m_LayoutStyles)
			{
				button.Value.OnPressed += LayoutStyleButtonListOnButtonClicked;
			}

			foreach (var button in m_LayoutPositions)
			{
				button.Value.OnPressed += ThumbnailPositionButtonListOnButtonClicked;
			}
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();
			
			m_ContentThumbnailButton.OnPressed -= ContentThumbnailButtonOnPressed;
			m_SelfviewCameraButton.OnPressed -= SelfviewCameraButtonOnPressed;

			foreach (var button in m_LayoutSizes)
			{
				button.Value.OnPressed -= LayoutSizeButtonListOnButtonClicked;
			}

			foreach (var button in m_LayoutStyles)
			{
				button.Value.OnPressed -= LayoutStyleButtonListOnButtonClicked;
			}

			foreach (var button in m_LayoutPositions)
			{
				button.Value.OnPressed -= ThumbnailPositionButtonListOnButtonClicked;
			}
		}

		private void LayoutSizeButtonListOnButtonClicked(object sender, EventArgs e)
		{
			OnLayoutSizeButtonPressed.Raise(this, new UShortEventArgs(m_LayoutSizes.GetKey(sender as VtProButton)));
		}

		private void LayoutStyleButtonListOnButtonClicked(object sender, EventArgs e)
		{
			OnLayoutStyleButtonPressed.Raise(this, new UShortEventArgs(m_LayoutStyles.GetKey(sender as VtProButton)));
		}

		private void ContentThumbnailButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnContentThumbnailButtonPressed.Raise(this);
		}

		private void SelfviewCameraButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSelfviewCameraButtonPressed.Raise(this);
		}

		private void ThumbnailPositionButtonListOnButtonClicked(object sender, EventArgs e)
		{
			OnThumbnailPositionButtonPressed.Raise(this, new UShortEventArgs(m_LayoutPositions.GetKey(sender as VtProButton)));
		}

		#endregion
	}
}
