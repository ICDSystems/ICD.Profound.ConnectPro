using System.Linq;
using ICD.Connect.Partitioning.Extensions;

namespace ICD.Profound.ConnectPRO.SettingsTree.RoomCombine
{
	public sealed class GridSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		public override bool Visible
		{
			get
			{
				return base.Visible &&
				       Room != null &&
				       Room.Core.GetPartitionManager().Cells.Any();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public GridSettingsLeaf()
		{
			Name = "Grid";
			Icon = SettingsTreeIcons.ICON_GRID;
		}
	}
}
