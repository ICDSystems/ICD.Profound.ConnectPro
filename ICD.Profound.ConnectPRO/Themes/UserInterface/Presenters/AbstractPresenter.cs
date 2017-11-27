using System;
using ICD.Common.Properties;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	/// <summary>
	/// Base class for all presenters.
	/// </summary>
	public abstract class AbstractPresenter<T> : IPresenter<T>
		where T : class, IView
	{
		public event EventHandler<BoolEventArgs> OnViewVisibilityChanged;

		private readonly INavigationController m_Navigation;
		private readonly IViewFactory m_ViewFactory;
		private readonly ConnectProTheme m_Theme;

		private T m_View;

		[UsedImplicitly] private object m_AsyncRefreshHandle;

		#region Properties

		/// <summary>
		/// Gets the navigation controller.
		/// </summary>
		protected INavigationController Navigation { get { return m_Navigation; } }

		/// <summary>
		/// Gets the room.
		/// </summary>
		[CanBeNull]
		public IRoom Room { get; private set; }

		/// <summary>
		/// Gets the view factory.
		/// </summary>
		protected IViewFactory ViewFactory { get { return m_ViewFactory; } }

		/// <summary>
		/// Gets the state of the view visibility.
		/// </summary>
		public bool IsViewVisible { get { return m_View != null && m_View.IsVisible; } }

		/// <summary>
		/// Gets the parent theme.
		/// </summary>
		protected ConnectProTheme Theme { get { return m_Theme; } }

		protected ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
		{
			if (nav == null)
				throw new ArgumentNullException("nav");

			if (views == null)
				throw new ArgumentNullException("views");

			if (theme == null)
				throw new ArgumentNullException("theme");

			m_Navigation = nav;
			m_ViewFactory = views;
			m_Theme = theme;
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			OnViewVisibilityChanged = null;
			m_AsyncRefreshHandle = null;

			SetRoom(null);

			if (m_View == null)
				return;

			Unsubscribe(m_View);
			m_View.Dispose();
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public virtual void SetRoom(IRoom room)
		{
			if (room == Room)
				return;

			Unsubscribe(Room);
			Room = room;
			Subscribe(Room);

			RefreshIfVisible();
		}

		/// <summary>
		/// Gets the view for this presenter.
		/// </summary>
		/// <returns></returns>
		protected T GetView()
		{
			return GetView(true);
		}

		/// <summary>
		/// Gets the view for the presenter.
		/// </summary>
		/// <param name="instantiate">When true instantiates a new view if the current view is null.</param>
		/// <returns></returns>
		protected T GetView(bool instantiate)
		{
			// Get default view from the factory
			if (m_View == null && instantiate)
			{
				T view = InstantiateView();
				SetView(view);
			}

			return m_View;
		}

		/// <summary>
		/// Override to control how views are instantiated.
		/// </summary>
		/// <returns></returns>
		protected virtual T InstantiateView()
		{
			return m_ViewFactory.GetNewView<T>();
		}

		/// <summary>
		/// Sets the current view to null.
		/// </summary>
		public void ClearView()
		{
			SetView(null);
		}

		/// <summary>
		/// Disposes the old view and sets the new view.
		/// </summary>
		/// <param name="view"></param>
		public virtual void SetView(T view)
		{
			if (view == m_View)
				return;

			if (m_View != null)
				Unsubscribe(m_View);

			m_View = view;

			if (m_View != null)
				Subscribe(m_View);

			RefreshIfVisible();
		}

		/// <summary>
		/// Sets the visibility of the presenters view.
		/// </summary>
		/// <param name="visible"></param>
		public void ShowView(bool visible)
		{
			// Don't bother creating the view just to hide it.
			if (m_View == null && !visible)
				return;

			GetView().Show(visible);
		}

		/// <summary>
		/// Sets the enabled state of the view.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetViewEnabled(bool enabled)
		{
			GetView().Enable(enabled);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		[PublicAPI]
		public virtual void Refresh()
		{
			T view = GetView(true);

			// Don't refresh if we currently have no view.
			if (view != null)
				Refresh(view);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected virtual void Refresh(T view)
		{
		}

		/// <summary>
		/// Refreshes the view asynchronously.
		/// </summary>
		[PublicAPI]
		public void RefreshAsync()
		{
			m_AsyncRefreshHandle = ThreadingUtils.SafeInvoke(Refresh);
		}

		/// <summary>
		/// Asynchronously updates the view if it is currently visible.
		/// </summary>
		[PublicAPI]
		public void RefreshIfVisible()
		{
			RefreshIfVisible(true);
		}

		/// <summary>
		/// Updates the view if it is currently visible.
		/// </summary>
		/// <param name="async"></param>
		[PublicAPI]
		public void RefreshIfVisible(bool async)
		{
			if (!IsViewVisible)
				return;

			if (async)
				RefreshAsync();
			else
				Refresh();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected virtual void Subscribe(T view)
		{
			view.OnVisibilityChanged += ViewOnVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected virtual void Unsubscribe(T view)
		{
			view.OnVisibilityChanged -= ViewOnVisibilityChanged;
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected virtual void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			RefreshIfVisible();

			OnViewVisibilityChanged.Raise(this, new BoolEventArgs(args.Data));
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Subscribe(IRoom room)
		{
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Unsubscribe(IRoom room)
		{
		}

		#endregion
	}
}
