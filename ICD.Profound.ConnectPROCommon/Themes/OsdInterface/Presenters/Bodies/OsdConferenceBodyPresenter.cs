using System;
using System.Linq;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Connect.Conferencing.Zoom.Components.System;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Bodies;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.FooterNotifications;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Bodies;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Bodies
{
	[PresenterBinding(typeof(IOsdConferenceBodyPresenter))]
	public sealed class OsdConferenceBodyPresenter : AbstractOsdPresenter<IOsdConferenceBodyView>, IOsdConferenceBodyPresenter
	{
		private const string KEY_MESSAGE_CONFERENCE = "Conference";
		private const string MESSAGE_CONFERENCE = "Your conference is about to begin.";

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

		public OsdConferenceBodyPresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			theme.DateFormatting.OnFormatChanged += DateFormattingOnFormatChanged;

		}

		protected override void Refresh(IOsdConferenceBodyView view)
		{
			m_RefreshSection.Enter();
			try
			{
				if (Room == null || ActiveConferenceControl == null) // page shouldn't be visible
					return;

				// left panel - meeting info
				// hidden if no scheduler
				if (!Room.GetCalendarControls().Any())
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
						                                    .Select(s => string.Format(CALL_IN_FORMAT, s))
						                                    .ToArray();
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

			return Room.Routing.Sources.GetRoomSources().FirstOrDefault(s => s.Device == device.Id);
		}

		private string FormatTime(DateTime time)
		{
			return Theme.DateFormatting.GetShortTime(time.ToLocalTime());
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

		#region Theme Callbacks

		private void DateFormattingOnFormatChanged(object sender, EventArgs eventArgs)
		{
			Refresh();
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

		#region View Callbacks

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (args.Data)
				Navigation.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>().PushMessage(KEY_MESSAGE_CONFERENCE, MESSAGE_CONFERENCE);
			else
				Navigation.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>().ClearMessages(KEY_MESSAGE_CONFERENCE);
		}

		#endregion
	}
}
