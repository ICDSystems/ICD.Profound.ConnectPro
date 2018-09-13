using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Devices;
using ICD.Connect.Panels.Crestron.Controls.TouchScreens;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.Mpc3201UserInterface
{
	public sealed class ConnectProMpc3201InterfaceFactory : AbstractConnectProUserInterfaceFactory<ConnectProMpc3201Interface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProMpc3201InterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		private ConnectProMpc3201Interface CreateUserInterface(IMPC3x201TouchScreenControl control)
		{
			return new ConnectProMpc3201Interface(control, Theme);
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProMpc3201Interface> CreateUserInterfaces(IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
					   .GetInstancesRecursive<IDeviceBase>()
					   .SelectMany(d => d.Controls.GetControls<IMPC3x201TouchScreenControl>())
					   .Select(o => CreateUserInterface(o));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsOriginator(IRoom room, ConnectProMpc3201Interface ui)
		{
			return room.Originators
			           .GetInstancesRecursive<IDeviceBase>()
			           .SelectMany(d => d.Controls.GetControls<IMPC3x201TouchScreenControl>())
			           .Contains(ui.TouchScreen);
		}
	}
}
