using System.Collections.Generic;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.About
{
	public sealed class AboutSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public AboutSettingsNode()
		{
			Name = "About";
			Icon = eSettingsIcon.Notifications;
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
