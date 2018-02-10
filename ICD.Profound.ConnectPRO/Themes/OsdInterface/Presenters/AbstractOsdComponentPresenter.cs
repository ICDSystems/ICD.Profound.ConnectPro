using System;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	/// <summary>
	/// A presenter that represents a single item in a list.
	/// 
	/// Components are often recycled between use so we don't want to automatically take views from the factory.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractOsdComponentPresenter<T> : AbstractOsdPresenter<T>
		where T : class, IOsdView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractOsdComponentPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override sealed void Refresh()
		{
			T view = GetView(false);

			// Don't refresh if we currently have no view.
			if (view != null)
				Refresh(view);
		}

		/// <summary>
		/// Sets the view.
		/// </summary>
		/// <param name="view"></param>
		public override sealed void SetView(T view)
		{
			if (view == GetView(false))
				return;

			// Special case - Can't set an SRL count to 0 (yay) so hide when cleaning up items.
			if (view == null && GetView(false) != null)
				ShowView(false);

			base.SetView(view);
		}

		/// <summary>
		/// Override to control how views are instantiated.
		/// </summary>
		/// <returns></returns>
		protected override T InstantiateView()
		{
			throw new InvalidOperationException(string.Format("{0} can not create its own view.", GetType().Name));
		}
	}
}
