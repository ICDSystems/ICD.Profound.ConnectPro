using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Administrative
{
	public sealed class PinSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public PinSettingsLeaf(IConnectProRoom room)
			: base(room)
		{
			Name = "PIN";
			Icon = SettingsTreeIcons.ICON_PIN;
		}

		/// <summary>
		/// Sets the pin.
		/// </summary>
		/// <param name="pin"></param>
		public void SetPin(string pin)
		{
			if (pin == Room.Passcode)
				return;

			Room.Passcode = pin;

			SetDirty(true);
		}
	}
}