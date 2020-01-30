using System;

namespace ICD.Profound.ConnectPRO.SettingsTree.Administrative
{
	public sealed class PinSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public PinSettingsLeaf()
		{
			Name = "PIN";
			Icon = eSettingsIcon.Pin;
		}

		/// <summary>
		/// Sets the pin.
		/// </summary>
		/// <param name="pin"></param>
		public void SetPin(string pin)
		{
			if (Room == null)
				throw new InvalidOperationException("No room assigned to node");

			if (pin == Room.Passcode)
				return;

			Room.Passcode = pin;

			SetDirty(true);
		}
	}
}
