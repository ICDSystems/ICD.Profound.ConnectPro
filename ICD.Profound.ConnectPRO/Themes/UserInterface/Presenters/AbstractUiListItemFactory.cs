using System;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	/// <summary>
	/// Base class for list item factories.
	/// Takes a sequence of model items and generates the views and presenters, using a callback
	/// to bind the MVP triad.
	/// </summary>
	public abstract class AbstractUiListItemFactory<TModel, TPresenter, TView> : AbstractListItemFactory<TModel, TPresenter, TView>
		where TPresenter : class, IUiPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		protected AbstractUiListItemFactory(IConnectProNavigationController navigationController,
		                                    ListItemFactory<TView> viewFactory, Action<TPresenter> subscribe,
		                                    Action<TPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Convenience method for updating the room on all of the instantiated presenters.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			foreach (TPresenter presenter in GetAllPresenters())
				presenter.SetRoom(room);
		}
	}
}
