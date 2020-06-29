using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	[ViewBinding(typeof(ITouchFreeCancelPromptView))]
	public sealed partial class TouchFreeCancelPromptView : AbstractUiView, ITouchFreeCancelPromptView
	{
		/// <summary>
		/// Raised when the user presses the Cancel Meeting Start button.
		/// </summary>
		public event EventHandler OnCancelMeetingStartPressed;

		public void SetTimer(int seconds)
		{
			m_Message.SetLabelText(seconds.ToString());
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public TouchFreeCancelPromptView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCancelMeetingStartPressed = null;

			base.Dispose();
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CancelMeetingStartButton.OnPressed += CancelMeetingStartButtonOnPressed;
			
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CancelMeetingStartButton.OnPressed -= CancelMeetingStartButtonOnPressed;

		}


		private void CancelMeetingStartButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCancelMeetingStartPressed.Raise(this);
		}

		#endregion
	}
}