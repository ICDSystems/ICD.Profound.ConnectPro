using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Devices;
using ICD.Connect.Panels.Crestron.Controls.TouchScreens;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.Mpc3201UserInterface
{
	public sealed class Mpc3201UserInterfaceFactory : AbstractConnectProUserInterfaceFactory<Mpc3201UserInterface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public Mpc3201UserInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		private Mpc3201UserInterface CreateUserInterface(IMPC3x201TouchScreenControl control)
		{
			return new Mpc3201UserInterface(control, Theme);
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<Mpc3201UserInterface> CreateUserInterfaces(IConnectProRoom room)
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
		protected override bool RoomContainsOriginator(IRoom room, Mpc3201UserInterface ui)
		{
			return room.Originators
			           .GetInstancesRecursive<IDeviceBase>()
			           .SelectMany(d => d.Controls.GetControls<IMPC3x201TouchScreenControl>())
			           .Contains(ui.TouchScreen);
		}
	}
}
