using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class PasscodePresenter : AbstractPresenter<IPasscodeView>, IPasscodePresenter
	{
		private readonly KeypadStringBuilder m_StringBuilder;

		private Action<IPasscodePresenter> m_SuccessCallback; 
		private bool m_ShowError;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public PasscodePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IPasscodeView view)
		{
			base.Refresh(view);

			string label = m_ShowError
				? "Invalid Passcode"
				: StringUtils.PasswordFormat(m_StringBuilder.ToString());

			if (string.IsNullOrEmpty(label))
				label = "Enter Passcode";

			view.SetPasscodeLabel(label);
		}
		
		/// <summary>
		/// Called when the string builder is updated.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StringBuilderOnStringChanged(object sender, StringEventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IPasscodeView view)
		{
			base.Subscribe(view);

			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
			view.OnEnterButtonPressed += ViewOnEnterButtonPressed;
			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnCancelButtonPressed += ViewOnCancelButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IPasscodeView view)
		{
			base.Unsubscribe(view);

			view.OnKeypadButtonPressed -= ViewOnKeypadButtonPressed;
			view.OnEnterButtonPressed -= ViewOnEnterButtonPressed;
			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnCancelButtonPressed -= ViewOnCancelButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the cancel button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCancelButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		/// <summary>
		/// Called when the user presses the clear button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnClearButtonPressed(object sender, EventArgs eventArgs)
		{
			m_ShowError = false;
			m_StringBuilder.Clear();
		}

		/// <summary>
		/// Called when the user presses the enter button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnEnterButtonPressed(object sender, EventArgs eventArgs)
		{
			string passcode = m_StringBuilder.ToString();

			// TODO - Pull passcode from room settings
			if (passcode == "1988")
			{
				HandleSuccess();
				return;
			}

			// Simple backdoor passcode for testing
			// Month * 100 + Day + Year
			// E.g.
			//	07/09/2018
			// Becomes
			//	0700 +
			//	0009 +
			//	2018
			//	----
			//	2727
			DateTime now = IcdEnvironment.GetLocalTime();

			int month = now.Month * 100;
			int day = now.Day;
			int year = now.Year;

			string expected = (month + day + year).ToString();
			if (passcode == expected)
			{
				HandleSuccess();
				return;
			}

			m_ShowError = true;
			m_StringBuilder.Clear();

			RefreshIfVisible();
		}

		/// <summary>
		/// Calls the success callback if it has been set.
		/// </summary>
		private void HandleSuccess()
		{
			if (m_SuccessCallback != null)
				m_SuccessCallback(this);
		}

		/// <summary>
		/// Called when the user presses a keypad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnKeypadButtonPressed(object sender, CharEventArgs eventArgs)
		{
			m_ShowError = false;
			m_StringBuilder.AppendCharacter(eventArgs.Data);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_ShowError = false;
			m_StringBuilder.Clear();

			if (!args.Data)
				m_SuccessCallback = null;
		}

		#endregion

		/// <summary>
		/// Shows the view and sets the success callback.
		/// </summary>
		/// <param name="successCallback"></param>
		public void ShowView(Action<IPasscodePresenter> successCallback)
		{
			m_SuccessCallback = successCallback;
			ShowView(true);
		}
	}
}
