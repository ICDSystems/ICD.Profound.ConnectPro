using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups
{
	public sealed partial class AppleTvView : AbstractPopupView, IAppleTvView
	{
		public override event EventHandler OnCloseButtonPressed;

		public event EventHandler OnDPadUpButtonPressed;
		public event EventHandler OnDPadDownButtonPressed;
		public event EventHandler OnDPadLeftButtonPressed;
		public event EventHandler OnDPadRightButtonPressed;
		public event EventHandler OnDPadButtonReleased;
		public event EventHandler OnMenuButtonPressed;
		public event EventHandler OnPlayButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public AppleTvView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDPadUpButtonPressed = null;
			OnDPadDownButtonPressed = null;
			OnDPadLeftButtonPressed = null;
			OnDPadRightButtonPressed = null;
			OnDPadButtonReleased = null;
			OnMenuButtonPressed = null;
			OnPlayButtonPressed = null;

			base.Dispose();
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}
