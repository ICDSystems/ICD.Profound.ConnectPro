using System;
using ICD.Common.Utils.Xml;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.ConnectPRO.Routing.Endpoints.Sources
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class ConnectProSourceSettings : AbstractSourceSettings
	{
		private const string FACTORY_NAME = "ConnectProSource";

		private const string ICON_ELEMENT = "Icon";
		private const string DESCRIPTION_ELEMENT = "Description";

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

		/// <summary>
		/// Gets/sets the description text.
		/// </summary>
		public string Description { get; set; }

		#endregion

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(ICON_ELEMENT, Icon);
			writer.WriteElementString(DESCRIPTION_ELEMENT, Description);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Icon = XmlUtils.TryReadChildElementContentAsString(xml, ICON_ELEMENT);
			Description = XmlUtils.TryReadChildElementContentAsString(xml, DESCRIPTION_ELEMENT);
		}
	}
}
