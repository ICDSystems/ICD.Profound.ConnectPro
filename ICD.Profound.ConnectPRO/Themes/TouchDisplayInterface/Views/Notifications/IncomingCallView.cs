using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Notifications;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Notifications
{
	[ViewBinding(typeof(IIncomingCallView))]
	public sealed partial class IncomingCallView : AbstractTouchDisplayView, IIncomingCallView
	{
		public event EventHandler OnAnswerButtonPressed;
		public event EventHandler OnRejectButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public IncomingCallView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetIcon(string icon)
		{
			m_Icon.SetImageUrl(icon);
		}

		public void SetSourceName(string name)
		{
			m_SourceNameLabel.SetLabelText(name);
		}

		public void SetCallerInfo(string number)
		{
			m_IncomingCallLabel.SetLabelText(number);
		}

		public void SetAnswerButtonMode(eIncomingCallAnswerButtonMode mode)
		{
			m_AnswerButton.SetMode((ushort) mode);
		}

		public void SetRejectButtonVisibility(bool visible)
		{
			m_RejectButton.Show(visible);
		}

		public void PlayRingtone(bool playing)
		{
			if (playing)
				m_Ringtone.Play(3300);
			else
				m_Ringtone.Stop();
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_AnswerButton.OnPressed += AnswerButtonOnPressed;
			m_RejectButton.OnPressed += RejectButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.SubscribeControls();

			m_AnswerButton.OnPressed -= AnswerButtonOnPressed;
			m_RejectButton.OnPressed -= RejectButtonOnPressed;
		}

		private void AnswerButtonOnPressed(object sender, EventArgs e)
		{
			OnAnswerButtonPressed.Raise(this);
		}

		private void RejectButtonOnPressed(object sender, EventArgs e)
		{
			OnRejectButtonPressed.Raise(this);
		}

		#endregion
	}
}