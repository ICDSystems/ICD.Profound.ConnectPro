using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Conference
{
	[ViewBinding(typeof(IStartConferenceView))]
	public sealed partial class StartConferenceView : AbstractTouchDisplayView, IStartConferenceView
	{
		private const int NUM_OF_DIGITS = 11;

		public StartConferenceView(ISigInputOutput panel, TouchCueTheme theme)
			: base(panel, theme)
		{
		}

		public event EventHandler OnMeetNowButtonPressed;
		public event EventHandler OnJoinByIdButtonPressed;
		public event EventHandler<CharEventArgs> OnKeypadButtonPressed;
		public event EventHandler OnClearButtonPressed;
		public event EventHandler OnBackButtonPressed;
		public event EventHandler<StringEventArgs> OnTextEntered;

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
			text = text ?? string.Empty;
			m_TextEntry.SetNumberOfItems(NUM_OF_DIGITS);
			for (ushort index = 0; index < NUM_OF_DIGITS; index++)
			{
				m_TextEntry.SetItemVisible(index, true);

				char item;
				if (text.TryElementAt(index, out item))
				{
					m_TextEntry.SetItemSelected(index, true);
					m_TextEntry.SetItemLabel(index, item.ToString());
				}
				else
				{
					m_TextEntry.SetItemSelected(index, false);
					m_TextEntry.SetItemLabel(index, "");
				}
			}
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();
			
			m_MeetNowButton.OnPressed += MeetNowButtonOnOnPressed;
			m_JoinByIdButton.OnPressed += JoinByIdButtonOnOnPressed;
			m_Keypad.OnButtonPressed += KeypadOnOnButtonPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_MeetNowButton.OnPressed -= MeetNowButtonOnOnPressed;
			m_JoinByIdButton.OnPressed -= JoinByIdButtonOnOnPressed;
			m_Keypad.OnButtonPressed -= KeypadOnOnButtonPressed;
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

		#endregion
	}
}