using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.LeftMenu
{
	[ViewBinding(typeof(IWtcReferencedLeftMenuView))]
	public sealed partial class WtcReferencedLeftMenuView : AbstractComponentView, IWtcReferencedLeftMenuView
	{
		private const ushort MODE_OFF = 0;
		private const ushort MODE_ON = 1;

		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		public event EventHandler OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public WtcReferencedLeftMenuView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the visibility of the status light.
		/// </summary>
		/// <param name="visible"></param>
		public void SetStatusLightVisible(bool visible)
		{
			m_StatusButton.Show(visible);
		}

		/// <summary>
		/// Sets the text label for the button.
		/// </summary>
		/// <param name="label"></param>
		public void SetLabelText(string label)
		{
			m_Button.SetLabelText(label);
		}

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		public void SetIcon(string icon)
		{
			m_Icon.SetIcon(icon);
		}

		/// <summary>
		/// Sets the status light state.
		/// </summary>
		/// <param name="state"></param>
		public void SetStatusLightState(bool state)
		{
			m_StatusButton.SetMode(state ? MODE_ON : MODE_OFF);
		}

		/// <summary>
		/// Sets the selected state of the button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetButtonSelected(bool selected)
		{
			m_Button.SetSelected(selected);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Button.OnPressed += ButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Button.OnPressed -= ButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnButtonPressed.Raise(this);
		}

		#endregion
	}
}