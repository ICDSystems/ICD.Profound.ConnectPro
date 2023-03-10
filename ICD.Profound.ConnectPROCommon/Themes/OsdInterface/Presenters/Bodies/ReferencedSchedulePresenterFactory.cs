using System;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Bodies;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Bodies;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Bodies
{
	public sealed class ReferencedSchedulePresenterFactory :
		AbstractOsdListItemFactory<IBooking, IReferencedSchedulePresenter, IReferencedScheduleView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public ReferencedSchedulePresenterFactory(IOsdNavigationController navigationController,
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
