﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.IncomingCalls;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Devices;
using ICD.Connect.Misc.Yepkit.Devices.YkupSwitcher;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.YkupSwitcherInterface;

namespace ICD.Profound.ConnectPRO.Dialing
{
	public sealed class ConnectProDialing
	{
		private const long ZOOM_USB_SETUP_TIME = 5 * 1000;

		/// <summary>
		/// Raised when an incoming call is answered.
		/// </summary>
		public event EventHandler<GenericEventArgs<IIncomingCall>> OnIncomingCallAnswered;

		/// <summary>
		/// Raised when an incoming call is rejected.
		/// </summary>
		public event EventHandler<GenericEventArgs<IIncomingCall>> OnIncomingCallRejected;

		private readonly IConnectProRoom m_Room;
		private readonly SafeTimer m_CallSetupTimer;

		private Action m_StartCall;

		/// <summary>
		/// Gets/sets the ATC number for dialing into the room.
		/// </summary>
		[CanBeNull]
		public string AtcNumber { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ConnectProDialing([NotNull] IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			m_Room = room;
			m_CallSetupTimer = SafeTimer.Stopped(FinishCallSetup);
		}

		#region Methods

		/// <summary>
		/// Dials the given booking and routes the dialer.
		/// </summary>
		/// <param name="booking"></param>
		public void DialBooking([NotNull] IBooking booking)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			IEnumerable<IConferenceDeviceControl> dialers =
				m_Room.ConferenceManager == null
					? Enumerable.Empty<IConferenceDeviceControl>()
					: m_Room.ConferenceManager.GetDialingProviders();

			// Build map of dialer to best number
			IDialContext dialContext;
			IConferenceDeviceControl preferredDialer = ConferencingBookingUtils.GetBestDialer(booking, dialers, out dialContext);
			if (preferredDialer == null)
				return;

			// Route device to displays and/or audio destination
			IDeviceBase dialerDevice = preferredDialer.Parent;
			ISource source = m_Room.Routing.Sources.GetCoreSources().FirstOrDefault(s => s.Device == dialerDevice.Id);
			if (source == null)
				return; // if we can't route a source, don't dial into conference users won't know they're in

			m_Room.FocusSource = source;

			if (preferredDialer.Supports.HasFlag(eCallType.Video))
				m_Room.Routing.RouteVtc(source);
			else if (preferredDialer.Supports.HasFlag(eCallType.Audio))
				m_Room.Routing.RouteAtc(source);
			else
				m_Room.Routing.RouteToAllDisplays(source);

			// Dial booking
			Dial(preferredDialer, dialContext);
		}

		/// <summary>
		/// Dials the context using the given conference control.
		/// </summary>
		/// <param name="conferenceControl"></param>
		/// <param name="context"></param>
		public void Dial([NotNull] IConferenceDeviceControl conferenceControl, [NotNull] IDialContext context)
		{
			if (conferenceControl == null)
				throw new ArgumentNullException("conferenceControl");

			if (context == null)
				throw new ArgumentNullException("context");

			SetupCall(conferenceControl, () => conferenceControl.Dial(context));
		}

		/// <summary>
		/// Starts a personal meeting using the given web conference control.
		/// </summary>
		/// <param name="webConferenceControl"></param>
		public void StartPersonalMeeting([NotNull] IWebConferenceDeviceControl webConferenceControl)
		{
			if (webConferenceControl == null)
				throw new ArgumentNullException("webConferenceControl");

			SetupCall(webConferenceControl, webConferenceControl.StartPersonalMeeting);
		}

		/// <summary>
		/// Answers the incoming call and focuses on the given conference call.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="call"></param>
		public void AnswerIncomingCall([CanBeNull] IConferenceDeviceControl control, [NotNull] IIncomingCall call)
		{
			if (call == null)
				throw new ArgumentNullException("call");

			SetupCall(control, call.Answer);

			m_Room.StartMeeting(false);

			// Focus on the dialer source
			IDeviceBase device = control == null ? null : control.Parent;
			ISource source = device == null ? null : m_Room.Originators.GetInstanceRecursive<ISource>(s => s.Device == device.Id);

			m_Room.FocusSource = source;

			if (source != null && control.Supports.HasFlag(eCallType.Video))
				m_Room.Routing.RouteVtc(source);
			else if (source != null && control.Supports.HasFlag(eCallType.Audio))
				m_Room.Routing.RouteAtc(source);

			OnIncomingCallAnswered.Raise(this, new GenericEventArgs<IIncomingCall>(call));
		}

		/// <summary>
		/// Rejects the incoming call.
		/// </summary>
		/// <param name="call"></param>
		public void RejectIncomingCall(IIncomingCall call)
		{
			if (call == null)
				throw new ArgumentNullException("call");

			call.Reject();

			OnIncomingCallRejected.Raise(this, new GenericEventArgs<IIncomingCall>(call));
		}

		/// <summary>
		/// Ends all of the online conferences.
		/// </summary>
		public void EndAllConferences()
		{
			List<IConference> activeConferences =
				m_Room.ConferenceManager == null
					? new List<IConference>()
					: m_Room.ConferenceManager.OnlineConferences.ToList();

			foreach (IConference activeConference in activeConferences)
				EndConference(activeConference);
		}

		/// <summary>
		/// Returns true if:
		/// 
		/// We are in a conference and the conference source does not use the Hide override.
		/// OR
		/// We have a routed source using the Show override.
		/// </summary>
		/// <param name="minimumCallType"></param>
		/// <returns></returns>
		public bool ConferenceActionsAvailable(eInCall minimumCallType)
		{
			// Are we in a conference and the source is NOT using the Hide override?
			if (m_Room.ConferenceManager != null && m_Room.ConferenceManager.IsInCall >= minimumCallType)
				return GetActiveConferenceSourceOverride() != eConferenceOverride.Hide;

			// Is a source routed with the Show override?
			return
				m_Room.Routing
				      .State
				      .GetFakeActiveVideoSources()
				      .SelectMany(kvp => kvp.Value)
				      .OfType<ConnectProSource>()
				      .Any(s => s.ConferenceOverride == eConferenceOverride.Show);
		}

		#endregion

		#region Private Methods

		private static void EndConference([NotNull] IConference conference)
		{
			if (conference == null)
				throw new ArgumentNullException("conference");

			// TODO - Actually use polymorphism like a good developer
			var traditional = conference as ITraditionalConference;
			if (traditional != null)
				traditional.Hangup();

			var web = conference as IWebConference;
			if (web != null)
				web.LeaveConference();
		}

		/// <summary>
		/// Returns the conference override for the active conferences. Show beats Hide.
		/// </summary>
		/// <returns></returns>
		private eConferenceOverride GetActiveConferenceSourceOverride()
		{
			if (m_Room.ConferenceManager == null)
				return eConferenceOverride.None;

			return
				m_Room.ConferenceManager
				      .GetDialingProviders()
				      .Where(p => p.GetActiveConference() != null)
				      .SelectMany(p => m_Room.Routing.Sources.GetSources(p))
				      .OfType<ConnectProSource>()
				      .Select(s => s.ConferenceOverride)
				      .MaxOrDefault();
		}

		/// <summary>
		/// Runs the necessary setup for the given conference control before starting the call.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="startCall"></param>
		private void SetupCall([CanBeNull] IConferenceDeviceControl control, [NotNull] Action startCall)
		{
			if (startCall == null)
				throw new ArgumentNullException("startCall");

			m_StartCall = startCall;

			IDeviceBase parent = control == null ? null : control.Parent;

			if (parent is ZoomRoom)
			{
				RouteUsbForZoom();
				return;
			}

			FinishCallSetup();
		}

		/// <summary>
		/// Performs the final step of call setup.
		/// </summary>
		private void FinishCallSetup()
		{
			if (m_StartCall == null)
				throw new InvalidOperationException("No Start Call Action");

			m_StartCall();
		}

		/// <summary>
		/// We need to ensure our USB routes are stable before starting a Zoom call.
		/// </summary>
		private void RouteUsbForZoom()
		{
			bool hasUsb = false;

			// Hack - Need to figure out a better way of tracking Zoom mic/camera routing
			foreach (YkupSwitcherDevice switcher in m_Room.Originators.GetInstancesRecursive<YkupSwitcherDevice>())
			{
				hasUsb = true;
				switcher.Route(ConnectProYkupSwitcherInterface.ZOOM_OUTPUT);
			}

			// Hack - Wait several seconds before actually starting the call
			if (hasUsb)
				m_CallSetupTimer.Reset(ZOOM_USB_SETUP_TIME);
			else
				FinishCallSetup();
		}

		#endregion
	}
}
