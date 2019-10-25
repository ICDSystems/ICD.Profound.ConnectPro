using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Audio.Shure;
using ICD.Connect.Audio.Shure.Devices.MXA;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.ShureMicrophoneInterface
{
	public sealed class ConnectProShureMicrophoneInterfaceFactory :
		AbstractConnectProUserInterfaceFactory<ConnectProShureMicrophoneInterface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProShureMicrophoneInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="originator"></param>
		/// <returns></returns>
		private ConnectProShureMicrophoneInterface CreateUserInterface(IShureMxaDevice originator)
		{
			return new ConnectProShureMicrophoneInterface(originator, Theme);
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProShureMicrophoneInterface> CreateUserInterfaces(IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
			           .GetInstancesRecursive<IShureMxaDevice>()
			           .Select(originator => CreateUserInterface(originator));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsOriginator(IRoom room, ConnectProShureMicrophoneInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Microphone.Id);
		}
	}
}
