using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
#if !SIMPLSHARP
using ICD.Connect.Misc.Windows.Utils;
#endif
using ICD.Profound.UnifyRooms.Devices.UnifyBar;
using ICD.Profound.UnifyRooms.UserAccounts;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons
{
	public sealed class UnifyBarSwitchAccountsButtonUi : AbstractUnifyBarButtonUi
	{
		private readonly UserAccountsConfiguration m_Accounts;

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyBarSwitchAccountsButtonUi([NotNull] UserAccountsConfiguration accounts)
		{
			if (accounts == null)
				throw new ArgumentNullException("accounts");

			m_Accounts = accounts;

			UpdateAccountState();

			IcdEnvironment.OnSessionChangedEvent += IcdEnvironmentOnSessionChangedEvent;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			IcdEnvironment.OnSessionChangedEvent -= IcdEnvironmentOnSessionChangedEvent;
		}

		/// <summary>
		/// Updates the button to reflect the current account.
		/// </summary>
		private void UpdateAccountState()
		{
			UserAccount other = GetOtherAccount();

			string name = other == null ? string.Empty : other.Name ?? string.Empty;
			bool teams = string.Equals(name, "Teams", StringComparison.OrdinalIgnoreCase);
			bool zoom = string.Equals(name, "Zoom", StringComparison.OrdinalIgnoreCase);

			Icon = teams ? eMainButtonIcon.Teams : zoom ? eMainButtonIcon.Zoom : default(eMainButtonIcon);
			Type = eMainButtonType.SwitchAccount;
			Label = name == null ? string.Empty : name.ToUpper();
			Visible = other != null;
			Enabled = true;
		}

		/// <summary>
		/// Override to implement the button press behaviour.
		/// </summary>
		/// <param name="pressed"></param>
		protected override void HandleButtonPress(bool pressed)
		{
#if !SIMPLSHARP
			UserAccount other = GetOtherAccount();
			if (other != null)
				LogonUtils.SwitchUser(other.Username, other.Password);
#endif
		}

		/// <summary>
		/// Gets the account details that the button will switch to when pressed.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private UserAccount GetOtherAccount()
		{
			return m_Accounts.GetAccounts().FirstOrDefault(a => a.IsValid && !a.IsCurrent);
		}

		/// <summary>
		/// Called when the current user session changes.
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="type"></param>
		private void IcdEnvironmentOnSessionChangedEvent(int sessionId, IcdEnvironment.eSessionChangeEventType type)
		{
			UpdateAccountState();
		}
	}
}
