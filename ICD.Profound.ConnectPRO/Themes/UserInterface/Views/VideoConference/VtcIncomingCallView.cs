using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	[ViewBinding(typeof(IVtcIncomingCallView))]
	public sealed partial class VtcIncomingCallView : AbstractUiView, IVtcIncomingCallView
	{
		public event EventHandler OnAnswerButtonPressed;
		public event EventHandler OnIgnoreButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcIncomingCallView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnAnswerButtonPressed = null;
			OnIgnoreButtonPressed = null;

			base.Dispose();
		}

		public void SetCallerInfo(string number)
		{
			m_CallerInfoLabel.SetLabelText(number);
		}

		/// <summary>
		/// Play or stop the incoming call ringtone.
		/// </summary>
		/// <param name="playing"></param>
		public void PlayRingtone(bool playing)
		{
			if (playing)
				m_Ringtone.Play(3300);
			else
				m_Ringtone.Stop();
		}

		#region Control Callbacks

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
			OnIgnoreButtonPressed.Raise(this);
		}

		private void AnswerButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnAnswerButtonPressed.Raise(this);
		}

		#endregion
	}
}
