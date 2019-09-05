using System.Collections.Generic;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Conferencing
{
	public sealed class ConferencingSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ConferencingSettingsNode(IConnectProRoom room)
			: base(room)
		{
			Name = "Conferencing";
			Icon = SettingsTreeIcons.ICON_CONFERENCE;
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new DirectorySettingsLeaf(Room);
		}
	}
}
