using System.Collections.Generic;
using System.Linq;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using ICD.Common.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.SettingsTree.About;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.About;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.About;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.About
{
	[PresenterBinding(typeof(ISettingsPluginsPresenter))]
	public sealed class SettingsPluginsPresenter :
		AbstractSettingsNodeBasePresenter<ISettingsPluginsView, PluginsSettingsLeaf>, ISettingsPluginsPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedSettingsPluginsPresenterFactory m_ChildrenFactory;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsPluginsPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedSettingsPluginsPresenterFactory(nav, ItemFactory);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsPluginsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<Assembly> pluginAssemblies =
					Node == null
						? Enumerable.Empty<Assembly>()
						: Node.GetPluginAssemblies();

				foreach (IReferencedSettingsPluginsPresenter child in m_ChildrenFactory.BuildChildren(pluginAssemblies))
					child.ShowView(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Generates the given number of views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IReferencedSettingsPluginsView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}
	}
}
