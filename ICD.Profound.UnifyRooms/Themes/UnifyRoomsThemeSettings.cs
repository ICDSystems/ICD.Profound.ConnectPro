using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes;
using ICD.Profound.ConnectPROCommon.Themes;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;
using ICD.Profound.UnifyRooms.MoreControls;
using ICD.Profound.UnifyRooms.UserAccounts;

namespace ICD.Profound.UnifyRooms.Themes
{
	[KrangSettings("UnifyRoomsTheme", typeof(UnifyRoomsTheme))]
	public sealed class UnifyRoomsThemeSettings : AbstractConnectProThemeSettings
	{
		private const string ELEMENT_MORE_CONTROLS_PANEL = "MoreControlsPanel";
		private const string ELEMENT_UNIFY_BAR_BUTTONS = "UnifyBarButtons";
		private const string ELEMENT_USER_ACCOUNTS = "UserAccounts";

		private readonly MoreControlsPanelConfiguration m_MoreControlsPanel;
		private readonly UnifyBarButtonsConfiguration m_UnifyBarButtons;
		private readonly UserAccountsConfiguration m_UserAccounts;

		#region Properties

		/// <summary>
		/// Gets/sets the enabled state of the "more controls" panel.
		/// </summary>
		public MoreControlsPanelConfiguration MoreControlsPanel { get { return m_MoreControlsPanel; } }

		/// <summary>
		/// Gets the UnifyBar buttons configuration.
		/// </summary>
		public UnifyBarButtonsConfiguration UnifyBarButtons { get { return m_UnifyBarButtons; } }

		/// <summary>
		/// Gets the user accounts configuration.
		/// </summary>
		public UserAccountsConfiguration UserAccounts { get { return m_UserAccounts; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyRoomsThemeSettings()
		{
			m_MoreControlsPanel = new MoreControlsPanelConfiguration();
			m_UnifyBarButtons = new UnifyBarButtonsConfiguration();
			m_UserAccounts = new UserAccountsConfiguration();
		}

		#region Serialization

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			string moreControlsPanelXml;
			if (XmlUtils.TryGetChildElementAsString(xml, ELEMENT_MORE_CONTROLS_PANEL, out moreControlsPanelXml))
				m_MoreControlsPanel.ParseXml(moreControlsPanelXml);

			string unifyBarButtonsXml;
			if (XmlUtils.TryGetChildElementAsString(xml, ELEMENT_UNIFY_BAR_BUTTONS, out unifyBarButtonsXml))
				m_UnifyBarButtons.ParseXml(unifyBarButtonsXml);

			string userAccountsXml;
			if (XmlUtils.TryGetChildElementAsString(xml, ELEMENT_USER_ACCOUNTS, out userAccountsXml))
				m_UserAccounts.ParseXml(userAccountsXml);
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			m_MoreControlsPanel.ToXml(writer, ELEMENT_MORE_CONTROLS_PANEL);
			m_UnifyBarButtons.ToXml(writer, ELEMENT_UNIFY_BAR_BUTTONS);
			m_UserAccounts.ToXml(writer, ELEMENT_USER_ACCOUNTS);
		}

		#endregion
	}
}
