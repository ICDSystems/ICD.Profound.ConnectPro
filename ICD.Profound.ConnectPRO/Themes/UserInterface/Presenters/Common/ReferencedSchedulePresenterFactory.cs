using System;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class ReferencedSchedulePresenterFactory :
		AbstractUiListItemFactory<IBooking, IReferencedSchedulePresenter, IReferencedScheduleView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public ReferencedSchedulePresenterFactory(IConnectProNavigationController navigationController,
		                                          ListItemFactory<IReferencedScheduleView> viewFactory,
		                                          Action<IReferencedSchedulePresenter> subscribe,
		                                          Action<IReferencedSchedulePresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IBooking model, IReferencedSchedulePresenter presenter,
		                                     IReferencedScheduleView view)
		{
			presenter.Booking = model;
			presenter.SetView(view);
		}
	}
}
