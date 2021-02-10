using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Profound.ConnectPRO.Themes.UserInterface;
using ICD.Profound.ConnectPROCommon.Themes;
using ICD.Profound.UnifyRooms.Themes.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes
{
	public sealed class UnifyRoomsTheme : AbstractConnectProTheme<UnifyRoomsThemeSettings>
	{
		private readonly IcdHashSet<IUserInterfaceFactory> m_UiFactories;

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyRoomsTheme()
		{
			m_UiFactories = new IcdHashSet<IUserInterfaceFactory>
			{
				new UnifyBarUserInterfaceFactory(this),

				// TODO - Replace with web panel
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
