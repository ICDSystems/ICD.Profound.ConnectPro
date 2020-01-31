using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.IncomingCalls;
using ICD.Connect.Devices;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Dialing
{
	public sealed class ConnectProDialing
	{
		/// <summary>
		/// Raised when an incoming call is answered.
		/// </summary>
		public event EventHandler<GenericEventArgs<IIncomingCall>> OnIncomingCallAnswered;

		/// <summary>
		/// Raised when an incoming call is rejected.
		/// </summary>
		public event EventHandler<GenericEventArgs<IIncomingCall>> OnIncomingCallRejected;

		private readonly IConnectProRoom m_Room;

		/// <summary>
		/// Gets/sets the ATC number for dialing into the room.
		/// </summary>
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
		}

		#region Methods

		/// <summary>
		/// Answers the incoming call and focuses on the given conference call.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="call"></param>
		public void AnswerIncomingCall(IConferenceDeviceControl control, [NotNull] IIncomingCall call)
		{
			if (call == null)
				throw new ArgumentNullException("call");

			call.Answer();

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

		#endregion
	}
}
