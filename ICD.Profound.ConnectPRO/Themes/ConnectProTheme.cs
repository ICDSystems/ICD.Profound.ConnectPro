using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Profound.ConnectPRO.Themes.UserInterface;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes
{
	public sealed class ConnectProTheme : AbstractConnectProTheme<ConnectProThemeSettings>
	{
		private readonly IcdHashSet<IUserInterfaceFactory> m_UiFactories;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProTheme()
		{
			m_UiFactories = new IcdHashSet<IUserInterfaceFactory>
			{
				new ConnectProUserInterfaceFactory(this)
			};
		}

		/// <summary>
		/// Gets the UI Factories.
		/// </summary>
		public override IEnumerable<IUserInterfaceFactory> GetUiFactories()
		{
			return m_UiFactories.Concat(base.GetUiFactories());
		}
	}
}
