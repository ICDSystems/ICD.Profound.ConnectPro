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
	public sealed partial class VtcKeyboardView : AbstractView, IVtcKeyboardView
	{
		public event EventHandler<KeyboardKeyEventArgs> OnKeyPressed;
		public event EventHandler<StringEventArgs> OnTextEntered;
		public event EventHandler OnBackspaceButtonPressed;
		public event EventHandler OnSpaceButtonPressed;
		public event EventHandler OnCapsButtonPressed;
		public event EventHandler OnShiftButtonPressed;
		public event EventHandler OnSubmitButtonPressed;

		private Dictionary<VtProButton, KeyboardKey> m_KeyMap;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcKeyboardView(ISigInputOutput panel, ConnectProTheme theme)
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

			base.Dispose();
		}

		/// <summary>
		/// Sets the selected state of the caps button.
		/// </summary>
		public void SelectCapsButton(bool select)
		{
			m_CapsButton.SetSelected(@select);
		}

		/// <summary>
		/// Sets the selected state of the shift button.
		/// </summary>
		public void SelectShiftButton(bool select)
		{
			m_ShiftButton.SetSelected(@select);
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
		/// Sets the shift state of the chars.
		/// </summary>
		/// <param name="shift"></param>
		/// <param name="caps"></param>
		public void SetShift(bool shift, bool caps)
		{
			// All letters use the same join
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

			m_KeyMap = new Dictionary<VtProButton, KeyboardKey>();

			m_KeyMap[m_KeyQButton] = new KeyboardKey('q');
			m_KeyMap[m_KeyWButton] = new KeyboardKey('w');
			m_KeyMap[m_KeyEButton] = new KeyboardKey('e');
			m_KeyMap[m_KeyRButton] = new KeyboardKey('r');
			m_KeyMap[m_KeyTButton] = new KeyboardKey('t');
			m_KeyMap[m_KeyYButton] = new KeyboardKey('y');
			m_KeyMap[m_KeyUButton] = new KeyboardKey('u');
			m_KeyMap[m_KeyIButton] = new KeyboardKey('i');
			m_KeyMap[m_KeyOButton] = new KeyboardKey('o');
			m_KeyMap[m_KeyPButton] = new KeyboardKey('p');

			m_KeyMap[m_KeyAButton] = new KeyboardKey('a');
			m_KeyMap[m_KeySButton] = new KeyboardKey('s');
			m_KeyMap[m_KeyDButton] = new KeyboardKey('d');
			m_KeyMap[m_KeyFButton] = new KeyboardKey('f');
			m_KeyMap[m_KeyGButton] = new KeyboardKey('g');
			m_KeyMap[m_KeyHButton] = new KeyboardKey('h');
			m_KeyMap[m_KeyJButton] = new KeyboardKey('j');
			m_KeyMap[m_KeyKButton] = new KeyboardKey('k');
			m_KeyMap[m_KeyLButton] = new KeyboardKey('l');

			m_KeyMap[m_KeyZButton] = new KeyboardKey('z');
			m_KeyMap[m_KeyXButton] = new KeyboardKey('x');
			m_KeyMap[m_KeyCButton] = new KeyboardKey('c');
			m_KeyMap[m_KeyVButton] = new KeyboardKey('v');
			m_KeyMap[m_KeyBButton] = new KeyboardKey('b');
			m_KeyMap[m_KeyNButton] = new KeyboardKey('n');
			m_KeyMap[m_KeyMButton] = new KeyboardKey('m');

			m_KeyMap[m_KeyAtButton] = new KeyboardKey('@', '@');
			m_KeyMap[m_KeyStopButton] = new KeyboardKey('.', '.');

			foreach (VtProButton button in m_KeyMap.Keys)
				button.OnPressed += ButtonOnPressed;

			m_TextEntry.OnTextModified += TextEntryOnTextModified;
			m_BackspaceButton.OnPressed += BackspaceButtonOnPressed;
			m_SpaceButton.OnPressed += SpaceButtonOnPressed;
			m_CapsButton.OnPressed += CapsButtonOnPressed;
			m_ShiftButton.OnPressed += ShiftButtonOnPressed;
			m_SubmitButton.OnPressed += SubmitButtonOnPressed;
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
		/// <param name="args"></param>
		private void SubmitButtonOnPressed(object sender, EventArgs args)
		{
			OnSubmitButtonPressed.Raise(this);
		}

		#endregion
	}
}
