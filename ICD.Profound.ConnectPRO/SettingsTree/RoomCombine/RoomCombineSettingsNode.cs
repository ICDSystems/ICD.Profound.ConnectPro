using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.SettingsTree.RoomCombine
{
	public sealed class RoomCombineSettingsNode : AbstractStaticSettingsNode
	{

		/// <summary>
		/// If true, the user must be logged in to access this part of the settings
		/// </summary>
		public override bool RequiresLogin { get { return false; } }


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
