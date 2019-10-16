using System;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule
{
	public interface IScheduleView : ITouchDisplayView
	{
		event EventHandler OnCloseButtonPressed;

		event EventHandler OnStartBookingButtonPressed;

		/// <summary>
		///     Sets the text of the availability label.
		/// </summary>
		/// <param name="availability"></param>
		void SetAvailabilityText(string availability);

		void SetAvailabilityVisible(bool visible);

		void SetCurrentBookingIcon(string icon);

		void SetCurrentBookingTime(string time);

		void SetCurrentBookingSubject(string meetingName);

		void SetColorMode(eScheduleViewColorMode mode);

		void SetCloseButtonVisible(bool visible);

		/// <summary>
		///     Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedBookingView> GetChildComponentViews(ITouchDisplayViewFactory factory, ushort count);
	}

	public enum eScheduleViewColorMode : ushort
	{
		Blue = 0,
		Red = 1,
		Green = 2
	}
}