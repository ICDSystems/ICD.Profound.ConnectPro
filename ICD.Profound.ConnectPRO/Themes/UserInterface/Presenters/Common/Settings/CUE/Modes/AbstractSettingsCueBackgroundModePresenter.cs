using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.CUE.Modes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.CUE.Modes;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.CUE.Modes
{
	public abstract class AbstractSettingsCueBackgroundModePresenter<TView> : AbstractUiPresenter<TView>,
	                                                                          ISettingsCueBackgroundModePresenter<TView>
		where TView : class, ISettingsCueBackgroundModeView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractSettingsCueBackgroundModePresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                                     IConnectProTheme theme)
			: base(nav, views, theme)
		{
		}
	}
}
