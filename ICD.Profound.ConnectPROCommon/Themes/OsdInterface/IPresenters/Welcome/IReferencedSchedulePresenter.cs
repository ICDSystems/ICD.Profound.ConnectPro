using ICD.Connect.Calendaring.Bookings;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Welcome;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Welcome
{
	public interface IReferencedSchedulePresenter : IOsdPresenter<IReferencedScheduleView>
	{
		IBooking Booking { get; set; }
	}
}