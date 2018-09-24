using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Audio.Shure;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.MicrophoneInterface
{
	public sealed class ConnectProMicrophoneInterfaceFactory :
		AbstractConnectProUserInterfaceFactory<ConnectProMicrophoneInterface>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProMicrophoneInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="originator"></param>
		/// <returns></returns>
		private ConnectProMicrophoneInterface CreateUserInterface(IShureMxaDevice originator)
		{
			return new ConnectProMicrophoneInterface(originator, Theme);
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProMicrophoneInterface> CreateUserInterfaces(IConnectProRoom room)
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
		protected override bool RoomContainsOriginator(IRoom room, ConnectProMicrophoneInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Microphone.Id);
		}
	}
}
