using ICD.Connect.Calendaring.Bookings;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Bodies;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Bodies
{
	public interface IReferencedSchedulePresenter : IOsdPresenter<IReferencedScheduleView>
	{
		IBooking Booking { get; set; }
	}
}