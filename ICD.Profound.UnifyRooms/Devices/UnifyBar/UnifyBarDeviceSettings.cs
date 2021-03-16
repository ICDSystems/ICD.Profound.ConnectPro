using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.UnifyRooms.Devices.UnifyBar
{
	[KrangSettings("UnifyBar", typeof(UnifyBarDevice))]
	public sealed class UnifyBarDeviceSettings : AbstractDeviceSettings
	{
		private const string DEFAULT_MAKE = "Profound Technologies";
		private const string DEFAULT_MODEL = "Unify Bar";

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyBarDeviceSettings()
		{
			ConfiguredDeviceInfo.Make = DEFAULT_MAKE;
			ConfiguredDeviceInfo.Model = DEFAULT_MODEL;
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			ConfiguredDeviceInfo.Make = string.IsNullOrEmpty(ConfiguredDeviceInfo.Make)
				? DEFAULT_MAKE
				: ConfiguredDeviceInfo.Make;

			ConfiguredDeviceInfo.Model = string.IsNullOrEmpty(ConfiguredDeviceInfo.Model)
				? DEFAULT_MODEL
				: ConfiguredDeviceInfo.Model;
		}
	}
}
