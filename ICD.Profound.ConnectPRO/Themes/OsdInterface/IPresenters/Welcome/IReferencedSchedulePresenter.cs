using ICD.Connect.Calendaring.Booking;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Welcome;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Welcome
{
	public interface IReferencedSchedulePresenter : IOsdPresenter<IReferencedScheduleView>
	{
		IBooking Booking { get; set; }
	}
}