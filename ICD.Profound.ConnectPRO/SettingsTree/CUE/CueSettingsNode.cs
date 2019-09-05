using System.Collections.Generic;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.CUE
{
	public sealed class CueSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public CueSettingsNode(IConnectProRoom room)
			: base(room)
		{
			Name = "CUE";
			Icon = SettingsTreeIcons.ICON_CUE;
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new BackgroundSettingsLeaf(Room);
		}
	}
}
