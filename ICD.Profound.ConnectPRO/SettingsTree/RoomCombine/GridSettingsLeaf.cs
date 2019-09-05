using System.Linq;
using ICD.Connect.Partitioning.Extensions;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.RoomCombine
{
	public sealed class GridSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		public override bool Visible { get { return base.Visible && Room.Core.GetPartitionManager().Cells.Any(); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public GridSettingsLeaf(IConnectProRoom room)
			: base(room)
		{
			Name = "Grid";
			Icon = SettingsTreeIcons.ICON_GRID;
		}
	}
}
