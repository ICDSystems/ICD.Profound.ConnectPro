using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Audio.Shure.Devices.MX;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.ShureMx396Interface
{
	public sealed class ConnectProShureMx396InterfaceFactory :
		AbstractConnectProUserInterfaceFactory<ConnectProShureMx396Interface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProShureMx396InterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="originator"></param>
		/// <returns></returns>
		private ConnectProShureMx396Interface CreateUserInterface(ShureMx396Device originator)
		{
			return new ConnectProShureMx396Interface(originator);
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProShureMx396Interface> CreateUserInterfaces(IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
			           .GetInstancesRecursive<ShureMx396Device>()
			           .Select(originator => CreateUserInterface(originator));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsOriginator(IRoom room, ConnectProShureMx396Interface ui)
		{
			return room.Originators.ContainsRecursive(ui.Microphone.Id);
		}
	}
}
