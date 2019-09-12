using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters
{
    /// <summary>
    ///     A presenter that represents a single item in a list.
    ///     Components are often recycled between use so we don't want to automatically take views from the factory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractTouchDisplayComponentPresenter<T> : AbstractTouchDisplayPresenter<T>
        where T : class, ITouchDisplayView
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="views"></param>
        /// <param name="theme"></param>
        protected AbstractTouchDisplayComponentPresenter(INavigationController nav, IViewFactory views,
            ConnectProTheme theme) : base(nav, views, theme)
        {
        }

        /// <summary>
        ///     Returns true if this presenter is part of a collection of components.
        /// </summary>
        public override bool IsComponent => true;
    }
}