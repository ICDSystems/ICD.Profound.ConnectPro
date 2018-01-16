﻿using ICD.Common.Utils.Xml;
using ICD.Connect.Devices.Controls;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public struct DialingPlanInfo
	{
		private const string AUDIO_ENDPOINT_ELEMENT = "AudioEndpoint";
		private const string VIDEO_ENDPOINT_ELEMENT = "VideoEndpoint";

		private const string DEVICE_ELEMENT = "Device";
		private const string CONTROL_ELEMENT = "Control";

		private readonly DeviceControlInfo m_VideoEndpoint;
		private readonly DeviceControlInfo m_AudioEndpoint;

		#region Properties

		/// <summary>
		/// Gets the video conferencing endpoint info.
		/// </summary>
		public DeviceControlInfo VideoEndpoint { get { return m_VideoEndpoint; } }

		/// <summary>
		/// Gets the audio conferencing endpoint info.
		/// </summary>
		public DeviceControlInfo AudioEndpoint { get { return m_AudioEndpoint; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="videoEndpoint"></param>
		/// <param name="audioEndpoint"></param>
		public DialingPlanInfo(DeviceControlInfo videoEndpoint, DeviceControlInfo audioEndpoint)
		{
			m_VideoEndpoint = videoEndpoint;
			m_AudioEndpoint = audioEndpoint;
		}

		#region Methods

		/// <summary>
		/// Parses the DialingPlanInfo from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static DialingPlanInfo FromXml(string xml)
		{
			DeviceControlInfo video;
			DeviceControlInfo audio;

			// Migrate an old DialingPlan that only had the config path
			if (XmlUtils.HasChildElements(xml))
			{
				video = ReadDeviceControlInfo(xml, VIDEO_ENDPOINT_ELEMENT);
				audio = ReadDeviceControlInfo(xml, AUDIO_ENDPOINT_ELEMENT);
			}
			else
			{
				video = default(DeviceControlInfo);
				audio = default(DeviceControlInfo);
			}

			return new DialingPlanInfo(video, audio);
		}

		/// <summary>
		/// Writes the DialingPlanInfo to XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		public void WriteToXml(IcdXmlTextWriter writer, string element)
		{
			writer.WriteStartElement(element);
			{
				WriteDeviceControlInfo(writer, m_VideoEndpoint, VIDEO_ENDPOINT_ELEMENT);
				WriteDeviceControlInfo(writer, m_AudioEndpoint, AUDIO_ENDPOINT_ELEMENT);
			}
			writer.WriteEndElement();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Reads the child element from the xml as a DeviceControlInfo. 
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		private static DeviceControlInfo ReadDeviceControlInfo(string xml, string element)
		{
			if (!XmlUtils.TryGetChildElementAsString(xml, element, out xml))
				return default(DeviceControlInfo);

			int device = XmlUtils.TryReadChildElementContentAsInt(xml, DEVICE_ELEMENT) ?? 0;
			int control = XmlUtils.TryReadChildElementContentAsInt(xml, CONTROL_ELEMENT) ?? 0;

			return new DeviceControlInfo(device, control);
		}

		/// <summary>
		/// Writes the DeviceControlInfo to xml under the given element name.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="control"></param>
		/// <param name="element"></param>
		private static void WriteDeviceControlInfo(IcdXmlTextWriter writer, DeviceControlInfo control, string element)
		{
			writer.WriteStartElement(element);
			{
				writer.WriteElementString(DEVICE_ELEMENT, IcdXmlConvert.ToString(control.DeviceId));
				writer.WriteElementString(CONTROL_ELEMENT, IcdXmlConvert.ToString(control.ControlId));
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}
