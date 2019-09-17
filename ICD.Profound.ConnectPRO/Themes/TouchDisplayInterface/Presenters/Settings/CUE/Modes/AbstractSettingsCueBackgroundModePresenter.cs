using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Settings.CUE.Modes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings.CUE.Modes;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Settings.CUE.Modes
{
	public abstract class AbstractSettingsCueBackgroundModePresenter<TView> : AbstractTouchDisplayPresenter<TView>,
	                                                                          ISettingsCueBackgroundModePresenter<TView>
		where TView : class, ISettingsCueBackgroundModeView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractSettingsCueBackgroundModePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}
	}
}
