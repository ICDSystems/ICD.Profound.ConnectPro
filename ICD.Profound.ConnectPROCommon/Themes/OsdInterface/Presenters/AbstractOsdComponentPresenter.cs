using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters
{
	/// <summary>
	/// A presenter that represents a single item in a list.
	/// 
	/// Components are often recycled between use so we don't want to automatically take views from the factory.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractOsdComponentPresenter<T> : AbstractOsdPresenter<T>, IComponentPresenter
		where T : class, IOsdView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractOsdComponentPresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
		}
	}
}
