using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Calendaring.Bookings;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Schedule
{
	public interface ISchedulePresenter : ITouchDisplayPresenter<IScheduleView>, IMainPagePresenter
	{
		event EventHandler OnRefreshed;

		event EventHandler<BookingEventArgs> OnSelectedBookingChanged;
	}

	public sealed class BookingEventArgs : GenericEventArgs<IBooking>
	{
		public BookingEventArgs(IBooking data) : base(data)
		{
		}
	}
}