using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public partial class VtcKeypadView : AbstractView, IVtcKeypadView
	{
		public event EventHandler<KeyboardKeyEventArgs> OnKeyPressed;
		public event EventHandler<StringEventArgs> OnTextEntered;
		public event EventHandler OnBackspaceButtonPressed;
		public event EventHandler OnDialButtonPressed;
		public event EventHandler OnKeyboardButtonPressed;

		private Dictionary<VtProButton, KeyboardKey> m_KeyMap;

		public VtcKeypadView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetText(string text)
		{
			m_TextEntry.SetLabelTextAtJoin(m_TextEntry.SerialLabelJoins.First(), text);
		}

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_KeyMap = new Dictionary<VtProButton, KeyboardKey>();

			m_KeyMap[m_Key0Button] = new KeyboardKey('0', '0');
			m_KeyMap[m_Key1Button] = new KeyboardKey('1', '1');
			m_KeyMap[m_Key2Button] = new KeyboardKey('2', '2');
			m_KeyMap[m_Key3Button] = new KeyboardKey('3', '3');
			m_KeyMap[m_Key4Button] = new KeyboardKey('4', '4');
			m_KeyMap[m_Key5Button] = new KeyboardKey('5', '5');
			m_KeyMap[m_Key6Button] = new KeyboardKey('6', '6');
			m_KeyMap[m_Key7Button] = new KeyboardKey('7', '7');
			m_KeyMap[m_Key8Button] = new KeyboardKey('8', '8');
			m_KeyMap[m_Key9Button] = new KeyboardKey('9', '9');

			foreach (VtProButton button in m_KeyMap.Keys)
				button.OnPressed += ButtonOnPressed;

			m_DialButton.OnPressed += DialButtonOnPressed;
			m_KeyboardButton.OnPressed += KeyboardButtonOnPressed;
		}
			/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			foreach (VtProButton button in m_KeyMap.Keys)
				button.OnPressed -= ButtonOnPressed;

			m_TextEntry.OnTextModified -= TextEntryOnTextModified;
			m_BackspaceButton.OnPressed -= BackspaceButtonOnPressed;
			m_DialButton.OnPressed -= DialButtonOnPressed;
			m_KeyboardButton.OnPressed -= KeyboardButtonOnPressed;
		}

		/// <summary>
		/// Called when a key button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ButtonOnPressed(object sender, EventArgs args)
		{
			OnKeyPressed.Raise(this, new KeyboardKeyEventArgs(m_KeyMap[sender as VtProButton]));
		}

		/// <summary>
		/// Called when the user presses the backspace button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void BackspaceButtonOnPressed(object sender, EventArgs args)
		{
			OnBackspaceButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user enters text in the text field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void TextEntryOnTextModified(object sender, StringEventArgs args)
		{
			OnTextEntered.Raise(this, new StringEventArgs(args.Data));
		}

		/// <summary>
		/// Called when the user presses the submit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DialButtonOnPressed(object sender, EventArgs args)
		{
			OnDialButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the exit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void KeyboardButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnKeyboardButtonPressed.Raise(this);
		}
	}
}