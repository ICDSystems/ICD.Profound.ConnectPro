using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Audio.ClockAudio.Devices.CCRM4000;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.Ccrm4000UserInterface
{
	public sealed class ConnectProCcrm4000UserInterfaceFactory :
		AbstractConnectProUserInterfaceFactory<ConnectProCcrm4000UserInterface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProCcrm4000UserInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="originator"></param>
		/// <returns></returns>
		private ConnectProCcrm4000UserInterface CreateUserInterface(ClockAudioCcrm4000Device originator)
		{
			return new ConnectProCcrm4000UserInterface(originator, Theme);
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProCcrm4000UserInterface> CreateUserInterfaces(IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
					   .GetInstancesRecursive<ClockAudioCcrm4000Device>()
			           .Select(originator => CreateUserInterface(originator));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsOriginator(IRoom room, ConnectProCcrm4000UserInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Microphone.Id);
		}
	}
}
