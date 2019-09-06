using System.Collections.Generic;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.SettingsTree.Administrative;
using ICD.Profound.ConnectPRO.SettingsTree.Conferencing;
using ICD.Profound.ConnectPRO.SettingsTree.CUE;
using ICD.Profound.ConnectPRO.SettingsTree.RoomCombine;

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
		}
	}
}
