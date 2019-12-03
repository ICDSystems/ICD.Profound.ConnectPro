using System;
using System.Collections.Generic;
using System.Linq;
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
		public event EventHandler<UShortEventArgs> OnButtonPressed;

		/// <summary>
		/// Raised when the user presses the close button.
		/// </summary>
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
			OnButtonPressed = null;
			OnCloseButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the selected state of a camera configuration button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetButtonSelected(ushort index, bool selected)
		{
			m_ButtonList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the visible state of a camera configuration button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetButtonVisible(ushort index, bool visible)
		{
			m_ButtonList.SetItemVisible(index, visible);
		}

		/// <summary>
		/// Sets the labels for the buttons.
		/// </summary>
		/// <param name="buttons"></param>
		public void SetButtons(IEnumerable<string> buttons)
		{
			m_ButtonList.SetItemLabels(buttons.ToArray());
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ButtonList.OnButtonClicked += ButtonListOnButtonClicked;
			m_CloseButton.OnPressed += CloseButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ButtonList.OnButtonClicked -= ButtonListOnButtonClicked;
			m_CloseButton.OnPressed -= CloseButtonOnPressed;
		}

		private void ButtonListOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}
