using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Utils
{
	public static class ConferenceOverrideUtils
	{
		/// <summary>
		/// Returns true if:
		/// 
		/// We are in a conference and the conference source does not use the Hide override.
		/// OR
		/// We have a routed source using the Show override.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="inCall"></param>
		/// <returns></returns>
		public static bool ConferenceActionsAvailable([NotNull] IConnectProRoom room, eInCall inCall)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			// Are we in a conference and the source is NOT using the Hide override?
			IConferenceManager conferenceManager = room.ConferenceManager;
			if (conferenceManager != null && conferenceManager.IsInCall >= inCall)
				return GetActiveConferenceSourceOverride(room) != eConferenceOverride.Hide;

			// Is a source routed with the Show override?
			return
				room.Routing
				    .State
				    .GetFakeActiveVideoSources()
				    .SelectMany(kvp => kvp.Value)
				    .OfType<ConnectProSource>()
				    .Any(s => s.ConferenceOverride == eConferenceOverride.Show);
		}

		/// <summary>
		/// Returns the conference override for the active conferences. Show beats Hide.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		private static eConferenceOverride GetActiveConferenceSourceOverride([NotNull] IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			IConferenceManager conferenceManager = room.ConferenceManager;
			if (conferenceManager == null)
				return eConferenceOverride.None;

			return
				conferenceManager.GetDialingProviders()
				                 .Where(p => p.GetActiveConference() != null)
				                 .SelectMany(p => room.Routing.Sources.GetSources(p))
				                 .OfType<ConnectProSource>()
				                 .Select(s => s.ConferenceOverride)
								 .Max();
		}
	}
}
