using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.SettingsTree.RoomCombine
{
	public sealed class RoomCombineSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public RoomCombineSettingsNode()
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
			yield return new GridSettingsLeaf();
		}
	}
}
