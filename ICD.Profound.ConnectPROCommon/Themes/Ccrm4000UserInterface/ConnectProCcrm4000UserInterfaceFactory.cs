using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Audio.ClockAudio.Devices.CCRM4000;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.Ccrm4000UserInterface
{
	public sealed class ConnectProCcrm4000UserInterfaceFactory :
		AbstractConnectProUserInterfaceFactory<IConnectProTheme, ConnectProCcrm4000UserInterface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProCcrm4000UserInterfaceFactory(IConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProCcrm4000UserInterface> CreateUserInterfaces(IRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
					   .GetInstancesRecursive<ClockAudioCcrm4000Device>()
					   .Select(originator => new ConnectProCcrm4000UserInterface(originator, Theme));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsTarget(IRoom room, ConnectProCcrm4000UserInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Microphone.Id);
		}
	}
}
