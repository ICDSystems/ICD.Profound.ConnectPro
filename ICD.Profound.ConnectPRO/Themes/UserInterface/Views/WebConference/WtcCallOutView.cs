using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public sealed partial class WtcCallOutView : AbstractUiView, IWtcCallOutView
	{
		public event EventHandler<StringEventArgs> OnTextEntered;
		public event EventHandler OnCallButtonPressed;
		public event EventHandler OnHangupButtonPressed;
		public event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		public WtcCallOutView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		#region Methods

		public void SetText(string text)
		{
			m_TextEntry.SetLabelText(text);
		}

		public void SetCallButtonEnabled(bool enabled)
		{
			m_CallButton.Enable(enabled);
		}

		public void SetHangupButtonEnabled(bool enabled)
		{
			m_HangupButton.Enable(enabled);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CallButton.OnPressed += CallButtonOnOnPressed;
			m_HangupButton.OnPressed += HangupButtonOnOnPressed;
			m_Keypad.OnButtonPressed += KeypadOnOnButtonPressed;
			m_TextEntry.OnTextModified += TextEntryOnOnTextModified;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CallButton.OnPressed -= CallButtonOnOnPressed;
			m_HangupButton.OnPressed -= HangupButtonOnOnPressed;
			m_Keypad.OnButtonPressed -= KeypadOnOnButtonPressed;
			m_TextEntry.OnTextModified -= TextEntryOnOnTextModified;
		}

		private void TextEntryOnOnTextModified(object sender, StringEventArgs args)
		{
			OnTextEntered.Raise(this, new StringEventArgs(args.Data));
		}

		private void KeypadOnOnButtonPressed(object sender, SimpleKeypadEventArgs args)
		{
			OnKeypadButtonPressed.Raise(this, new CharEventArgs(m_Keypad.GetButtonChar(args.Data)));
		}

		private void CallButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnCallButtonPressed.Raise(this);
		}

		private void HangupButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnHangupButtonPressed.Raise(this);
		}

		#endregion
	}
}