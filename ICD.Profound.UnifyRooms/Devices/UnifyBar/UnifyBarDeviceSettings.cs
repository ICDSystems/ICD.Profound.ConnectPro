using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.UnifyRooms.Devices.UnifyBar
{
	[KrangSettings("UnifyBar", typeof(UnifyBarDevice))]
	public sealed class UnifyBarDeviceSettings : AbstractDeviceSettings
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyBarDeviceSettings()
		{
			ConfiguredDeviceInfo.Make = "Profound Technologies";
			ConfiguredDeviceInfo.Model = "Unify Bar";
		}
	}
}
