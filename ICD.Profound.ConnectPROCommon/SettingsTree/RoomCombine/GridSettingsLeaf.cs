using System.Linq;
using ICD.Connect.Partitioning.Extensions;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.RoomCombine
{
	public sealed class GridSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// If true, the user must be logged in to access this part of the settings
		/// </summary>
		public override bool RequiresLogin { get { return false; } }

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
			Icon = eSettingsIcon.Grid;
		}
	}
}
