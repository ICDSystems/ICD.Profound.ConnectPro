using System.Collections.Generic;
namespace ICD.Profound.ConnectPROCommon.SettingsTree.TouchFreeConferencing
{
	public sealed class TouchFreeSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public TouchFreeSettingsNode()
		{
			Name = "Touch Free";
			Icon = eSettingsIcon.TouchFree;
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new TouchFreeSettingsLeaf();
		}
	}
}