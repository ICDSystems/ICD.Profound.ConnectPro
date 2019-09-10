using System;
using ICD.Common.Utils;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.Shared.Models;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Schedule;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Schedule
{
    [PresenterBinding(typeof(IReferencedBookingPresenter))]
	public sealed class ReferencedBookingPresenter : AbstractTouchDisplayComponentPresenter<IReferencedBookingView>, IReferencedBookingPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private const string TIME_HTML_FORMAT = "<span style=\"color: {0}\">{1}</span>";
		private const string AVAILABLE_COLOR = "#67FCF1";
		private const string RESERVED_COLOR = "#F0544F";

		public IBooking Booking { get; set; }

	    public ReferencedBookingPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
	    {
            m_RefreshSection = new SafeCriticalSection();
	    }

		protected override void  Refresh(IReferencedBookingView view)
		{
 			base.Refresh(view);
            
		    if (Booking == null)
		        return;

			m_RefreshSection.Enter();

		    try
		    {
		        string color = Booking is EmptyBooking ? AVAILABLE_COLOR : RESERVED_COLOR;

			    string timeString =
				    Booking.EndTime == DateTime.MaxValue
					    ? "Remaining Time"
					    : string.Format("{0} - {1}", FormatTime(Booking.StartTime), FormatTime(Booking.EndTime));

		        view.SetTimeLabel(string.Format(TIME_HTML_FORMAT, color, timeString));

		        view.SetSubjectLabel(Booking.IsPrivate ? "Private Meeting" : Booking.MeetingName);
		    }
		    finally
		    {
		        m_RefreshSection.Leave();
		    }
		}

        private static string FormatTime(DateTime time)
        {
	        return ConnectProDateFormatting.GetShortTime(time);
        }
	}
}