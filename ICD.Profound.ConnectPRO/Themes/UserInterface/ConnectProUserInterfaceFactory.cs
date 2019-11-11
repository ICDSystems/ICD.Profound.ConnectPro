using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface
{
	public sealed class ConnectProUserInterfaceFactory : AbstractConnectProUserInterfaceFactory<ConnectProUserInterface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProUserInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="panel"></param>
		/// <returns></returns>
		private ConnectProUserInterface CreateUserInterface(IPanelDevice panel)
		{
			return new ConnectProUserInterface(panel, Theme);
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProUserInterface> CreateUserInterfaces(IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
			           .GetInstancesRecursive<IPanelDevice>()
			           .Where(o => !(o is OsdPanelDevice) && !(o is VibeBoard))
			           .Select(o => CreateUserInterface(o));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsOriginator(IRoom room, ConnectProUserInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Panel.Id);
		}
	}
}
