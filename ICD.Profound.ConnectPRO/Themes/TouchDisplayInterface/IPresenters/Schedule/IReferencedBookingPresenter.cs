using ICD.Connect.Calendaring.Booking;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Schedule
{
	public interface IReferencedBookingPresenter : ITouchDisplayPresenter<IReferencedBookingView>
	{
		IBooking Booking { get; set; }
	}
}