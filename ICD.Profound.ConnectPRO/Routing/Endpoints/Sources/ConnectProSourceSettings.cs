using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.ConnectPRO.Routing.Endpoints.Sources
{
	public sealed class ConnectProSourceSettings : AbstractSourceSettings
	{
		private const string FACTORY_NAME = "ConnectProSource";

		private const string ICON_ELEMENT = "Icon";

		#region Properties

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(ConnectProSource); } }

		/// <summary>
		/// Gets/sets the icon serial.
		/// </summary>
		public string Icon { get; set; }

		#endregion

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(ICON_ELEMENT, Icon);
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static ConnectProSourceSettings FromXml(string xml)
		{
			ConnectProSourceSettings output = new ConnectProSourceSettings
			{
				Icon = XmlUtils.TryReadChildElementContentAsString(xml, ICON_ELEMENT)
			};

			output.ParseXml(xml);
			return output;
		}
	}
}
