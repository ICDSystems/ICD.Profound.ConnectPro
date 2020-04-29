using System.Collections.Generic;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.Administrative
{
	public sealed class AdministrativeSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public AdministrativeSettingsNode()
		{
			Name = "Administrative";
			Image = SettingsTreeImages.IMAGE_ADMIN;
			Icon = eSettingsIcon.Admin;
			Prompt = "Please choose an administrative option from the menu to the left.";
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new ClockSettingsLeaf();
			yield return new PinSettingsLeaf();
			yield return new WakeSleepSettingsLeaf();
		}
	}
}
