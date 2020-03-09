using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface
{
	public sealed class ConnectProOsdInterfaceFactory : AbstractConnectProUserInterfaceFactory<ConnectProOsdInterface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProOsdInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProOsdInterface> CreateUserInterfaces(IRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
			           .GetInstancesRecursive<OsdPanelDevice>()
					   .Select(o => new ConnectProOsdInterface(o, Theme));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsTarget(IRoom room, ConnectProOsdInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Panel.Id);
		}
	}
}
