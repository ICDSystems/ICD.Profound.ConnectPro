using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;
using ICD.Connect.Themes;

namespace ICD.Profound.ConnectPRO.Themes
{
	[KrangSettings("ConnectProTheme", typeof(ConnectProTheme))]
	public sealed class ConnectProThemeSettings : AbstractThemeSettings
	{
		private const string LOGO_ELEMENT = "Logo";
		private const string TVPRESETS_ELEMENT = "TvPresets";
		private const string WEB_CONFERENCING_INSTRUCTIONS_ELEMENT = "WebConferencing";
		private const string CUE_BACKGROUND_ELEMENT = "CueBackground";

		public string Logo { get; set; }

		[PathSettingsProperty("TvPresets", ".xml")]
		public string TvPresets { get; set; }

		[PathSettingsProperty("WebConferencing", ".xml")]
		public string WebConferencingInstructions { get; set; }

		public eCueBackgroundMode CueBackground { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProThemeSettings()
		{
			Logo = ConnectProTheme.LOGO_DEFAULT;
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(LOGO_ELEMENT, Logo);
			writer.WriteElementString(TVPRESETS_ELEMENT, TvPresets);
			writer.WriteElementString(WEB_CONFERENCING_INSTRUCTIONS_ELEMENT, WebConferencingInstructions);
			writer.WriteElementString(CUE_BACKGROUND_ELEMENT, IcdXmlConvert.ToString(CueBackground));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Logo = XmlUtils.TryReadChildElementContentAsString(xml, LOGO_ELEMENT) ?? ConnectProTheme.LOGO_DEFAULT;
			TvPresets = XmlUtils.TryReadChildElementContentAsString(xml, TVPRESETS_ELEMENT);
			WebConferencingInstructions = XmlUtils.TryReadChildElementContentAsString(xml, WEB_CONFERENCING_INSTRUCTIONS_ELEMENT);
			CueBackground = XmlUtils.TryReadChildElementContentAsEnum<eCueBackgroundMode>(xml, CUE_BACKGROUND_ELEMENT, true) ?? eCueBackgroundMode.Neutral;
		}
	}

	/// <summary>
	/// Describes which mode the CUE background should use.
	/// </summary>
	public enum eCueBackgroundMode : ushort
	{
		/// <summary>
		/// Default mode, neutral/business-friendly CUE background, never changes
		/// </summary>
		Neutral = 0,
		/// <summary>
		/// New background each month, themed for month (US themes)
		/// </summary>
		Monthly = 1,
	}
}
