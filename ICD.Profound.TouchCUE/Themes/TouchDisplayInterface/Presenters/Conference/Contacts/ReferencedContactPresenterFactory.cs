using System;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Contacts;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference.Contacts
{
	public sealed class ReferencedContactPresenterFactory :
		AbstractTouchDisplayListItemFactory<IContact, IReferencedContactPresenter, IReferencedContactView>
	{
		public ReferencedContactPresenterFactory(ITouchDisplayNavigationController navigationController,
		                                            ListItemFactory<IReferencedContactView> viewFactory,
		                                            Action<IReferencedContactPresenter> subscribe,
		                                            Action<IReferencedContactPresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		protected override void BindMvpTriad(IContact model, IReferencedContactPresenter presenter,
		                                     IReferencedContactView view)
		{
			presenter.Contact = model;
			presenter.SetView(view);
		}
	}
}
