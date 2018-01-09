using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	/// <summary>
	/// Generates the given number of views.
	/// </summary>
	/// <typeparam name="TView"></typeparam>
	/// <param name="count"></param>
	/// <returns></returns>
	public delegate IEnumerable<TView> ListItemFactory<TView>(ushort count);

	/// <summary>
	/// Base class for list item factories.
	/// Takes a sequence of model items and generates the views and presenters, using a callback
	/// to bind the MVP triad.
	/// </summary>
	public abstract class AbstractListItemFactory<TModel, TPresenter, TView> : IEnumerable<TPresenter>, IDisposable
		where TPresenter : class, IPresenter
	{
		private readonly Dictionary<int, TPresenter> m_PresenterCache;
		private readonly SafeCriticalSection m_CacheSection;
		private readonly SafeCriticalSection m_BuildViewsSection;

		private readonly INavigationController m_NavigationController;
		private readonly ListItemFactory<TView> m_ViewFactory;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		protected AbstractListItemFactory(INavigationController navigationController, ListItemFactory<TView> viewFactory)
		{
			m_PresenterCache = new Dictionary<int, TPresenter>();
			m_CacheSection = new SafeCriticalSection();
			m_BuildViewsSection = new SafeCriticalSection();

			m_NavigationController = navigationController;
			m_ViewFactory = viewFactory;
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			foreach (TPresenter presenter in m_PresenterCache.Values)
				presenter.Dispose();
			m_PresenterCache.Clear();
		}

		/// <summary>
		/// Generates the presenters and views for the given sequence of models.
		/// </summary>
		/// <param name="models"></param>
		[PublicAPI]
		public TPresenter[] BuildChildren(IEnumerable<TModel> models)
		{
			List<TPresenter> output = new List<TPresenter>();

			m_BuildViewsSection.Enter();

			try
			{
				// Gather all of the models
				TModel[] modelsArray = models as TModel[] ?? models.ToArray();

				// Clear the existing views
				ClearChildViews(modelsArray.Length);

				// Build the views (may be fewer than models due to list max size)
				TView[] views = m_ViewFactory((ushort)modelsArray.Length).ToArray();

				// Build the presenters
				for (int index = 0; index < views.Length; index++)
				{
					// Get the view
					TView view = views[index];

					// Get the model
					TModel model = modelsArray[index];
					Type key = GetPresenterTypeForModel(model);

					// Get the presenter
					TPresenter presenter = LazyLoadPresenterFromCache(key, index);

					// Bind
					BindMvpTriad(model, presenter, view);

					output.Add(presenter);
				}
			}
			finally
			{
				m_BuildViewsSection.Leave();
			}

			return output.ToArray();
		}

		/// <summary>
		/// Convenience method for updating the room on all of the instantiated presenters.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			foreach (TPresenter presenter in GetPresenters())
				presenter.SetRoom(room);
		}

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected abstract void BindMvpTriad(TModel model, TPresenter presenter, TView view);

		/// <summary>
		/// Gets the presenter type for the given model instance.
		/// Override to fill lists with different presenters.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[PublicAPI]
		protected virtual Type GetPresenterTypeForModel(TModel model)
		{
			return typeof(TPresenter);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Loops over the child presenters by index and sets the views to null.
		/// </summary>
		private void ClearChildViews(int startIndex)
		{
			m_CacheSection.Enter();

			try
			{
				m_PresenterCache.Where(kvp => kvp.Key >= startIndex)
				                .Select(kvp => kvp.Value)
				                .ForEach(p => p.ClearView());
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Gets the instantiated presenters.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<TPresenter> GetPresenters()
		{
			return m_CacheSection.Execute(() => m_PresenterCache.OrderValuesByKey().ToList());
		}

		/// <summary>
		/// Retrieves or generates a presenter from the cache.
		/// </summary>
		/// <param name="presenterType"></param>
		/// <param name="cacheIndex"></param>
		/// <returns></returns>
		private TPresenter LazyLoadPresenterFromCache(Type presenterType, int cacheIndex)
		{
			m_CacheSection.Enter();

			try
			{
				// Dispose the existing presenter if it does not match the requested type.
				TPresenter existing = m_PresenterCache.GetDefault(cacheIndex, null);
				if (existing != null && !existing.GetType().IsAssignableTo(presenterType))
				{
					existing.ClearView();
					existing.Dispose();
					existing = null;
				}

				// Create a new presenter if one doesn't already exist.
				if (existing == null)
					m_PresenterCache[cacheIndex] = m_NavigationController.GetNewPresenter(presenterType) as TPresenter;

				// Return the existing presenter
				return m_PresenterCache[cacheIndex];
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		#endregion

		public IEnumerator<TPresenter> GetEnumerator()
		{
			return GetPresenters().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
