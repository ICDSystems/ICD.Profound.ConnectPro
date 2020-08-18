using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPROCommon.Rooms
{
	public static class ConnectProRoomConsole
	{
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(IConnectProRoom instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield break;
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="addRow"></param>
		public static void BuildConsoleStatus(IConnectProRoom instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			addRow("Is In Meeting", instance.IsInMeeting);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleCommand> GetConsoleCommands(IConnectProRoom instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield return new ConsoleCommand("Start Meeting", "Starts the meeting", () => instance.StartMeeting(null, null));
			yield return new ConsoleCommand("End Meeting", "Ends the meeting", () => instance.EndMeeting(false));
			yield return new ConsoleCommand("PrintSources", "Prints a table of the sources in the room and their availability", () => PrintSources(instance));
		}

		private static string PrintSources(IConnectProRoom instance)
		{
			TableBuilder builder = new TableBuilder("Source", "Availability");

			bool combined = instance.IsCombineRoom();
			eSourceAppearance appearance = EnumUtils.GetFlagsAllValue<eSourceAppearance>();

			IcdHashSet<ISource> visible =
				instance.Routing
				        .Sources.GetRoomSourcesForUi(appearance)
				        .ToIcdHashSet();

			foreach (ISource source in instance.Routing.Sources.GetRoomSources())
			{
				string name = source.GetName(combined);
				string reason = visible.Contains(source) ? "Available" : "Hidden";

				builder.AddRow(name, reason);
			}

			return builder.ToString();
		}
	}
}
