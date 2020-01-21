using System.Collections.Generic;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.SettingsTree.Administrative;
using ICD.Profound.ConnectPRO.SettingsTree.Conferencing;
using ICD.Profound.ConnectPRO.SettingsTree.CUE;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public sealed class TouchDisplayRootSettingsNode : AbstractRootSettingsNode
	{
		public TouchDisplayRootSettingsNode(IConnectProRoom room) : base(room)
		{
		}
		
		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new AdministrativeSettingsNode();
			// removed, will be replaced by zoom settings in future
			// yield return new ConferencingSettingsNode();
			yield return new CueSettingsNode();
		}
	}
}
