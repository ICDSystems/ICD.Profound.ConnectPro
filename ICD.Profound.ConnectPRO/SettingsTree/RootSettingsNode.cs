using System.Collections.Generic;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Cores;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.SettingsTree.Administrative;
using ICD.Profound.ConnectPRO.SettingsTree.Conferencing;
using ICD.Profound.ConnectPRO.SettingsTree.CUE;
using ICD.Profound.ConnectPRO.SettingsTree.RoomCombine;
using ICD.Profound.ConnectPRO.SettingsTree.Zoom;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public sealed class RootSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public RootSettingsNode(IConnectProRoom room)
			: base(room)
		{
			Name = "Settings";
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new AdministrativeSettingsNode(Room);
			yield return new ConferencingSettingsNode(Room);
			yield return new CueSettingsNode(Room);
			yield return new RoomCombineSettingsNode(Room);
			yield return new ZoomSettingsLeaf(Room);
		}

		/// <summary>
		/// If there are any dirty nodes the core is saved back to disk and the nodes are marked as clean.
		/// </summary>
		public void SaveDirtySettings()
		{
			if (!Dirty)
				return;

			ICoreSettings settings = Room.Core.CopySettings();
			FileOperations.SaveSettings(settings, true);

			SetDirty(false);
		}
	}
}
