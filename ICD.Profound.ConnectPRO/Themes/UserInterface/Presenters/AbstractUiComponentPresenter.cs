using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	/// <summary>
	/// A presenter that represents a single item in a list.
	/// 
	/// Components are often recycled between use so we don't want to automatically take views from the factory.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractUiComponentPresenter<T> : AbstractUiPresenter<T>, IComponentPresenter
		where T : class, IUiView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractUiComponentPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
		}
	}
}
