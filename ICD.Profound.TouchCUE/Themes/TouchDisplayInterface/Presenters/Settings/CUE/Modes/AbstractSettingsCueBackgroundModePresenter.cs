using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings.CUE.Modes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.CUE.Modes;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Settings.CUE.Modes
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
		protected AbstractSettingsCueBackgroundModePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
		}
	}
}
