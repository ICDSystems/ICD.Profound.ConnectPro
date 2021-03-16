using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Profound.UnifyRooms.Devices.UnifyBar
{
	public sealed class UnifyBarButtonsConfiguration
	{
		private const string ELEMENT_BUTTON_1 = "Button1";
		private const string ELEMENT_BUTTON_2 = "Button2";
		private const string ELEMENT_BUTTON_3 = "Button3";
		private const string ELEMENT_BUTTON_4 = "Button4";
		private const string ELEMENT_BUTTON_5 = "Button5";
		private const string ELEMENT_BUTTON_6 = "Button6";

		#region Properties

		/// <summary>
		/// Gets/sets the first button type.
		/// </summary>
		public eMainButton Button1 { get; set; }

		/// <summary>
		/// Gets/sets the second button type.
		/// </summary>
		public eMainButton Button2 { get; set; }

		/// <summary>
		/// Gets/sets the third button type.
		/// </summary>
		public eMainButton Button3 { get; set; }

		/// <summary>
		/// Gets/sets the fourth button type.
		/// </summary>
		public eMainButton Button4 { get; set; }

		/// <summary>
		/// Gets/sets the fifth button type.
		/// </summary>
		public eMainButton Button5 { get; set; }

		/// <summary>
		/// Gets/sets the sixth button type.
		/// </summary>
		public eMainButton Button6 { get; set; }

		/// <summary>
		/// Gets the 6 button configurations.
		/// </summary>
		public IEnumerable<eMainButton> Buttons
		{
			get
			{
				yield return Button1;
				yield return Button2;
				yield return Button3;
				yield return Button4;
				yield return Button5;
				yield return Button6;
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyBarButtonsConfiguration()
		{
			Clear();
		}

		#region Methods

		/// <summary>
		/// Reverts the settings to defaults.
		/// </summary>
		public void Clear()
		{
			Button1 = eMainButton.RoomPower;
			Button2 = eMainButton.VolumeUp;
			Button3 = eMainButton.VolumeDown;
			Button4 = eMainButton.VolumeMute;
			Button5 = eMainButton.PrivacyMute;
			Button6 = eMainButton.ToggleLights;
		}

		/// <summary>
		/// Copies the values from the other instance.
		/// </summary>
		/// <param name="other"></param>
		public void Copy([NotNull] UnifyBarButtonsConfiguration other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			Button1 = other.Button1;
			Button2 = other.Button2;
			Button3 = other.Button3;
			Button4 = other.Button4;
			Button5 = other.Button5;
			Button6 = other.Button6;
		}

		/// <summary>
		/// Updates the settings from the given xml element.
		/// </summary>
		/// <param name="xml"></param>
		public void ParseXml(string xml)
		{
			Button1 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_1, true) ?? eMainButton.None;
			Button2 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_2, true) ?? eMainButton.None;
			Button3 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_3, true) ?? eMainButton.None;
			Button4 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_4, true) ?? eMainButton.None;
			Button5 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_5, true) ?? eMainButton.None;
			Button6 = XmlUtils.TryReadChildElementContentAsEnum<eMainButton>(xml, ELEMENT_BUTTON_6, true) ?? eMainButton.None;
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
				writer.WriteElementString(ELEMENT_BUTTON_1, IcdXmlConvert.ToString(Button1));
				writer.WriteElementString(ELEMENT_BUTTON_2, IcdXmlConvert.ToString(Button2));
				writer.WriteElementString(ELEMENT_BUTTON_3, IcdXmlConvert.ToString(Button3));
				writer.WriteElementString(ELEMENT_BUTTON_4, IcdXmlConvert.ToString(Button4));
				writer.WriteElementString(ELEMENT_BUTTON_5, IcdXmlConvert.ToString(Button5));
				writer.WriteElementString(ELEMENT_BUTTON_6, IcdXmlConvert.ToString(Button6));
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}
