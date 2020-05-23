using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Profound.ConnectPROCommon.Themes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface;

namespace ICD.Profound.TouchCUE.Themes
{
	public sealed class TouchCueTheme : AbstractConnectProTheme<TouchCueThemeSettings>
	{
		private readonly IcdHashSet<IUserInterfaceFactory> m_UiFactories;

		/// <summary>
		/// Constructor.
		/// </summary>
		public TouchCueTheme()
		{
			m_UiFactories = new IcdHashSet<IUserInterfaceFactory>
			{
				new ConnectProTouchDisplayInterfaceFactory(this)
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
