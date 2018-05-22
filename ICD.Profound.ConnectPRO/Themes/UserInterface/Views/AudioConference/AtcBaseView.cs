using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.AudioConference
{
	public sealed partial class AtcBaseView : AbstractPopupView, IAtcBaseView
	{
		/// <summary>
		/// Raised when the user presses the close button.
		/// </summary>
		public override event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Raised when the user presses the dial button.
		/// </summary>
		public event EventHandler OnDialButtonPressed;

		/// <summary>
		/// Raised when the user presses the hangup button.
		/// </summary>
		public event EventHandler OnHangupButtonPressed;

		/// <summary>
		/// Raised when the user presses the clear button.
		/// </summary>
		public event EventHandler OnClearButtonPressed;

		/// <summary>
		/// Raised when the user presses a keypad button.
		/// </summary>
		public event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public AtcBaseView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCloseButtonPressed = null;
			OnDialButtonPressed = null;
			OnHangupButtonPressed = null;
			OnClearButtonPressed = null;
			OnKeypadButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the active call status text.
		/// </summary>
		/// <param name="status"></param>
		public void SetCallStatus(string status)
		{
			m_CallStatusLabel.SetLabelTextAtJoin(m_CallStatusLabel.SerialLabelJoins.First(), status);
		}

		/// <summary>
		/// Sets the text for the number being dialed.
		/// </summary>
		/// <param name="number"></param>
		public void SetDialNumber(string number)
		{
			m_DialNumberLabel.SetLabelTextAtJoin(m_DialNumberLabel.SerialLabelJoins.First(), number);
		}

		/// <summary>
		/// Sets the text for the room number.
		/// </summary>
		/// <param name="number"></param>
		public void SetRoomNumber(string number)
		{
			m_RoomNumberLabel.SetLabelTextAtJoin(m_RoomNumberLabel.SerialLabelJoins.First(), number);
		}

		/// <summary>
		/// Sets the enabled state of the dial button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetDialButtonEnabled(bool enabled)
		{
			IcdConsole.PrintLine(eConsoleColor.Magenta, "SetDialButtonEnabled {0}", enabled);

			m_DialButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the hangup button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetHangupButtonEnabled(bool enabled)
		{
			IcdConsole.PrintLine(eConsoleColor.Magenta, "SetHangupButtonEnabled {0}", enabled);

			m_HangupButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the clear button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetClearButtonEnabled(bool enabled)
		{
			IcdConsole.PrintLine(eConsoleColor.Magenta, "SetClearButtonEnabled {0}", enabled);

			m_ClearButton.Enable(enabled);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
			m_DialButton.OnPressed += DialButtonOnPressed;
			m_HangupButton.OnPressed += HangupButtonOnPressed;
			m_ClearButton.OnPressed += ClearButtonOnPressed;
			m_DialKeypad.OnButtonPressed += DialKeypadOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
			m_DialButton.OnPressed -= DialButtonOnPressed;
			m_HangupButton.OnPressed -= HangupButtonOnPressed;
			m_ClearButton.OnPressed -= ClearButtonOnPressed;
			m_DialKeypad.OnButtonPressed -= DialKeypadOnButtonPressed;
		}

		private void DialKeypadOnButtonPressed(object sender, SimpleKeypadEventArgs eventArgs)
		{
			char character = m_DialKeypad.GetButtonChar(eventArgs.Data);

			OnKeypadButtonPressed.Raise(this, new CharEventArgs(character));
		}

		private void ClearButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnClearButtonPressed.Raise(this);
		}

		private void HangupButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnHangupButtonPressed.Raise(this);
		}

		private void DialButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDialButtonPressed.Raise(this);
		}

		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}
