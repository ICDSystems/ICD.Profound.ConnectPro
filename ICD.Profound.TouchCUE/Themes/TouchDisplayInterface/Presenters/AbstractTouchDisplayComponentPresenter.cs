using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters
{
	/// <summary>
	///     A presenter that represents a single item in a list.
	///     Components are often recycled between use so we don't want to automatically take views from the factory.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractTouchDisplayComponentPresenter<T> : AbstractTouchDisplayPresenter<T>, IComponentPresenter
		where T : class, ITouchDisplayView
	{
		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractTouchDisplayComponentPresenter(INavigationController nav, IViewFactory views,
			TouchCueTheme theme) : base(nav, views, theme)
		{
		}
	}
}