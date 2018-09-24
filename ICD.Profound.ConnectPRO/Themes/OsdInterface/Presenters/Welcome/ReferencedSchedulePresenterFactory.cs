using ICD.Connect.Calendaring.Booking;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Welcome;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Welcome;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Welcome
{
	public sealed class ReferencedSchedulePresenterFactory :
		AbstractOsdListItemFactory<IBooking, IReferencedSchedulePresenter, IReferencedScheduleView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public ReferencedSchedulePresenterFactory(IOsdNavigationController navigationController,
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
