using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Administrative
{
	public sealed class PowerSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public PowerSettingsLeaf(IConnectProRoom room)
			: base(room)
		{
			Name = "Power";
			Icon = SettingsTreeIcons.ICON_POWER;
		}
	}
}