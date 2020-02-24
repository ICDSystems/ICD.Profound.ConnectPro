using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Devices;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.EventServerUserInterface
{
	public sealed class ConnectProEventServerUserInterfaceFactory :
		AbstractConnectProUserInterfaceFactory<ConnectProEventServerUserInterface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProEventServerUserInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="originator"></param>
		/// <returns></returns>
		private ConnectProEventServerUserInterface CreateUserInterface(ConnectProEventServerDevice originator)
		{
			return new ConnectProEventServerUserInterface(originator, Theme);
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProEventServerUserInterface> CreateUserInterfaces(IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
					   .GetInstancesRecursive<ConnectProEventServerDevice>()
					   .Select(originator => CreateUserInterface(originator));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsOriginator(IRoom room, ConnectProEventServerUserInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Device.Id);
		}
	}
}
