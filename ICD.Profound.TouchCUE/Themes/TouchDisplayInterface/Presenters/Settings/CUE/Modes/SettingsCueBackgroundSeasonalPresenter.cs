using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings.CUE.Modes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.CUE.Modes;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Settings.CUE.Modes
{
	[PresenterBinding(typeof(ISettingsCueBackgroundSeasonalPresenter))]
	public sealed class SettingsCueBackgroundSeasonalPresenter :
		AbstractSettingsCueBackgroundModePresenter<ISettingsCueBackgroundSeasonalView>,
		ISettingsCueBackgroundSeasonalPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsCueBackgroundSeasonalPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
		}
	}
}
