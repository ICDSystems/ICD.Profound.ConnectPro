using System.Collections.Generic;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.RoomCombine
{
	public sealed class RoomCombineSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public RoomCombineSettingsNode(IConnectProRoom room)
			: base(room)
		{
			Name = "Room Combine";
			Icon = SettingsTreeIcons.ICON_ROOM_COMBINE;
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new GridSettingsLeaf(Room);
		}
	}
}
