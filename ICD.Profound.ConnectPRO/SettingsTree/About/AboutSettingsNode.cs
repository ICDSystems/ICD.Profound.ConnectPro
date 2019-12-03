using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.SettingsTree.About
{
	public sealed class AboutSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public AboutSettingsNode()
		{
			Name = "About";
			Icon = SettingsTreeIcons.ICON_NOTIFICATION;
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new PluginsSettingsLeaf();
		}
	}
}
