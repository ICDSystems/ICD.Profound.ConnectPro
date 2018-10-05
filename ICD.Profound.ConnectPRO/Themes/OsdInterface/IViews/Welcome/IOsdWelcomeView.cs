using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Welcome
{
    public interface IOsdWelcomeView : IOsdView
	{
		/// <summary>
		/// Sets the text of the availability label.
		/// </summary>
		/// <param name="availability"></param>
		void SetAvailabilityText(string availability);

	    void SetCurrentBookingIcon(string icon);

	    void SetCurrentBookingTime(string time);

	    void SetCurrentBookingSubject(string meetingName);

	    /// <summary>
	    /// Returns child views for list items.
	    /// </summary>
	    /// <param name="factory"></param>
	    /// <param name="count"></param>
	    /// <returns></returns>
	    IEnumerable<IReferencedScheduleView> GetChildComponentViews(IOsdViewFactory factory, ushort count);
	}
}
