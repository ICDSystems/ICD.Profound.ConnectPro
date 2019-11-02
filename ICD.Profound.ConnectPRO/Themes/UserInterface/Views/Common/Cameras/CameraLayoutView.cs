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
		public event EventHandler<UShortEventArgs> OnLayoutShareButtonPressed;

		/// <summary>
		/// Raised when a self-view layout configuration button is pressed.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnLayoutSelfViewButtonPressed;

		/// <summary>
		/// Raised when a position layout configuration button is pressed.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnLayoutPositionButtonPressed;

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
			OnLayoutShareButtonPressed = null;
			OnLayoutSelfViewButtonPressed = null;
			OnLayoutPositionButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the selected state of a button for the size layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetLayoutSizeControlButtonSelected(ushort index, bool selected)
		{
			m_SizeLayoutControl.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the style layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetLayoutStyleControlButtonSelected(ushort index, bool selected)
		{
			m_StyleLayoutControl.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the share layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetLayoutShareControlButtonSelected(ushort index, bool selected)
		{
			m_ShareLayoutControl.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the self-view layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetLayoutSelfViewControlButtonSelected(ushort index, bool selected)
		{
			m_SelfViewLayoutControl.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the selected state of a button for the position layout control.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetLayoutPositionControlButtonSelected(ushort index, bool selected)
		{
			m_PositionLayoutControl.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the enabled state of the size layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetLayoutSizeListEnabled(bool enabled)
		{
			m_SizeLayoutControl.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the style layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetLayoutStyleListEnable(bool enabled)
		{
			m_StyleLayoutControl.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the share layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetLayoutShareListEnable(bool enabled)
		{
			m_ShareLayoutControl.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the self-view layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetLayoutSelfViewListEnable(bool enabled)
		{
			m_SelfViewLayoutControl.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the position layout controls dynamic button list.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetLayoutPositionListEnable(bool enabled)
		{
			m_PositionLayoutControl.Enable(enabled);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();
			
			m_SizeLayoutControl.OnButtonClicked += SizeLayoutControlOnButtonClicked;
			m_StyleLayoutControl.OnButtonClicked += StyleLayoutControlOnButtonClicked;
			m_ShareLayoutControl.OnButtonClicked += ShareLayoutControlOnButtonClicked;
			m_SelfViewLayoutControl.OnButtonClicked += SelfViewLayoutControlOnButtonClicked;
			m_PositionLayoutControl.OnButtonClicked += PositionLayoutControlOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();
			
			m_SizeLayoutControl.OnButtonClicked -= SizeLayoutControlOnButtonClicked;
			m_StyleLayoutControl.OnButtonClicked -= StyleLayoutControlOnButtonClicked;
			m_ShareLayoutControl.OnButtonClicked -= ShareLayoutControlOnButtonClicked;
			m_SelfViewLayoutControl.OnButtonClicked -= SelfViewLayoutControlOnButtonClicked;
			m_PositionLayoutControl.OnButtonClicked -= PositionLayoutControlOnButtonClicked;
		}

		private void SizeLayoutControlOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnLayoutSizeButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		private void StyleLayoutControlOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnLayoutStyleButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		private void ShareLayoutControlOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnLayoutShareButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		private void SelfViewLayoutControlOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnLayoutSelfViewButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		private void PositionLayoutControlOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnLayoutPositionButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		#endregion
	}
}
