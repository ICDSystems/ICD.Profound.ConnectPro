#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.About;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.About;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.About
{
	[PresenterBinding(typeof(IReferencedSettingsPluginsPresenter))]
	public sealed class ReferencedSettingsPluginsPresenter : AbstractUiComponentPresenter<IReferencedSettingsPluginsView>,
	                                                         IReferencedSettingsPluginsPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private Assembly m_Assembly;

		/// <summary>
		/// Gets/sets the wrapped assembly.
		/// </summary>
		public Assembly Assembly
		{
			get { return m_Assembly; }
			set
			{
				if (Equals(value, m_Assembly))
					return;

				m_Assembly = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ReferencedSettingsPluginsPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IReferencedSettingsPluginsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string name = Assembly == null ? string.Empty : Assembly.GetName().Name;
				string version = Assembly == null ? string.Empty : Assembly.GetName().Version.ToString();
				string date = Assembly == null ? string.Empty : Assembly.GetCreationTime().ToString();

				view.SetPluginLabel(name);
				view.SetVersionLabel(version);
				view.SetDateLabel(date);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}
	}
}
