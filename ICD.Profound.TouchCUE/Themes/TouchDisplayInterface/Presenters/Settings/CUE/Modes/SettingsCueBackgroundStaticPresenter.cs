using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings.CUE.Modes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.CUE.Modes;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Settings.CUE.Modes
{
	[PresenterBinding(typeof(ISettingsCueBackgroundStaticPresenter))]
	public sealed class SettingsCueBackgroundStaticPresenter :
		AbstractSettingsCueBackgroundModePresenter<ISettingsCueBackgroundStaticView>, ISettingsCueBackgroundStaticPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsCueBackgroundStaticPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
		}
	}
}
