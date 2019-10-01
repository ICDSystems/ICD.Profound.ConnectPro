using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Profound.ConnectPRO.Rooms
{
	[KrangSettings("ConnectProRoom", typeof(ConnectProRoom))]
	public sealed class ConnectProRoomSettings : AbstractCommercialRoomSettings
	{
		private const string ATC_NUMBER_ELEMENT = "AtcNumber";
		private const string PASSCODE_ELEMENT = "Passcode";
		private const string CALENDAR_DEVICE_ELEMENT = "CalendarDevice";

		public string AtcNumber { get; set; }

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

			writer.WriteElementString(ATC_NUMBER_ELEMENT, AtcNumber);
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

			AtcNumber = XmlUtils.TryReadChildElementContentAsString(xml, ATC_NUMBER_ELEMENT);
			Passcode = XmlUtils.TryReadChildElementContentAsString(xml, PASSCODE_ELEMENT);
			CalendarDevice = XmlUtils.TryReadChildElementContentAsInt(xml, CALENDAR_DEVICE_ELEMENT);
		}
	}
}
