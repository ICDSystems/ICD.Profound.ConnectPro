using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Misc.Yepkit.Devices.YkupSwitcher;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.YkupSwitcherInterface
{
	public sealed class ConnectProYkupSwitcherInterfaceFactory :
		AbstractConnectProUserInterfaceFactory<IConnectProTheme, ConnectProYkupSwitcherInterface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProYkupSwitcherInterfaceFactory(IConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="originator"></param>
		/// <returns></returns>
		private ConnectProYkupSwitcherInterface CreateUserInterface(YkupSwitcherDevice originator)
		{
			return new ConnectProYkupSwitcherInterface(originator, Theme);
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProYkupSwitcherInterface> CreateUserInterfaces(IRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
			           .GetInstancesRecursive<YkupSwitcherDevice>()
			           .Select(originator => CreateUserInterface(originator));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsTarget(IRoom room, ConnectProYkupSwitcherInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Switcher.Id);
		}
	}
}
