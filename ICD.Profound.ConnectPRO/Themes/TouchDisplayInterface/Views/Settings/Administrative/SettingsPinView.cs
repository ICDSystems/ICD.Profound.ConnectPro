using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings.Administrative;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Settings.Administrative
{
	[ViewBinding(typeof(ISettingsPinView))]
	public sealed partial class SettingsPinView : AbstractTouchDisplayView, ISettingsPinView
	{
		/// <summary>
		/// Raised when the user presses a keypad button.
		/// </summary>
		public event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		/// <summary>
		/// Raised when the user presses the clear button.
		/// </summary>
		public event EventHandler OnClearButtonPressed;

		/// <summary>
		/// Raised when the user presses the enter button.
		/// </summary>
		public event EventHandler OnEnterButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsPinView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnKeypadButtonPressed = null;
			OnClearButtonPressed = null;
			OnEnterButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the instructional text.
		/// </summary>
		/// <param name="label"></param>
		public void SetInstructionLabel(string label)
		{
			m_InstructionLabel.SetLabelText(label);
		}

		/// <summary>
		/// Sets the passcode text.
		/// </summary>
		/// <param name="label"></param>
		public void SetPasscodeLabel(string label)
		{
			label = label ?? string.Empty;

			for (ushort index = 0; index < 4; index++)
			{
				char item;

				if (label.TryElementAt(index, out item))
				{
					m_PasscodeLabel.SetItemSelected(index, true);
					m_PasscodeLabel.SetItemLabel(index, item.ToString());
				}
				else
				{
					m_PasscodeLabel.SetItemSelected(index, false);
					m_PasscodeLabel.SetItemLabel(index, "0");
				}
			}
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Keypad.OnButtonPressed += KeypadOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Keypad.OnButtonPressed -= KeypadOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a keypad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void KeypadOnButtonPressed(object sender, SimpleKeypadEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case SimpleKeypadEventArgs.eButton.MiscButtonOne:
					OnClearButtonPressed.Raise(this);
					break;

				case SimpleKeypadEventArgs.eButton.MiscButtonTwo:
					OnEnterButtonPressed.Raise(this);
					break;

				default:
					char character = m_Keypad.GetButtonChar(eventArgs.Data);
					OnKeypadButtonPressed.Raise(this, new CharEventArgs(character));
					break;
			}
		}

		#endregion
	}
}
