using System;
#if !SIMPLSHARP
using System.Linq;
using System.Management;
using System.Security.Principal;
using ICD.Connect.Misc.Windows.Utils;
#endif
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Profound.UnifyRooms.UserAccounts
{
	public sealed class UserAccount
	{
		private const string ELEMENT_NAME = "Name";
		private const string ELEMENT_USERNAME = "Username";
		private const string ELEMENT_PASSWORD = "Password";

		#region Properties

		/// <summary>
		/// Gets the nice-name for the account.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets the username for the account.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Gets the password for the account.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Returns true if this account matches an account on the system.
		/// </summary>
		public bool IsValid
		{
			get
			{
#if SIMPLSHARP
				return false;
#else
				return Username != null &&
				       LogonUtils.GetUsernames().Any(n => n.Equals(Username, StringComparison.OrdinalIgnoreCase));
#endif
			}
		}

		/// <summary>
		/// Returns true if this account matches the current logged in user.
		/// </summary>
		public bool IsCurrent
		{
			get
			{
#if SIMPLSHARP
				return false;
#else
				string current = LogonUtils.GetCurrentUsername();
				return Username != null && Username.Equals(current, StringComparison.OrdinalIgnoreCase);
#endif
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reverts the settings to defaults.
		/// </summary>
		public void Clear()
		{
			Name = null;
			Username = null;
			Password = null;
		}

		#endregion

		#region Serialization

		/// <summary>
		/// Copies the values from the other instance.
		/// </summary>
		/// <param name="other"></param>
		public void Copy([NotNull] UserAccount other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			Name = other.Name;
			Username = other.Username;
			Password = other.Password;
		}

		/// <summary>
		/// Updates the settings from the given xml element.
		/// </summary>
		/// <param name="xml"></param>
		public void ParseXml(string xml)
		{
			Name = XmlUtils.TryReadChildElementContentAsString(xml, ELEMENT_NAME);
			Username = XmlUtils.TryReadChildElementContentAsString(xml, ELEMENT_USERNAME);
			Password = XmlUtils.TryReadChildElementContentAsString(xml, ELEMENT_PASSWORD);
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

			writer.WriteStartElement(element);
			{
				writer.WriteElementString(ELEMENT_NAME, Name);
				writer.WriteElementString(ELEMENT_USERNAME, Username);
				writer.WriteElementString(ELEMENT_PASSWORD, Password);
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}