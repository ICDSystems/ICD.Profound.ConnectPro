using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Zoom.SubSettings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom.SubSettings;
using ICD.Profound.ConnectPROCommon.SettingsTree.Zoom;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Zoom.SubSettings
{
	public class AbstractSettingsZoomSubPresenter<TView> : AbstractUiPresenter<TView>, ISettingsZoomSubPresenter<TView>
		where TView : class, ISettingsZoomSubView
	{
		private ZoomSettingsLeaf m_Settings;

		/// <summary>
		/// Gets/sets the wrapped zoom settings leaf instance.
		/// </summary>
		public ZoomSettingsLeaf Settings
		{
			get { return m_Settings; }
			set
			{
				if (value == m_Settings)
					return;

				Unsubscribe(m_Settings);
				m_Settings = value;
				Subscribe(m_Settings);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public AbstractSettingsZoomSubPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Subscribe to the settings events.
		/// </summary>
		/// <param name="settings"></param>
		protected virtual void Subscribe(ZoomSettingsLeaf settings)
		{
		}

		/// <summary>
		/// Unsubscribe from the settings events.
		/// </summary>
		/// <param name="settings"></param>
		protected virtual void Unsubscribe(ZoomSettingsLeaf settings)
		{
		}
	}
}
