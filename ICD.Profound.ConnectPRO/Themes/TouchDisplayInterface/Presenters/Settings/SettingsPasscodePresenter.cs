using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Settings;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Settings
{
	[PresenterBinding(typeof(ISettingsPasscodePresenter))]
	public sealed class SettingsPasscodePresenter : AbstractTouchDisplayPresenter<ISettingsPasscodeView>, ISettingsPasscodePresenter
	{
		private readonly KeypadStringBuilder m_StringBuilder;
		private readonly SafeCriticalSection m_RefreshSection;

		private Action<ISettingsPasscodePresenter> m_SuccessCallback; 
		private bool m_ShowError;
		private int m_ErrorLength;
		private string m_InformationalVersion;
		private Version m_AssemblyVersion;

		#region Properties

		private Version AssemblyVersion
		{
			get { return m_AssemblyVersion ?? (m_AssemblyVersion = GetType().GetAssembly().GetName().Version); }
		}

		private string InformationalVersion
		{
			get
			{
				return m_InformationalVersion ?? (m_InformationalVersion = GetType().GetAssembly().GetInformationalVersion());
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsPasscodePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_RefreshSection = new SafeCriticalSection();

			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsPasscodeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string passcodeLabel = m_ShowError
					               ? StringUtils.Repeat("\u00d7", m_ErrorLength)
					               : StringUtils.PasswordFormat(m_StringBuilder.ToString());

				if (string.IsNullOrEmpty(passcodeLabel))
					passcodeLabel = "";

				string versionLabel = string.Format("ConnectPRO v{0} ({1})", InformationalVersion, AssemblyVersion);

				view.SetPasscodeLabel(passcodeLabel);
				view.SetVersionLabel(versionLabel);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Shows the view and sets the success callback.
		/// </summary>
		/// <param name="successCallback"></param>
		public void ShowView(Action<ISettingsPasscodePresenter> successCallback)
		{
			m_SuccessCallback = successCallback;
			ShowView(true);
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

		/// <summary>
		/// Calls the success callback if it has been set.
		/// </summary>
		private void HandleSuccess()
		{
			if (m_SuccessCallback != null)
				m_SuccessCallback(this);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsPasscodeView view)
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
		protected override void Unsubscribe(ISettingsPasscodeView view)
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
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the user presses the enter button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnEnterButtonPressed(object sender, EventArgs eventArgs)
		{
			string passcode = m_StringBuilder.ToString();

			if (Room != null && !string.IsNullOrEmpty(Room.Passcode) && passcode == Room.Passcode)
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
			m_ErrorLength = m_StringBuilder.ToString().Length;
			m_StringBuilder.Clear();

			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the user presses a keypad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnKeypadButtonPressed(object sender, CharEventArgs eventArgs)
		{
			m_ShowError = false;

			if (m_StringBuilder.ToString().Length < 4)
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
	}
}
