using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	[ViewBinding(typeof(IConfirmLeaveCallView))]
	public sealed partial class ConfirmLeaveCallView : AbstractUiView, IConfirmLeaveCallView
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
		public ConfirmLeaveCallView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnYesButtonPressed = null;
			OnCancelButtonPressed = null;

			base.Dispose();
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_YesButton.OnPressed += YesButtonOnPressed;
			m_CancelButton.OnPressed += CancelButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_YesButton.OnPressed -= YesButtonOnPressed;
			m_CancelButton.OnPressed -= CancelButtonOnPressed;
		}

		private void CancelButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCancelButtonPressed.Raise(this);
		}

		private void YesButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnYesButtonPressed.Raise(this);
		}

		#endregion
	}
}
