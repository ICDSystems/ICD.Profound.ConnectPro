using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Conference;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Conference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IOsdConferencePresenter))]
	public sealed class OsdConferencePresenter : AbstractOsdPresenter<IOsdConferenceView>, IOsdConferencePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		
		public IConferenceDeviceControl ActiveConferenceControl { get; set; }

		public OsdConferencePresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IOsdConferenceView view)
		{
			m_RefreshSection.Enter();
			try
			{
				if (Room == null || ActiveConferenceControl == null) // page shouldn't be visible
					return;

				// left panel - meeting info
				if (Room.CalendarControl == null) // hidden if no scheduler
					view.SetCurrentBookingPanelVisibility(false);
				else if (Room.CurrentBooking == null) // instant meeting
				{
					view.SetCurrentBookingPanelVisibility(true);
					view.SetCurrentBookingNameText("Instant Meeting");
					view.SetCurrentBookingTimeVisibility(false);
					view.SetCurrentBookingHostText("N/A");
				}
				else // obtp meeting
				{
					view.SetCurrentBookingPanelVisibility(true);
					view.SetCurrentBookingNameText(Room.CurrentBooking.MeetingName);

					view.SetCurrentBookingTimeVisibility(true);
					view.SetCurrentBookingTimeText(string.Format("{0} - {1}", FormatTime(Room.CurrentBooking.StartTime),
					                                             FormatTime(Room.CurrentBooking.EndTime)));

					view.SetCurrentBookingHostText(Room.CurrentBooking.OrganizerName ?? Room.CurrentBooking.OrganizerEmail ?? "N/A");
				}


				// right panel - conference device info
				string deviceIcon = Icons.GetSourceIcon(GetSourceIconString(ActiveConferenceControl.Supports), eSourceColor.White);
				view.SetSourceIcon(deviceIcon);

				var conferences = ActiveConferenceControl.GetConferences().ToList();
				var conferenceConnecting = conferences.Any(c => c.Status == eConferenceStatus.Connecting) && conferences.All(c => c.Status != eConferenceStatus.Connected);
				view.SetConnectingBannerVisibility(conferenceConnecting);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private static string GetSourceIconString(eCallType callType)
		{
			if (callType.HasFlag(eCallType.Video))
				return "videoConferencing";
			if (callType.HasFlag(eCallType.Audio))
				return "audioConferencing";
			return null;
		}

		private static string FormatTime(DateTime time)
		{
			return time.ToString("h:mmt").ToLower();
		}
	}
}
