using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views
{
	[ViewBinding(typeof(IGenericKeyboardView))]
	public sealed partial class GenericKeyboardView : AbstractTouchDisplayView, IGenericKeyboardView
	{
		/// <summary>
		/// Raised when the user presses a key button.
		/// </summary>
		public event EventHandler<KeyboardKeyEventArgs> OnKeyPressed;

		/// <summary>
		/// Raised when the user enters text in the text field.
		/// </summary>
		public event EventHandler<StringEventArgs> OnTextEntered;

		/// <summary>
		/// Raised when the user presses the backspace button.
		/// </summary>
		public event EventHandler OnBackspaceButtonPressed;

		/// <summary>
		/// Raised when the user presses the space button.
		/// </summary>
		public event EventHandler OnSpaceButtonPressed;

		/// <summary>
		/// Raised when the user presses the caps button.
		/// </summary>
		public event EventHandler OnCapsButtonPressed;

		/// <summary>
		/// Raised when the user presses the shift button.
		/// </summary>
		public event EventHandler OnShiftButtonPressed;

		/// <summary>
		/// Raised when the user presses the submit button.
		/// </summary>
		public event EventHandler OnSubmitButtonPressed;

		/// <summary>
		/// Raised when the user presses the close button. 
		/// </summary>
		public event EventHandler OnCloseButtonPressed;

		private Dictionary<VtProButton, KeyboardKey> m_KeyMap;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public GenericKeyboardView(ISigInputOutput panel, TouchCueTheme theme) 
			: base(panel, theme)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnKeyPressed = null;
			OnTextEntered = null;
			OnBackspaceButtonPressed = null;
			OnSpaceButtonPressed = null;
			OnCapsButtonPressed = null;
			OnShiftButtonPressed = null;
			OnSubmitButtonPressed = null;
			OnCloseButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the prompt above the text box.
		/// </summary>
		/// <param name="prompt"></param>
		public void SetPrompt(string prompt)
		{
			m_FeedbackText.SetLabelText(prompt);
		}

		/// <summary>
		/// Sets the text in the text entry field.
		/// </summary>
		/// <param name="text"></param>
		public void SetText(string text)
		{
			m_TextEntry.SetLabelTextAtJoin(m_TextEntry.SerialLabelJoins.First(), text);
		}

		/// <summary>
		/// Sets the selected state of the caps button.
		/// </summary>
		public void SelectCapsButton(bool @select)
		{
			m_CapsButton.SetSelected(@select);
		}

		/// <summary>
		/// Sets the selected state of the shift button.
		/// </summary>
		public void SelectShiftButton(bool @select)
		{
			m_ShiftButton.SetSelected(@select);
		}

		/// <summary>
		/// Sets the shift state of the chars.
		/// </summary>
		/// <param name="shift"></param>
		/// <param name="caps"></param>
		public void SetShift(bool shift, bool caps)
		{
			//All numbers & special characters use the same join.
			m_Key0Button.SetLabelTextAtJoin(m_Key0Button.DigitalLabelJoins.First(), shift);

			//All letters use the same join.
			m_KeyQButton.SetLabelTextAtJoin(m_KeyQButton.DigitalLabelJoins.First(), shift ^ caps);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_KeyMap = new Dictionary<VtProButton, KeyboardKey>
			{
				{m_Key0Button, new KeyboardKey('0', ')')},
				{m_Key1Button, new KeyboardKey('1', '!')},
				{m_Key2Button, new KeyboardKey('2', '@')},
				{m_Key3Button, new KeyboardKey('3', '#')},
				{m_Key4Button, new KeyboardKey('4', '$')},
				{m_Key5Button, new KeyboardKey('5', '%')},
				{m_Key6Button, new KeyboardKey('6', '^')},
				{m_Key7Button, new KeyboardKey('7', '&')},
				{m_Key8Button, new KeyboardKey('8', '*')},
				{m_Key9Button, new KeyboardKey('9', '(')},

				{m_KeyQButton, new KeyboardKey('q')},
				{m_KeyWButton, new KeyboardKey('w')},
				{m_KeyEButton, new KeyboardKey('e')},
				{m_KeyRButton, new KeyboardKey('r')},
				{m_KeyTButton, new KeyboardKey('t')},
				{m_KeyYButton, new KeyboardKey('y')},
				{m_KeyUButton, new KeyboardKey('u')},
				{m_KeyIButton, new KeyboardKey('i')},
				{m_KeyOButton, new KeyboardKey('o')},
				{m_KeyPButton, new KeyboardKey('p')},

				{m_KeyAButton, new KeyboardKey('a')},
				{m_KeySButton, new KeyboardKey('s')},
				{m_KeyDButton, new KeyboardKey('d')},
				{m_KeyFButton, new KeyboardKey('f')},
				{m_KeyGButton, new KeyboardKey('g')},
				{m_KeyHButton, new KeyboardKey('h')},
				{m_KeyJButton, new KeyboardKey('j')},
				{m_KeyKButton, new KeyboardKey('k')},
				{m_KeyLButton, new KeyboardKey('l')},

				{m_KeyZButton, new KeyboardKey('z')},
				{m_KeyXButton, new KeyboardKey('x')},
				{m_KeyCButton, new KeyboardKey('c')},
				{m_KeyVButton, new KeyboardKey('v')},
				{m_KeyBButton, new KeyboardKey('b')},
				{m_KeyNButton, new KeyboardKey('n')},
				{m_KeyMButton, new KeyboardKey('m')},

				{m_KeyPeriodButton, new KeyboardKey('.', '>')},
				{m_KeyCommaButton, new KeyboardKey(',', '<')},
				{m_KeySlashButton, new KeyboardKey('/', '?')},
				{m_KeySemiColonButton, new KeyboardKey(';', ':')},
				{m_KeyApostropheButton, new KeyboardKey('\'', '"')}
			};

			foreach (VtProButton button in m_KeyMap.Keys)
				button.OnPressed += ButtonOnPressed;

			m_TextEntry.OnTextModified += TextEntryOnTextModified;
			m_BackspaceButton.OnPressed += BackspaceButtonOnPressed;
			m_SpaceButton.OnPressed += SpaceButtonOnPressed;
			m_CapsButton.OnPressed += CapsButtonOnPressed;
			m_ShiftButton.OnPressed += ShiftButtonOnPressed;
			m_SubmitButton.OnPressed += SubmitButtonOnPressed;
			m_CloseButton.OnPressed += CloseButtonOnPressed;
		
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
			m_SpaceButton.OnPressed -= SpaceButtonOnPressed;
			m_CapsButton.OnPressed -= CapsButtonOnPressed;
			m_ShiftButton.OnPressed -= ShiftButtonOnPressed;
			m_SubmitButton.OnPressed -= SubmitButtonOnPressed;
			m_CloseButton.OnPressed -= CloseButtonOnPressed;
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
		/// Called when the user presses the shift button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ShiftButtonOnPressed(object sender, EventArgs args)
		{
			OnShiftButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the caps button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CapsButtonOnPressed(object sender, EventArgs args)
		{
			OnCapsButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the space button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SpaceButtonOnPressed(object sender, EventArgs args)
		{
			OnSpaceButtonPressed.Raise(this);
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
		/// <param name="eventArgs"></param>
		private void SubmitButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSubmitButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}
