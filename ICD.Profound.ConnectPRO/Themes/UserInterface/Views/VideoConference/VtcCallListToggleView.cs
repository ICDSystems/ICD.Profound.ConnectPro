using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	[ViewBinding(typeof(IVtcCallListToggleView))]
	public sealed partial class VtcCallListToggleView : AbstractUiView, IVtcCallListToggleView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		public event EventHandler OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcCallListToggleView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
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

		/// <summary>
		/// When true shows the "contacts" button, otherwise shows the "call" button.
		/// </summary>
		/// <param name="mode"></param>
		public void SetContactsMode(bool mode)
		{
			m_Button.SetSelected(!mode);
		}

		/// <summary>
		/// Sets the visibility of the button and label.
		/// </summary>
		/// <param name="visible"></param>
		public void SetButtonVisible(bool visible)
		{
			m_Button.Show(visible);
		}

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