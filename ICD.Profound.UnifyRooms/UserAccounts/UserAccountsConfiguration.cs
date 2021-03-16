using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Profound.UnifyRooms.UserAccounts
{
	public sealed class UserAccountsConfiguration
	{
		private const string ELEMENT_ACCOUNT = "Account";

		private readonly List<UserAccount> m_Accounts;

		#region Properties

		/// <summary>
		/// Gets/sets the first user account.
		/// Used for DAV compatibility.
		/// </summary>
		[NotNull]
		public UserAccount Account1
		{
			get
			{
				while (m_Accounts.Count < 1)
					m_Accounts.Add(new UserAccount());
				return m_Accounts[0];
			}
		}

		/// <summary>
		/// Gets/sets the second user account.
		/// Used for DAV compatibility.
		/// </summary>
		[NotNull]
		public UserAccount Account2
		{
			get
			{
				while (m_Accounts.Count < 2)
					m_Accounts.Add(new UserAccount());
				return m_Accounts[1];
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public UserAccountsConfiguration()
		{
			m_Accounts = new List<UserAccount>();

			Clear();
		}

		#region Methods

		/// <summary>
		/// Reverts the settings to defaults.
		/// </summary>
		public void Clear()
		{
			m_Accounts.Clear();
		}

		/// <summary>
		/// Gets the configured user accounts.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public IEnumerable<UserAccount> GetAccounts()
		{
			return m_Accounts.ToArray();
		}

		/// <summary>
		/// Gets the configured user accounts.
		/// </summary>
		/// <param name="accounts"></param>
		public void SetAccounts([NotNull] IEnumerable<UserAccount> accounts)
		{
			if (accounts == null)
				throw new ArgumentNullException("accounts");

			Clear();
			m_Accounts.AddRange(accounts);
		}

		#endregion

		#region Serialization

		/// <summary>
		/// Copies the values from the other instance.
		/// </summary>
		/// <param name="other"></param>
		public void Copy([NotNull] UserAccountsConfiguration other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			Clear();

			foreach (UserAccount otherAccount in other.GetAccounts())
			{
				UserAccount account = new UserAccount();
				account.Copy(otherAccount);

				m_Accounts.Add(account);
			}
		}

		/// <summary>
		/// Updates the settings from the given xml element.
		/// </summary>
		/// <param name="xml"></param>
		public void ParseXml(string xml)
		{
			IEnumerable<UserAccount> accounts =
				XmlUtils.ReadListFromXml(xml, ELEMENT_ACCOUNT, x =>
				{
					UserAccount account = new UserAccount();
					account.ParseXml(x);
					return account;
				});

			SetAccounts(accounts);
		}

		/// <summary>
		/// Writes the current configuration to the given XML writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		public void ToXml(IcdXmlTextWriter writer, string element)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			XmlUtils.WriteListToXml(writer, m_Accounts, element, (w, a) => a.ToXml(w, ELEMENT_ACCOUNT));
		}

		#endregion
	}
}
