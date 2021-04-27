using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Profound.UnifyRooms.MoreControls
{
	public sealed class MoreControlsPanelConfiguration
	{
		private const string ELEMENT_ENABLED = "Enabled";
		private const string ELEMENT_PATH = "Path";
		private const string ELEMENT_HOSTNAME = "Hostname";
		private const string ELEMENT_PORT = "Port";
		private const string ELEMENT_IPID = "Ipid";

		#region Properties

		/// <summary>
		/// Gets/sets the enabled state of the XPanel.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets/sets the path to the XPanel.
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Gets/sets the hostname.
		/// </summary>
		public string Hostname { get; set; }

		/// <summary>
		/// Gets/sets the port.
		/// </summary>
		public ushort Port { get; set; }

		/// <summary>
		/// Gets/sets the Ipid.
		/// </summary>
		public byte Ipid { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public MoreControlsPanelConfiguration()
		{
			Clear();
		}

		#region Methods

		/// <summary>
		/// Reverts the settings to defaults.
		/// </summary>
		public void Clear()
		{
			Enabled = false;
			Path = null;
			Hostname = null;
			Port = 41794;
			Ipid = 0x03;
		}

		/// <summary>
		/// Copies the values from the other instance.
		/// </summary>
		/// <param name="other"></param>
		public void Copy([NotNull] MoreControlsPanelConfiguration other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			Enabled = other.Enabled;
			Path = other.Path;
			Hostname = other.Hostname;
			Port = other.Port;
			Ipid = other.Ipid;
		}

		/// <summary>
		/// Updates the settings from the given xml element.
		/// </summary>
		/// <param name="xml"></param>
		public void ParseXml(string xml)
		{
			Enabled = XmlUtils.TryReadChildElementContentAsBoolean(xml, ELEMENT_ENABLED) ?? false;
			Path = XmlUtils.TryReadChildElementContentAsString(xml, ELEMENT_PATH);
			Hostname = XmlUtils.TryReadChildElementContentAsString(xml, ELEMENT_HOSTNAME);
			Port = XmlUtils.TryReadChildElementContentAsUShort(xml, ELEMENT_PORT) ?? 41794;
			Ipid = XmlUtils.TryReadChildElementContentAsByte(xml, ELEMENT_IPID) ?? 0x03;
		}

		/// <summary>
		/// Writes the current configuration to the given XML writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		public void ToXml(IcdXmlTextWriter writer, string element)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteStartElement(element);
			{
				writer.WriteElementString(ELEMENT_ENABLED, IcdXmlConvert.ToString(Enabled));
				writer.WriteElementString(ELEMENT_PATH, Path);
				writer.WriteElementString(ELEMENT_HOSTNAME, Hostname);
				writer.WriteElementString(ELEMENT_PORT, IcdXmlConvert.ToString(Port));
				writer.WriteElementString(ELEMENT_IPID, IcdXmlConvert.ToString(Ipid));
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}
