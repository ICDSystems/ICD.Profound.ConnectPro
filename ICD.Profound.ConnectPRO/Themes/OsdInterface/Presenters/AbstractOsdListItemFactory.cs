using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
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
	public abstract class AbstractOsdListItemFactory<TModel, TPresenter, TView> : IEnumerable<TPresenter>, IDisposable
		where TPresenter : class, IOsdPresenter
	{
		private readonly Dictionary<Type, Queue<TPresenter>> m_PresenterPool;

		private readonly List<TPresenter> m_Presenters;
		private readonly List<TModel> m_Models;

		private readonly SafeCriticalSection m_CacheSection;

		private readonly IOsdNavigationController m_NavigationController;
		private readonly ListItemFactory<TView> m_ViewFactory;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		protected AbstractOsdListItemFactory(IOsdNavigationController navigationController, ListItemFactory<TView> viewFactory)
		{
			m_PresenterPool = new Dictionary<Type, Queue<TPresenter>>();

			m_Presenters = new List<TPresenter>();
			m_Models = new List<TModel>();

			m_CacheSection = new SafeCriticalSection();

			m_NavigationController = navigationController;
			m_ViewFactory = viewFactory;
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			foreach (TPresenter presenter in m_PresenterPool.SelectMany(kvp => kvp.Value))
				presenter.Dispose();
			m_PresenterPool.Clear();

			foreach (TPresenter presenter in m_Presenters)
				presenter.Dispose();
			m_Presenters.Clear();

			m_Models.Clear();
		}

		/// <summary>
		/// Generates the presenters and views for the given sequence of models.
		/// </summary>
		/// <param name="models"></param>
		/// <param name="subscribe">Called for each new presenter taken from the pool.</param>
		/// <param name="unsubscribe">Called for each presenter put back into the pool.</param>
		[PublicAPI]
		public IEnumerable<TPresenter> BuildChildren(IEnumerable<TModel> models, Action<TPresenter> subscribe, Action<TPresenter> unsubscribe)
		{
			if (models == null)
				throw new ArgumentNullException("models");

			if (subscribe == null)
				throw new ArgumentNullException("subscribeAction");

			if (unsubscribe == null)
				throw new ArgumentNullException("unsubscribeAction");

			m_CacheSection.Enter();

			try
			{
				// Update the model cache to match the given models
				if (!UpdateModelCache(models))
					return m_Presenters.ToArray(m_Presenters.Count);

				// Build the views (may be fewer than models due to list max size)
				IEnumerable<TView> views = m_ViewFactory((ushort)m_Models.Count);
				IList<TView> viewsArray = views as IList<TView> ?? views.ToArray();

				// Return presenters that are outside of the current view count back to the pool
				ReturnPresentersToPool(viewsArray.Count, unsubscribe);

				// Build the presenters
				for (int index = 0; index < viewsArray.Count; index++)
				{
					TView view = viewsArray[index];

					// Get the model
					TModel model = m_Models[index];

					// Get the presenter
					Type key = GetPresenterTypeForModel(model);
					TPresenter presenter = LazyLoadPresenterFromPool(key, index, subscribe, unsubscribe);

					// Bind
					BindMvpTriad(model, presenter, view);
				}

				return m_Presenters.ToArray(m_Presenters.Count);
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Convenience method for updating the room on all of the instantiated presenters.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			m_CacheSection.Enter();

			try
			{
				foreach (TPresenter presenter in m_PresenterPool.SelectMany(kvp => kvp.Value))
					presenter.SetRoom(room);

				foreach (TPresenter presenter in m_Presenters)
					presenter.SetRoom(room);
			}
			finally
			{
				m_CacheSection.Leave();
			}
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

		private bool UpdateModelCache(IEnumerable<TModel> models)
		{
			if (models == null)
				throw new ArgumentNullException("models");

			m_CacheSection.Enter();

			try
			{
				// Gather all of the models
				IList<TModel> modelsArray = models as IList<TModel> ?? models.ToArray();

				// Check if anything has actually changed
				if (modelsArray.SequenceEqual(m_Models))
					return false;

				m_Models.Clear();
				m_Models.AddRange(modelsArray);

				return true;
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Retrieves or generates a presenter from the pool and adds it to the cache at the given index.
		/// </summary>
		/// <param name="presenterType"></param>
		/// <param name="index"></param>
		/// <param name="subscribe">Called for each item put into the presenter cache.</param>
		/// <param name="unsubscribe">Called for each item put into the pool.</param>
		/// <returns></returns>
		private TPresenter LazyLoadPresenterFromPool(Type presenterType, int index, Action<TPresenter> subscribe, Action<TPresenter> unsubscribe)
		{
			if (presenterType == null)
				throw new ArgumentNullException("presenterType");

			if (index < 0)
				throw new ArgumentOutOfRangeException("index");

			if (subscribe == null)
				throw new ArgumentNullException("subscribe");

			if (unsubscribe == null)
				throw new ArgumentNullException("unsubscribe");

			m_CacheSection.Enter();

			try
			{
				// Does the cache already contain the correct presenter type at the given index?
				TPresenter existing = m_Presenters.Count > index ? m_Presenters[index] : null;
				if (existing != null && existing.GetType().IsAssignableTo(presenterType))
					return existing;

				// Return the existing presenter of the wrong type to the pool
				if (existing != null)
					ReturnPresenterToPool(index, unsubscribe);

				// Get the pool for the given type
				Queue<TPresenter> pool;
				if (!m_PresenterPool.TryGetValue(presenterType, out pool))
				{
					pool = new Queue<TPresenter>();
					m_PresenterPool[presenterType] = pool;
				}

				// Create a new presenter if the pool is empty
				TPresenter presenter =
					pool.Dequeue(out presenter)
						? presenter
						: m_NavigationController.GetNewPresenter(presenterType) as TPresenter;

				// Insert the presenter into the cache
				m_Presenters.Insert(index, presenter);

				// Call the subscription action
				subscribe(presenter);

				return presenter;
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Loops over the presenters starting at the given index, clears the views and moves the
		/// presenters from the cache to the pool.
		/// </summary>
		private void ReturnPresentersToPool(int startIndex, Action<TPresenter> unsubscribe)
		{
			if (startIndex < 0)
				throw new ArgumentOutOfRangeException("startIndex");

			if (unsubscribe == null)
				throw new ArgumentNullException("unsubscribeAction");

			m_CacheSection.Enter();

			try
			{
				for (int index = m_Presenters.Count - 1; index >= startIndex; index--)
					ReturnPresenterToPool(index, unsubscribe);
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Removes the presenter at the given index and puts it back into the pool.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="unsubscribe"></param>
		private void ReturnPresenterToPool(int index, Action<TPresenter> unsubscribe)
		{
			if (unsubscribe == null)
				throw new ArgumentNullException("unsubscribe");

			m_CacheSection.Enter();

			try
			{
				if (index < 0 || index >= m_Presenters.Count)
					throw new ArgumentOutOfRangeException("index");

				TPresenter presenter = m_Presenters[index];

				ReturnPresenterToPool(presenter, unsubscribe);

				m_Presenters.RemoveAt(index);
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Puts the presenter back into the pool.
		/// </summary>
		/// <param name="presenter"></param>
		/// <param name="unsubscribe"></param>
		private void ReturnPresenterToPool(TPresenter presenter, Action<TPresenter> unsubscribe)
		{
			if (presenter == null)
				throw new ArgumentNullException("presenter");

			if (unsubscribe == null)
				throw new ArgumentNullException("unsubscribe");

			unsubscribe(presenter);
			presenter.ClearView();

			Type key = presenter.GetType();

			m_CacheSection.Enter();

			try
			{
				Queue<TPresenter> pool;
				if (!m_PresenterPool.TryGetValue(key, out pool))
				{
					pool = new Queue<TPresenter>();
					m_PresenterPool[key] = pool;
				}

				pool.Enqueue(presenter);
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		#endregion

		public IEnumerator<TPresenter> GetEnumerator()
		{
			return m_CacheSection.Execute(() => m_Presenters.ToList()).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
