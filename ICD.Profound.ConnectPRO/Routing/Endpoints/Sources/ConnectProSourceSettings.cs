using ICD.Common.Utils.Xml;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.ConnectPRO.Routing.Endpoints.Sources
{
	[KrangSettings("ConnectProSource", typeof(ConnectProSource))]
	public sealed class ConnectProSourceSettings : AbstractSourceSettings
	{
		private const string ICON_ELEMENT = "Icon";
		private const string SHARE_ELEMENT = "Share";
		private const string CONTROL_OVERRIDE_ELEMENT = "ControlOverride";

		#region Properties

		/// <summary>
		/// Gets/sets the icon serial.
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// Gets/sets if the source can be shared.
		/// </summary>
		public bool Share { get; set; }

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
			ControlOverride = XmlUtils.TryReadChildElementContentAsEnum<eControlOverride>(xml, CONTROL_OVERRIDE_ELEMENT, true) ??
			              eControlOverride.Default;
		}
	}
}
