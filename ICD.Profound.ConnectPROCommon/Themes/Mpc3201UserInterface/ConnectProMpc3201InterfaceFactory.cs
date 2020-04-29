using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Devices;
using ICD.Connect.Panels.Crestron.Controls.TouchScreens;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.Mpc3201UserInterface
{
	public sealed class ConnectProMpc3201InterfaceFactory : AbstractConnectProUserInterfaceFactory<IConnectProTheme, ConnectProMpc3201Interface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProMpc3201InterfaceFactory(IConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProMpc3201Interface> CreateUserInterfaces(IRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
					   .GetInstancesRecursive<IDevice>()
					   .SelectMany(d => d.Controls.GetControls<IMPC3x201TouchScreenControl>())
					   .Select(o => new ConnectProMpc3201Interface(o, Theme));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsTarget(IRoom room, ConnectProMpc3201Interface ui)
		{
			return room.Originators
			           .GetInstancesRecursive<IDevice>()
			           .SelectMany(d => d.Controls.GetControls<IMPC3x201TouchScreenControl>())
			           .Contains(ui.TouchScreen);
		}
	}
}
