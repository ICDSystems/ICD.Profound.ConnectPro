using System;
using System.Linq;
using System.Text;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Connect.Conferencing.Zoom.Components.System;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Conference;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IOsdConferencePresenter))]
	public sealed class OsdConferencePresenter : AbstractOsdPresenter<IOsdConferenceView>, IOsdConferencePresenter
	{
		private const string MEETING_NUMBER_FORMAT = "<div class=\"conferenceInfoLabel\">Meeting Number: </div><div class=\"conferenceInfoField\"> {0}</div>";
		private const string CALL_IN_FORMAT = "<span class=\"blueText\">{0}</span>";

		private readonly SafeCriticalSection m_RefreshSection;

		private CallInfo m_CachedCallInfo;


		private IConferenceDeviceControl m_ActiveConferenceControl;
		public IConferenceDeviceControl ActiveConferenceControl
		{
			get { return m_ActiveConferenceControl; }
			set
			{
				if (m_ActiveConferenceControl == value)
					return;

				Unsubscribe(m_ActiveConferenceControl);
				m_ActiveConferenceControl = value;
				Subscribe(m_ActiveConferenceControl);
			}
		}

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
				// hidden if no scheduler
				if (Room.CalendarControl == null)
					view.SetCurrentBookingPanelVisibility(false);
				// instant meeting
				else if (Room.CurrentBooking == null)
				{
					view.SetCurrentBookingPanelVisibility(true);
					view.SetCurrentBookingNameText("Instant Meeting");
					view.SetCurrentBookingTimeVisibility(false);
					view.SetCurrentBookingTimeText(string.Empty); // TODO show "Now - ..." next meeting time or end of hour
					view.SetCurrentBookingHostText("N/A");
				}
				// OBTP meeting
				else
				{
					view.SetCurrentBookingPanelVisibility(true);
					view.SetCurrentBookingNameText(Room.CurrentBooking.MeetingName);

					view.SetCurrentBookingTimeVisibility(true);
					view.SetCurrentBookingTimeText(string.Format("{0} - {1}", FormatTime(Room.CurrentBooking.StartTime),
					                                             FormatTime(Room.CurrentBooking.EndTime)));

					view.SetCurrentBookingHostText(Room.CurrentBooking.OrganizerName ?? Room.CurrentBooking.OrganizerEmail ?? "N/A");
				}
				
				// right panel - conference device info
				var source = GetSource();
				var sourceIcon = !(source is ConnectProSource) ? null : (source as ConnectProSource).Icon;
				view.SetSourceIcon(sourceIcon);
				view.SetSourceNameText(source == null ? "Unknown Source" : source.Name);
				if (ActiveConferenceControl.Parent is ZoomRoom) // zoom specific info
				{
					var builder = new StringBuilder();

					builder.Append("<div class=\"conferenceInfo\">");
					// meeting number
					if (Room.CurrentBooking != null)
					{
						var booking = Room.CurrentBooking;
						var dialContext = booking.GetBookingNumbers()
												 .FirstOrDefault(b => b.Protocol == eDialProtocol.Zoom && !string.IsNullOrEmpty(b.DialString));
						if (dialContext != null)
							builder.Append(string.Format(MEETING_NUMBER_FORMAT, dialContext.DialString));

					}
					else
					{
						var zoomRoom = ActiveConferenceControl.Parent as ZoomRoom;
						var systemComponent = zoomRoom.Components.GetComponent<SystemComponent>();
						if (systemComponent != null && systemComponent.SystemInfo != null)
						{
							var info = systemComponent.SystemInfo;
							builder.Append(string.Format(MEETING_NUMBER_FORMAT, info.MeetingNumber));
						}
					}
					
					// call in numbers
					builder.Append("<div class=\"conferenceInfoLabel\">Call In Number: </div><div class=\"conferenceInfoField\">");
					if (m_CachedCallInfo != null && !string.IsNullOrEmpty(m_CachedCallInfo.DialIn))
					{
						var callInNumbers = m_CachedCallInfo.DialIn
						                                   .Split(';')
						                                   .Select(s => string.Format(CALL_IN_FORMAT, s));
						builder.Append(string.Join("<br/>", callInNumbers));
					}
					else
					{
						builder.Append("Join a meeting to<br />initialize call in numbers");
					}
					builder.Append("</div></div>");

					view.SetSourceDescriptionText(builder.ToString());
				}
				else // use source description as backup
					view.SetSourceDescriptionText(source == null ? string.Empty : source.Description);

				var conferences = ActiveConferenceControl.GetConferences().ToList();
				var conferenceConnecting = conferences.Any(c => c.Status == eConferenceStatus.Connecting);
				view.SetConnectingBannerVisibility(conferenceConnecting);

				var conferenceDisconnecting = conferences.Any(c => c.Status == eConferenceStatus.Disconnecting);
				view.SetDisconnectingBannerVisibility(conferenceDisconnecting);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private ISource GetSource()
		{
			if (Room == null)
				return null;
			
			var device = ActiveConferenceControl == null ? null : ActiveConferenceControl.Parent;
			if (device == null)
				return null;

			return Room.Routing.Sources.GetSources().FirstOrDefault(s => s.Device == device.Id)
			       ?? Room.Routing.Sources.GetCoreSources().FirstOrDefault(s => s.Device == device.Id);
		}

		private static string FormatTime(DateTime time)
		{
			return time.ToString("h:mmt").ToLower();
		}

		#region Control Callbacks

		private void Subscribe(IConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences() ?? Enumerable.Empty<IConference>())
				Subscribe(conference);
		}

		private void Unsubscribe(IConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences() ?? Enumerable.Empty<IConference>())
				Unsubscribe(conference);
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data);
			RefreshIfVisible();
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			Unsubscribe(e.Data);
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs e)
		{
			var zoomConference = sender as CallComponent;
			if (zoomConference != null && zoomConference.CallInfo != null)
				m_CachedCallInfo = zoomConference.CallInfo;
			RefreshIfVisible();
		}

		#endregion
	}
}
