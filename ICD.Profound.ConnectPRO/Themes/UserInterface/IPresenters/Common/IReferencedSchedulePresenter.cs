using System;
using ICD.Connect.Calendaring.Bookings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common
{
	public interface IReferencedSchedulePresenter : IUiPresenter<IReferencedScheduleView>
	{
		/// <summary>
		/// Raised when the user presses the presenter.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Gets/sets the booking for this presenter.
		/// </summary>
		IBooking Booking { get; set; }

		/// <summary>
		/// Sets the selected state.
		/// </summary>
		/// <value></value>
		bool Selected { get; set; }
	}
}
