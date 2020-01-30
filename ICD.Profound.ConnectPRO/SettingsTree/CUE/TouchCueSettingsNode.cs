using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.SettingsTree.CUE
{
	public sealed class TouchCueSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public TouchCueSettingsNode()
		{
			Name = "TouchCUE";
			Icon = eSettingsIcon.Cue;
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
