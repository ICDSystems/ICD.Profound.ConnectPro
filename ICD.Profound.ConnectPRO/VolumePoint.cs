using ICD.Common.Utils.Xml;

namespace ICD.Profound.ConnectPRO
{
	public sealed class VolumePoint
	{
		private const string DEVICE_ELEMENT = "Device";
		private const string CONTROL_ELEMENT = "Control";

		/// <summary>
		/// Device id
		/// </summary>
		public int DeviceId { get; set; }

		/// <summary>
		/// Control id.
		/// </summary>
		public int ControlId { get; set; }

		/// <summary>
		/// Creates a VolumePoint.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static VolumePoint FromXml(string xml)
		{
			int deviceId = XmlUtils.TryReadChildElementContentAsInt(xml, DEVICE_ELEMENT) ?? 0;
			int controlId = XmlUtils.TryReadChildElementContentAsInt(xml, CONTROL_ELEMENT) ?? 0;

			return new VolumePoint
			{
				DeviceId = deviceId,
				ControlId = controlId
			};
		}

		/// <summary>
		/// Output element as XML to the IcdXmlTextWriter.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="elementName"></param>
		public void WriteToXml(IcdXmlTextWriter writer, string elementName)
		{
			writer.WriteStartElement(elementName);
			{
				writer.WriteElementString(DEVICE_ELEMENT, IcdXmlConvert.ToString(DeviceId));
				writer.WriteElementString(CONTROL_ELEMENT, IcdXmlConvert.ToString(ControlId));
			}
			writer.WriteEndElement();
		}
	}
}
