using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcIncomingCallView : AbstractView, IVtcIncomingCallView
	{
		public event EventHandler OnAnswerButtonPressed;
		public event EventHandler OnIgoreButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcIncomingCallView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetCallerInfo(string number)
		{
			m_CallerInfoLabel.SetLabelText(number);
		}

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_AnswerButton.OnPressed += AnswerButtonOnPressed;
			m_IgnoreButton.OnPressed += IgnoreButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_AnswerButton.OnPressed -= AnswerButtonOnPressed;
			m_IgnoreButton.OnPressed -= IgnoreButtonOnPressed;
		}

		private void IgnoreButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnIgoreButtonPressed.Raise(this);
		}

		private void AnswerButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnAnswerButtonPressed.Raise(this);
		}
	}
}
