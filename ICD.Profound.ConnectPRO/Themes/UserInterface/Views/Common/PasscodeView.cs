using System;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class PasscodeView : AbstractUiView, IPasscodeView
	{
		/// <summary>
		/// Raised when the user presses the cancel button.
		/// </summary>
		public event EventHandler OnCancelButtonPressed;

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
		public PasscodeView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCancelButtonPressed = null;
			OnKeypadButtonPressed = null;
			OnClearButtonPressed = null;
			OnEnterButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the label text for the passcode.
		/// </summary>
		/// <param name="label"></param>
		public void SetPasscodeLabel(string label)
		{
			m_PasscodeLabel.SetLabelTextAtJoin(m_PasscodeLabel.SerialLabelJoins.First(), label);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Keypad.OnButtonPressed += KeypadOnButtonPressed;
			m_CancelButton.OnPressed += CancelButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Keypad.OnButtonPressed -= KeypadOnButtonPressed;
			m_CancelButton.OnPressed -= CancelButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the cancel button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void CancelButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCancelButtonPressed.Raise(this);
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
					char digit = m_Keypad.GetButtonChar(eventArgs.Data);
					OnKeypadButtonPressed.Raise(this, new CharEventArgs(digit));
					break;
			}
		}

		#endregion
	}
}
