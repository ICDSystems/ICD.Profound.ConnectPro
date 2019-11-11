using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.SettingsTree.CUE
{
	public sealed class CueSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public CueSettingsNode()
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
			yield return new BackgroundSettingsLeaf();
		}
	}
}
