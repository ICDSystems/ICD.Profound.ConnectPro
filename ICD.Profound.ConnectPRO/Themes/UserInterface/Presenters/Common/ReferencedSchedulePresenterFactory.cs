using ICD.Connect.Calendaring.Booking;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class ReferencedSchedulePresenterFactory :
		AbstractListItemFactory<IBooking, IReferencedSchedulePresenter, IReferencedScheduleView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public ReferencedSchedulePresenterFactory(IConnectProNavigationController navigationController,
		                                              ListItemFactory<IReferencedScheduleView> viewFactory)
			: base(navigationController, viewFactory)
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
