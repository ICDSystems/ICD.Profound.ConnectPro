using System;
using ICD.Common.Utils;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Bodies;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Bodies;
using ICD.Profound.ConnectPROCommon.Themes.Shared.Models;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Bodies
{
	[PresenterBinding(typeof(IReferencedSchedulePresenter))]
	public sealed class ReferencedSchedulePresenter : AbstractOsdComponentPresenter<IReferencedScheduleView>, IReferencedSchedulePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private const string TIME_HTML_FORMAT = "<span style=\"color: {0}\">{1}</span>";
		private const string AVAILABLE_COLOR = "#67FCF1";
		private const string RESERVED_COLOR = "#F0544F";

		public IBooking Booking { get; set; }

	    public ReferencedSchedulePresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
	    {
            m_RefreshSection = new SafeCriticalSection();

			theme.DateFormatting.OnFormatChanged += DateFormattingOnFormatChanged;
	    }

		protected override void  Refresh(IReferencedScheduleView view)
		{
 			base.Refresh(view);
            
		    if (Booking == null)
		        return;

			m_RefreshSection.Enter();

		    try
		    {
			    bool isRemainingTimePlaceholder = Booking is EmptyBooking;
			    bool allDay = !isRemainingTimePlaceholder && (Booking.EndTime - Booking.StartTime).TotalHours >= 23;

				string color = Booking is EmptyBooking ? AVAILABLE_COLOR : RESERVED_COLOR;

				string timeString =
					isRemainingTimePlaceholder
						? "Remaining Time"
						: allDay
							? "All Day"
							: string.Format("{0} - {1}", FormatTime(Booking.StartTime), FormatTime(Booking.EndTime));

				view.SetTimeLabel(string.Format(TIME_HTML_FORMAT, color, timeString));
		        view.SetSubjectLabel(Booking.IsPrivate ? "Private Meeting" : Booking.MeetingName);
		    }
		    finally
		    {
		        m_RefreshSection.Leave();
		    }
		}

		private void DateFormattingOnFormatChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		private string FormatTime(DateTime time)
        {
	        return Theme.DateFormatting.GetShortTime(time.ToLocalTime());
        }
	}
}