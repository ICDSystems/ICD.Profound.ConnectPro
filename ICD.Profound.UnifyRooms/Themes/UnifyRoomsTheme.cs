using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Connect.Settings;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Profound.ConnectPRO.Themes.UserInterface;
using ICD.Profound.ConnectPROCommon.Themes;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;
using ICD.Profound.UnifyRooms.MoreControls;
using ICD.Profound.UnifyRooms.Themes.UnifyBar;
using ICD.Profound.UnifyRooms.UserAccounts;

namespace ICD.Profound.UnifyRooms.Themes
{
	public sealed class UnifyRoomsTheme : AbstractConnectProTheme<UnifyRoomsThemeSettings>
	{
		private readonly MoreControlsPanelConfiguration m_MoreControlsPanel;
		private readonly UnifyBarButtonsConfiguration m_UnifyBarButtons;
		private readonly UserAccountsConfiguration m_UserAccounts;

		private readonly IcdHashSet<IUserInterfaceFactory> m_UiFactories;

		#region Properties

		/// <summary>
		/// Gets the more controls panel configuration.
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
		public UnifyRoomsTheme()
		{
			m_MoreControlsPanel = new MoreControlsPanelConfiguration();
			m_UnifyBarButtons = new UnifyBarButtonsConfiguration();
			m_UserAccounts = new UserAccountsConfiguration();

			m_UiFactories = new IcdHashSet<IUserInterfaceFactory>
			{
				new UnifyBarUserInterfaceFactory(this),

				// TODO - Replace with web panel
				new ConnectProUserInterfaceFactory(this)
			};
		}

		#region Methods

		/// <summary>
		/// Gets the UI Factories.
		/// </summary>
		public override IEnumerable<IUserInterfaceFactory> GetUiFactories()
		{
			return m_UiFactories.Concat(base.GetUiFactories());
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(UnifyRoomsThemeSettings settings, IDeviceFactory factory)
		{
			m_MoreControlsPanel.Copy(settings.MoreControlsPanel);
			m_UnifyBarButtons.Copy(settings.UnifyBarButtons);
			m_UserAccounts.Copy(settings.UserAccounts);

			base.ApplySettingsFinal(settings, factory);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_MoreControlsPanel.Clear();
			m_UnifyBarButtons.Clear();
			m_UserAccounts.Clear();
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(UnifyRoomsThemeSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.MoreControlsPanel.Copy(m_MoreControlsPanel);
			settings.UnifyBarButtons.Copy(m_UnifyBarButtons);
			settings.UserAccounts.Copy(m_UserAccounts);
		}

		#endregion
	}
}
