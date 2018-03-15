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
		private const string SHARE_ELEMENT = "Share";
		private const string HIDE_ELEMENT = "Hide";
		private const string DESCRIPTION_ELEMENT = "Description";
		private const string CONTROL_OVERRIDE_ELEMENT = "ControlOverride";

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
		/// Gets/sets if the source can be shared.
		/// </summary>
		public bool Share { get; set; }

		/// <summary>
		/// Gets/sets if the source should be hidden from the source list.
		/// </summary>
		public bool Hide { get; set; }

		/// <summary>
		/// Gets/sets the description text.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets/sets the type of control to show when selecting the source in the UI.
		/// </summary>
		public eControlOverride ControlOverride { get; set; }

		#endregion

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(ICON_ELEMENT, Icon);
			writer.WriteElementString(SHARE_ELEMENT, IcdXmlConvert.ToString(Share));
			writer.WriteElementString(HIDE_ELEMENT, IcdXmlConvert.ToString(Hide));
			writer.WriteElementString(DESCRIPTION_ELEMENT, Description);
			writer.WriteElementString(CONTROL_OVERRIDE_ELEMENT, IcdXmlConvert.ToString(ControlOverride));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Icon = XmlUtils.TryReadChildElementContentAsString(xml, ICON_ELEMENT);
			Share = XmlUtils.TryReadChildElementContentAsBoolean(xml, SHARE_ELEMENT) ?? false;
			Hide = XmlUtils.TryReadChildElementContentAsBoolean(xml, HIDE_ELEMENT) ?? false;
			Description = XmlUtils.TryReadChildElementContentAsString(xml, DESCRIPTION_ELEMENT);
			ControlOverride = XmlUtils.TryReadChildElementContentAsEnum<eControlOverride>(xml, CONTROL_OVERRIDE_ELEMENT, true) ??
			              eControlOverride.Default;
		}
	}
}
