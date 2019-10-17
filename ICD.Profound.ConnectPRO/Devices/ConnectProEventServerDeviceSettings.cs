using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.ConnectPRO.Devices
{
	[KrangSettings("ConnectProEventServer", typeof(ConnectProEventServerDevice))]
	public sealed class ConnectProEventServerDeviceSettings : AbstractDeviceSettings
	{
		private const string PORT_ELEMENT = "Port";

		public ushort Port { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Port = XmlUtils.TryReadChildElementContentAsUShort(xml, PORT_ELEMENT) ?? 0;
		}
	}
}
