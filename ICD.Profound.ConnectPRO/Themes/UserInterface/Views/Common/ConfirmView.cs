using System;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class ConfirmView : AbstractView, IConfirmView
	{
		/// <summary>
		/// Raised when the user presses the Yes button.
		/// </summary>
		public event EventHandler OnYesButtonPressed;

		/// <summary>
		/// Raised when the user presses the Cancel button.
		/// </summary>
		public event EventHandler OnCancelButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConfirmView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();
		}

		#endregion
	}
}
