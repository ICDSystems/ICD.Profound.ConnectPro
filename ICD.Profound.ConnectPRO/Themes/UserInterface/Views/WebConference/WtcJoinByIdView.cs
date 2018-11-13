using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public sealed partial class WtcJoinByIdView : AbstractView, IWtcJoinByIdView
	{
		public event EventHandler OnJoinMyMeetingButtonPressed;
		public event EventHandler<CharEventArgs> OnKeypadButtonPressed;
		public event EventHandler OnClearButtonPressed;
		public event EventHandler OnBackButtonPressed;
		public event EventHandler<StringEventArgs> OnTextEntered;

		public WtcJoinByIdView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public override void Dispose()
		{
			base.Dispose();

			OnJoinMyMeetingButtonPressed = null;
			OnKeypadButtonPressed = null;
			OnClearButtonPressed = null;
			OnBackButtonPressed = null;
			OnTextEntered = null;
		}

		#region Methods

		public void SetText(string text)
		{
			m_TextEntry.SetLabelText(text);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Keypad.OnButtonPressed += KeypadOnOnButtonPressed;
			m_JoinMyMeetingButton.OnPressed += JoinMyMeetingButtonOnOnPressed;
			m_TextEntry.OnTextModified += TextEntryOnOnTextModified;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Keypad.OnButtonPressed -= KeypadOnOnButtonPressed;
			m_JoinMyMeetingButton.OnPressed -= JoinMyMeetingButtonOnOnPressed;
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

		private void JoinMyMeetingButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnJoinMyMeetingButtonPressed.Raise(this);
		}

		private void TextEntryOnOnTextModified(object sender, StringEventArgs args)
		{
			OnTextEntered.Raise(this, new StringEventArgs(args.Data));
		}

		#endregion
	}
}