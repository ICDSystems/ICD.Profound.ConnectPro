using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.FloatingActions
{
	[ViewBinding(typeof(IFloatingActionPrivacyMuteView))]
	public sealed partial class FloatingActionPrivacyMuteView : AbstractFloatingActionView, IFloatingActionPrivacyMuteView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		public override event EventHandler OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public FloatingActionPrivacyMuteView(ISigInputOutput panel, ConnectProTheme theme)
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
		/// Sets the selected state of the option.
		/// </summary>
		/// <param name="active"></param>
		public override void SetActive(bool active)
		{
			m_Button.SetSelected(active);
		}

		/// <summary>
		/// Sets the enabled state of the option.
		/// </summary>
		/// <param name="enabled"></param>
		public override void SetEnabled(bool enabled)
		{
			m_Button.Enable(enabled);
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
