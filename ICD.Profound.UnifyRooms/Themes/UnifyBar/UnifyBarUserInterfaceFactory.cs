using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar
{
	public sealed class UnifyBarUserInterfaceFactory : AbstractUserInterfaceFactory<UnifyBarUserInterface>
	{
		/// <summary>
		/// Gets the theme.
		/// </summary>
		public new UnifyRoomsTheme Theme { get { return (UnifyRoomsTheme)base.Theme; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public UnifyBarUserInterfaceFactory(UnifyRoomsTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Creates the user interfaces for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		protected override IEnumerable<UnifyBarUserInterface> CreateUserInterfaces(IRoom room)
		{
			return room.Originators
					   .GetInstancesRecursive<UnifyBarDevice>()
					   .Select(o => new UnifyBarUserInterface(o, Theme));
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsTarget(IRoom room, UnifyBarUserInterface ui)
		{
			return room.Originators
					   .GetInstancesRecursive<UnifyBarDevice>()
					   .Contains(ui.UnifyBar);
		}
	}
}
