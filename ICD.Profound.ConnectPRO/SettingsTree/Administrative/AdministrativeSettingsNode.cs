using System.Collections.Generic;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Administrative
{
	public sealed class AdministrativeSettingsNode : AbstractStaticSettingsNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public AdministrativeSettingsNode(IConnectProRoom room)
			: base(room)
		{
			Name = "Administrative";
			Image = SettingsTreeImages.IMAGE_ADMIN;
			Icon = SettingsTreeIcons.ICON_ADMIN;
			Prompt = "Please choose an administrative option from the menu to the left.";
		}

		/// <summary>
		/// Override to initialize child items.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ISettingsNodeBase> BuildChildren()
		{
			yield return new ClockSettingsLeaf(Room);
			yield return new PinSettingsLeaf(Room);
			yield return new PowerSettingsLeaf(Room);
		}
	}
}
