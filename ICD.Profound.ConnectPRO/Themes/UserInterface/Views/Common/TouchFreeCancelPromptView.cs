using System;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	[ViewBinding(typeof(ITouchFreeCancelPromptView))]
	public sealed partial class TouchFreeCancelPromptView : AbstractUiView, ITouchFreeCancelPromptView
	{
		/// <summary>
		/// Raised when the user presses the Cancel Meeting Start button.
		/// </summary>
		public event EventHandler OnCancelMeetingStartPressed;

		/// <summary>
		/// Raised when the user presses the Start Meeting Now button.
		/// </summary>
		public event EventHandler OnStartMeetingNowPressed;

		public void SetTimer(int seconds)
		{
			m_Message.SetLabelTextAtJoin(m_Message.SerialLabelJoins.First(), seconds.ToString());
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public TouchFreeCancelPromptView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCancelMeetingStartPressed = null;
			OnStartMeetingNowPressed = null;

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
			m_StartMeetingNowButton.OnPressed += StartMeetingNowButtonOnPressed;
			
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CancelMeetingStartButton.OnPressed -= CancelMeetingStartButtonOnPressed;
			m_StartMeetingNowButton.OnPressed -= StartMeetingNowButtonOnPressed;

		}


		private void CancelMeetingStartButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCancelMeetingStartPressed.Raise(this);
		}

		private void StartMeetingNowButtonOnPressed(object sender, EventArgs e)
		{
			OnStartMeetingNowPressed.Raise(this);
		}

		#endregion
	}
}