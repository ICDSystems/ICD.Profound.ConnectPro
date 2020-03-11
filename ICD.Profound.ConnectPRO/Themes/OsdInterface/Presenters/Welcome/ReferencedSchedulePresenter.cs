﻿using System;
using ICD.Common.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Welcome;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Welcome;
using ICD.Connect.Calendaring.Booking;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Welcome
{
	[PresenterBinding(typeof(IReferencedSchedulePresenter))]
	public sealed class ReferencedSchedulePresenter : AbstractOsdComponentPresenter<IReferencedScheduleView>, IReferencedSchedulePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private const string TIME_HTML_FORMAT = "<span style=\"color: {0}\">{1}</span>";
		private const string AVAILABLE_COLOR = "#67FCF1";
		private const string RESERVED_COLOR = "#F0544F";

		public IBooking Booking { get; set; }

	    public ReferencedSchedulePresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme)
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