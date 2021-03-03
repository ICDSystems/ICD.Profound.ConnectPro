using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.UnifyRooms.Devices.UnifyBar
{
	[KrangSettings("UnifyBar", typeof(UnifyBarDevice))]
	public sealed class UnifyBarDeviceSettings : AbstractDeviceSettings
	{
		private const string DEFAULT_MAKE = "Profound Technologies";
		private const string DEFAULT_MODEL = "Unify Bar";

		private const string ELEMENT_BUTTON_1 = "Button1";
		private const string ELEMENT_BUTTON_2 = "Button2";
		private const string ELEMENT_BUTTON_3 = "Button3";
		private const string ELEMENT_BUTTON_4 = "Button4";
		private const string ELEMENT_BUTTON_5 = "Button5";
		private const string ELEMENT_BUTTON_6 = "Button6";

		private const eMainButton DEFAULT_BUTTON_1 = eMainButton.RoomPower;
		private const eMainButton DEFAULT_BUTTON_2 = eMainButton.VolumeUp;
		private const eMainButton DEFAULT_BUTTON_3 = eMainButton.VolumeDown;
		private const eMainButton DEFAULT_BUTTON_4 = eMainButton.VolumeMute;
		private const eMainButton DEFAULT_BUTTON_5 = eMainButton.PrivacyMute;
		private const eMainButton DEFAULT_BUTTON_6 = eMainButton.ToggleLights;

		public eMainButton Button1 { get; set; }
		public eMainButton Button2 { get; set; }
		public eMainButton Button3 { get; set; }
		public eMainButton Button4 { get; set; }
		public eMainButton Button5 { get; set; }
		public eMainButton Button6 { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyBarDeviceSettings()
		{
			ConfiguredDeviceInfo.Make = DEFAULT_MAKE;
			ConfiguredDeviceInfo.Model = DEFAULT_MODEL;

			Button1 = DEFAULT_BUTTON_1;
			Button2 = DEFAULT_BUTTON_2;
			Button3 = DEFAULT_BUTTON_3;
			Button4 = DEFAULT_BUTTON_4;
			Button5 = DEFAULT_BUTTON_5;
			Button6 = DEFAULT_BUTTON_6;
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(ELEMENT_BUTTON_1, IcdXmlConvert.ToString(ELEMENT_BUTTON_1));
			writer.WriteElementString(ELEMENT_BUTTON_2, IcdXmlConvert.ToString(ELEMENT_BUTTON_2));
			writer.WriteElementString(ELEMENT_BUTTON_3, IcdXmlConvert.ToString(ELEMENT_BUTTON_3));
			writer.WriteElementString(ELEMENT_BUTTON_4, IcdXmlConvert.ToString(ELEMENT_BUTTON_4));
			writer.WriteElementString(ELEMENT_BUTTON_5, IcdXmlConvert.ToString(ELEMENT_BUTTON_5));
			writer.WriteElementString(ELEMENT_BUTTON_6, IcdXmlConvert.ToString(ELEMENT_BUTTON_6));
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

			Button1 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_1, true) ?? DEFAULT_BUTTON_1;
			Button2 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_2, true) ?? DEFAULT_BUTTON_2;
			Button3 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_3, true) ?? DEFAULT_BUTTON_3;
			Button4 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_4, true) ?? DEFAULT_BUTTON_4;
			Button5 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_5, true) ?? DEFAULT_BUTTON_5;
			Button6 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_6, true) ?? DEFAULT_BUTTON_6;
		}
	}
}
