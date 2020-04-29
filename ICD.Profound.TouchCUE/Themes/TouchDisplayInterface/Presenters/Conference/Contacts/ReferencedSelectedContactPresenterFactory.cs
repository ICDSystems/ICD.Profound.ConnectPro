using System;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Contacts;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference.Contacts
{
	public sealed class ReferencedSelectedContactPresenterFactory :
		AbstractTouchDisplayListItemFactory<IContact, IReferencedSelectedContactPresenter, IReferencedSelectedContactView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public ReferencedSelectedContactPresenterFactory(ITouchDisplayNavigationController navigationController,
		                                                    ListItemFactory<IReferencedSelectedContactView> viewFactory,
		                                                    Action<IReferencedSelectedContactPresenter> subscribe,
		                                                    Action<IReferencedSelectedContactPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IContact model, IReferencedSelectedContactPresenter presenter,
		                                     IReferencedSelectedContactView view)
		{
			presenter.Contact = model;
			presenter.SetView(view);
		}
	}
}
