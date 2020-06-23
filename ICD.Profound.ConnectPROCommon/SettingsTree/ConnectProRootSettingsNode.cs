using System.Collections.Generic;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.SettingsTree.About;
using ICD.Profound.ConnectPROCommon.SettingsTree.Administrative;
using ICD.Profound.ConnectPROCommon.SettingsTree.Conferencing;
using ICD.Profound.ConnectPROCommon.SettingsTree.CUE;
using ICD.Profound.ConnectPROCommon.SettingsTree.RoomCombine;
using ICD.Profound.ConnectPROCommon.SettingsTree.TouchFreeConferencing;
using ICD.Profound.ConnectPROCommon.SettingsTree.Zoom;

namespace ICD.Profound.ConnectPROCommon.SettingsTree
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
			yield return new TouchFreeSettingsNode();
			yield return new ZoomSettingsNode();
		}
	}
}
