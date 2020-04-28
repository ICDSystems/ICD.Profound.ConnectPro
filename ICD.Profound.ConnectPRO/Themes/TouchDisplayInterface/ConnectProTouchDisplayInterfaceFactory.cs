﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface
{
	public sealed class
		ConnectProTouchDisplayInterfaceFactory : AbstractConnectProUserInterfaceFactory<ConnectProTouchDisplayInterface>
	{
		public ConnectProTouchDisplayInterfaceFactory(ConnectProTheme theme) : base(theme)
		{
		}

		/// <summary>
		///     Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<ConnectProTouchDisplayInterface> CreateUserInterfaces(IRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			return room.Originators
				.GetInstancesRecursive<VibeBoard>()
				.Select(o => CreateUserInterface(o));
		}

		/// <summary>
		///     Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="panel"></param>
		/// <returns></returns>
		private ConnectProTouchDisplayInterface CreateUserInterface(IPanelDevice originator)
		{
			return new ConnectProTouchDisplayInterface(originator, Theme);
		}

		/// <summary>
		///     Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsTarget(IRoom room, ConnectProTouchDisplayInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Panel.Id);
		}
	}
}