using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public sealed partial class WtcStartMeetingView : AbstractUiView, IWtcStartMeetingView
	{
		public event EventHandler OnMeetNowButtonPressed;
		public event EventHandler OnJoinByIdButtonPressed;
		public event EventHandler<CharEventArgs> OnKeypadButtonPressed;
		public event EventHandler OnClearButtonPressed;
		public event EventHandler OnBackButtonPressed;
		public event EventHandler<StringEventArgs> OnTextEntered;

		public WtcStartMeetingView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public override void Dispose()
		{
			base.Dispose();

			OnMeetNowButtonPressed = null;
			OnJoinByIdButtonPressed = null;
			OnKeypadButtonPressed = null;
			OnClearButtonPressed = null;
			OnBackButtonPressed = null;
			OnTextEntered = null;
		}

		#region Methods

		public void SetMeetNowButtonEnabled(bool enabled)
		{
			m_MeetNowButton.Enable(enabled);
		}

		public void SetJoinByIdButtonEnabled(bool enabled)
		{
			m_JoinByIdButton.Enable(enabled);
		}

		public void SetMeetingIdText(string text)
		{
			m_TextEntry.SetLabelText(text);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_MeetNowButton.OnPressed += MeetNowButtonOnOnPressed;
			m_JoinByIdButton.OnPressed += JoinByIdButtonOnOnPressed;
			m_Keypad.OnButtonPressed += KeypadOnOnButtonPressed;
			m_TextEntry.OnTextModified += TextEntryOnOnTextModified;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_MeetNowButton.OnPressed -= MeetNowButtonOnOnPressed;
			m_JoinByIdButton.OnPressed -= JoinByIdButtonOnOnPressed;
			m_Keypad.OnButtonPressed -= KeypadOnOnButtonPressed;
			m_TextEntry.OnTextModified -= TextEntryOnOnTextModified;
		}

		private void JoinByIdButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnJoinByIdButtonPressed.Raise(this);
		}

		private void MeetNowButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnMeetNowButtonPressed.Raise(this);
		}

		private void KeypadOnOnButtonPressed(object sender, SimpleKeypadEventArgs args)
		{
			switch (args.Data)
			{
				case SimpleKeypadEventArgs.eButton.MiscButtonOne:
					OnBackButtonPressed.Raise(this);
					break;
				case SimpleKeypadEventArgs.eButton.MiscButtonTwo:
					OnClearButtonPressed.Raise(this);
					break;
				default:
					OnKeypadButtonPressed.Raise(this, new CharEventArgs(m_Keypad.GetButtonChar(args.Data)));
					break;
			}
		}

		private void TextEntryOnOnTextModified(object sender, StringEventArgs args)
		{
			OnTextEntered.Raise(this, new StringEventArgs(args.Data));
		}

		#endregion
	}
}