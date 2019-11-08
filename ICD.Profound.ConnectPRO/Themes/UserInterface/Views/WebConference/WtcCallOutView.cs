using System;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	[ViewBinding(typeof(IWtcCallOutView))]
	public sealed partial class WtcCallOutView : AbstractUiView, IWtcCallOutView
	{
		/// <summary>
		/// Raised when the Call button is pressed.
		/// </summary>
		public event EventHandler OnCallButtonPressed;

		/// <summary>
		/// Raised when the clear button is pressed.
		/// </summary>
		public event EventHandler OnClearButtonPressed;

		/// <summary>
		/// Raised when the back button is pressed.
		/// </summary>
		public event EventHandler OnBackButtonPressed;

		/// <summary>
		/// Raised when a keypad button is pressed.
		/// </summary>
		public event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public WtcCallOutView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCallButtonPressed = null;
			OnClearButtonPressed = null;
			OnBackButtonPressed = null;
			OnKeypadButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the text value of the text entry box.
		/// </summary>
		/// <param name="text"></param>
		public void SetText(string text)
		{
			m_TextEntry.SetLabelText(text);
		}

		/// <summary>
		/// Sets the enabled state of the clear button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetClearButtonEnabled(bool enabled)
		{
			m_ClearButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the back button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetBackButtonEnabled(bool enabled)
		{
			m_BackButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the call button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetCallButtonEnabled(bool enabled)
		{
			m_CallButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the selected state of the call button;
		/// </summary>
		/// <param name="selected"></param>
		public void SetCallButtonSelected(bool selected)
		{
			m_CallButton.SetSelected(selected);
		}

		/// <summary>
		/// Sets the label text for the call button.
		/// </summary>
		/// <param name="label"></param>
		public void SetCallButtonLabel(string label)
		{
			m_CallButton.SetLabelText(label);
		}

		/// <summary>
		/// Sets the label text for the call status
		/// </summary>
		/// <param name="label"></param>
		public void SetCallStatusLabel(string label)
		{
			m_CallStatus.SetLabelTextAtJoin(m_CallStatus.SerialLabelJoins.First(), label);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CallButton.OnPressed += CallButtonOnPressed;
			m_Keypad.OnButtonPressed += KeypadOnButtonPressed;
			m_ClearButton.OnPressed += ClearButtonOnPressed;
			m_BackButton.OnPressed += BackButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CallButton.OnPressed -= CallButtonOnPressed;
			m_Keypad.OnButtonPressed -= KeypadOnButtonPressed;
			m_ClearButton.OnPressed -= ClearButtonOnPressed;
			m_BackButton.OnPressed -= BackButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses a keypad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void KeypadOnButtonPressed(object sender, SimpleKeypadEventArgs args)
		{
			char character = m_Keypad.GetButtonChar(args.Data);
			OnKeypadButtonPressed.Raise(this, new CharEventArgs(character));
		}

		/// <summary>
		/// Called when the user presses the call button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void CallButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCallButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the back button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void BackButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnBackButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the clear button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ClearButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnClearButtonPressed.Raise(this);
		}

		#endregion
	}
}