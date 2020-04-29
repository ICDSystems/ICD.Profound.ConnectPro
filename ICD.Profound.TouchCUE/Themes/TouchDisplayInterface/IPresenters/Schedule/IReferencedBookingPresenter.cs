using System;
using ICD.Connect.Calendaring.Booking;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Schedule
{
	public interface IReferencedBookingPresenter : ITouchDisplayPresenter<IReferencedBookingView>
	{
		IBooking Booking { get; set; }
		event EventHandler OnBookingPressed;

		void SetSelected(bool selected);
	}
}