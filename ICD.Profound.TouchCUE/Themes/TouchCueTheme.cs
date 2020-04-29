using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Profound.ConnectPROCommon.Themes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface;

namespace ICD.Profound.TouchCUE.Themes
{
	public sealed class TouchCueTheme : AbstractConnectProTheme<TouchCueThemeSettings>
	{
		private readonly IcdHashSet<IUserInterfaceFactory> m_UiFactories;
		private readonly SafeCriticalSection m_UiFactoriesSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		public TouchCueTheme()
		{
			m_UiFactories = new IcdHashSet<IUserInterfaceFactory>
			{
				new ConnectProTouchDisplayInterfaceFactory(this)
			};

			m_UiFactoriesSection = new SafeCriticalSection();
		}
		

		#region Public Methods

		/// <summary>
		/// Gets the UI Factories.
		/// </summary>
		public override IEnumerable<IUserInterfaceFactory> GetUiFactories()
		{
			return m_UiFactoriesSection.Execute(() => m_UiFactories.ToArray())
			                           .Concat(base.GetUiFactories());
		}

		/// <summary>
		/// Clears the instantiated user interfaces.
		/// </summary>
		public override void ClearUserInterfaces()
		{
			base.ClearUserInterfaces();

			m_UiFactoriesSection.Execute(() => m_UiFactories.ForEach(f => f.Clear()));
		}

		/// <summary>
		/// Clears and rebuilds the user interfaces.
		/// </summary>
		public override void BuildUserInterfaces()
		{
			base.BuildUserInterfaces();

			m_UiFactoriesSection.Execute(() => m_UiFactories.ForEach(f => f.BuildUserInterfaces()));
		}

		#endregion
	}
}
