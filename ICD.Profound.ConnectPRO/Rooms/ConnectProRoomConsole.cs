using System;
using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Profound.ConnectPRO.Rooms
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

			yield return new ConsoleCommand("Start Meeting", "Starts the meeting", () => instance.StartMeeting());
			yield return new ConsoleCommand("End Meeting", "Ends the meeting", () => instance.EndMeeting(false));
			yield return new ConsoleCommand("Wake", "Wakes the room", () => instance.Wake());
			yield return new ConsoleCommand("Sleep", "Puts the room to sleep", () => instance.Sleep());
		}
	}
}
