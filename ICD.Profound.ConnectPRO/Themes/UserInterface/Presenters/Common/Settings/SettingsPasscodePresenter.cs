using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	public sealed class SettingsPasscodePresenter : AbstractUiPresenter<ISettingsPasscodeView>, ISettingsPasscodePresenter
	{
		private const string INSTRUCTION_NEW_PASSCODE = "Please type in new passcode to change";
		private const string INSTRUCTION_CONFIRM_PASSCODE = "Please confirm new passcode";
		private const string INSTRUCTION_SUCCESS = "New passcode successfully set";
		private const string INSTRUCTION_FAIL = "Passcodes do not match";

		private readonly KeypadStringBuilder m_StringBuilder;
		private ePasscodeState m_State;
		private string m_Previous;

		private enum ePasscodeState
		{
			New,
			Confirm,
			Success,
			Fail
		}

		/// <summary>
		/// Lazy state machine
		/// </summary>
		private ePasscodeState State
		{
			get
			{
				return m_State;
			}
			set
			{
				if (value == m_State)
					return;

				m_State = value;

				switch (m_State)
				{
					case ePasscodeState.New:
					case ePasscodeState.Success:
					case ePasscodeState.Fail:
						m_Previous = null;
						break;

					case ePasscodeState.Confirm:
						m_Previous = m_StringBuilder.ToString();
						break;
					
					default:
						throw new ArgumentOutOfRangeException();
				}

				m_StringBuilder.Clear();

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsPasscodePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsPasscodeView view)
		{
			base.Refresh(view);

			string instruction = GetInstruction();
			string passcode = StringUtils.PasswordFormat(m_StringBuilder.ToString());

			view.SetInstructionLabel(instruction);
			view.SetPasscodeLabel(passcode);
		}

		#region Private Methods

		private void StringBuilderOnStringChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Gets the instruction label for the current state.
		/// </summary>
		/// <returns></returns>
		private string GetInstruction()
		{
			switch (m_State)
			{
				case ePasscodeState.New:
					return INSTRUCTION_NEW_PASSCODE;
				case ePasscodeState.Confirm:
					return INSTRUCTION_CONFIRM_PASSCODE;
				case ePasscodeState.Success:
					return INSTRUCTION_SUCCESS;
				case ePasscodeState.Fail:
					return INSTRUCTION_FAIL;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsPasscodeView view)
		{
			base.Subscribe(view);

			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnEnterButtonPressed += ViewOnEnterButtonPressed;
			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsPasscodeView view)
		{
			base.Unsubscribe(view);

			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnEnterButtonPressed -= ViewOnEnterButtonPressed;
			view.OnKeypadButtonPressed -= ViewOnKeypadButtonPressed;
		}

		private void ViewOnKeypadButtonPressed(object sender, CharEventArgs charEventArgs)
		{
			m_StringBuilder.AppendCharacter(charEventArgs.Data);
		}

		private void ViewOnEnterButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (m_State)
			{
				case ePasscodeState.New:
				case ePasscodeState.Success:
				case ePasscodeState.Fail:
					State = ePasscodeState.Confirm;
					break;
				case ePasscodeState.Confirm:
					Validate();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Validate()
		{
			string current = m_StringBuilder.ToString();

			if (string.IsNullOrEmpty(m_Previous) || current != m_Previous)
			{
				State = ePasscodeState.Fail;
				return;
			}

			if (Room != null)
				Room.Passcode = current;

			State = ePasscodeState.Success;
		}

		private void ViewOnClearButtonPressed(object sender, EventArgs eventArgs)
		{
			Reset();
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			Reset();
		}

		private void Reset()
		{
			State = ePasscodeState.New;
			m_StringBuilder.Clear();
			m_Previous = null;
		}

		#endregion
	}
}
