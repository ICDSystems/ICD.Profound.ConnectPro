using System.Collections.Generic;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.SettingsTree.About;
using ICD.Profound.ConnectPRO.SettingsTree.Administrative;
using ICD.Profound.ConnectPRO.SettingsTree.Conferencing;
using ICD.Profound.ConnectPRO.SettingsTree.CUE;
using ICD.Profound.ConnectPRO.SettingsTree.RoomCombine;
using ICD.Profound.ConnectPRO.SettingsTree.Zoom;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public sealed class ConnectProRootSettingsNode : AbstractRootSettingsNode
	{
		public ConnectProRootSettingsNode(IConnectProRoom room) : base(room)
		{
		}
		
		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new AboutSettingsNode();
			yield return new AdministrativeSettingsNode();
			yield return new ConferencingSettingsNode();
			yield return new CueSettingsNode();
			yield return new RoomCombineSettingsNode();
			yield return new ZoomSettingsLeaf();
		}
	}
}
