using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups
{
	public sealed class CableTvPresenter : AbstractPopupPresenter<ICableTvView>, ICableTvPresenter
	{
		/// <summary>
		/// Gets/sets the tv tuner control that this preseter controls.
		/// </summary>
		public ITvTunerControl Control { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CableTvPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}
	}
}
