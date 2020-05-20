using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Profound.ConnectPROCommon.Rooms.Single
{
	[KrangSettings("ConnectProRoom", typeof(ConnectProRoom))]
	public sealed class ConnectProRoomSettings : AbstractConnectProRoomSettings
	{
		private const string PASSCODE_ELEMENT = "Passcode";
		private const string CALENDAR_DEVICE_ELEMENT = "CalendarDevice";

		public string Passcode { get; set; }

		[OriginatorIdSettingsProperty(typeof(IDeviceBase))]
		public int? CalendarDevice { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PASSCODE_ELEMENT, Passcode);
			writer.WriteElementString(CALENDAR_DEVICE_ELEMENT, IcdXmlConvert.ToString(CalendarDevice));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Passcode = XmlUtils.TryReadChildElementContentAsString(xml, PASSCODE_ELEMENT);
			CalendarDevice = XmlUtils.TryReadChildElementContentAsInt(xml, CALENDAR_DEVICE_ELEMENT);
		}
	}
}
