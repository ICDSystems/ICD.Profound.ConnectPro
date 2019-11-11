using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.SettingsTree.Conferencing
{
	public sealed class ConferencingSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ConferencingSettingsNode()
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
			yield return new DirectorySettingsLeaf();
		}
	}
}
