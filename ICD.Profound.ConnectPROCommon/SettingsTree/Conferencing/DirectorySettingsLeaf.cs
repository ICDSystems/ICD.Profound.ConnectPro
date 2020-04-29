using System.Linq;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.Conferencing
{
	public sealed class DirectorySettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		public override bool Visible { get { return base.Visible && Room.GetControlsRecursive<IDirectoryControl>().Any(); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public DirectorySettingsLeaf()
		{
			Name = "Directory";
			Icon = eSettingsIcon.Directory;
		}

		/// <summary>
		/// Clears and repopulates the directories.
		/// </summary>
		public void Refresh()
		{
			foreach (IDirectoryControl control in Room.GetControlsRecursive<IDirectoryControl>())
			{
				control.Clear();
				control.PopulateFolder(control.GetRoot());
			}
		}
	}
}
