using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;
using ICD.Connect.Themes;

namespace ICD.Profound.ConnectPRO.Themes
{
	[KrangSettings("ConnectProTheme", typeof(ConnectProTheme))]
	public sealed class ConnectProThemeSettings : AbstractThemeSettings
	{
		private const string TVPRESETS_ELEMENT = "TvPresets";
		private const string WEB_CONFERENCING_INSTRUCTIONS_ELEMENT = "WebConferencing";

		[PathSettingsProperty("TvPresets", ".xml")]
		public string TvPresets { get; set; }

		[PathSettingsProperty("WebConferencing", ".xml")]
		public string WebConferencingInstructions { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(TVPRESETS_ELEMENT, TvPresets);
			writer.WriteElementString(WEB_CONFERENCING_INSTRUCTIONS_ELEMENT, WebConferencingInstructions);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			TvPresets = XmlUtils.TryReadChildElementContentAsString(xml, TVPRESETS_ELEMENT);
			WebConferencingInstructions = XmlUtils.TryReadChildElementContentAsString(xml, WEB_CONFERENCING_INSTRUCTIONS_ELEMENT);
		}
	}
}
