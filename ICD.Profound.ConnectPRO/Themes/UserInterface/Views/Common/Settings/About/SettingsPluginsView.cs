using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.About;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.About
{
	[ViewBinding(typeof(ISettingsPluginsView))]
	public sealed partial class SettingsPluginsView : AbstractUiView, ISettingsPluginsView
	{
		private readonly List<IReferencedSettingsPluginsView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsPluginsView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedSettingsPluginsView>();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IReferencedSettingsPluginsView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_PluginList, m_ChildList, count);
		}
	}
}
