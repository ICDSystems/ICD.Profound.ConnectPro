using ICD.Common.Utils.Xml;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources
{
	[KrangSettings("ConnectProSource", typeof(ConnectProSource))]
	public sealed class ConnectProSourceSettings : AbstractSourceSettings
	{
		private const string ICON_ELEMENT = "Icon";
		private const string SHARE_ELEMENT = "Share";
		private const string CUE_NAME_ELEMENT = "CueName";
		private const string CONTROL_OVERRIDE_ELEMENT = "ControlOverride";
		private const string CONFERENCE_OVERRIDE_ELEMENT = "ConferenceOverride";

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
		/// Gets/sets the name of this source to be displayed on the Cue.
		/// Only unique names are displayed
		/// </summary>
		public string CueName { get; set; }

		/// <summary>
		/// Gets/sets the type of control to show when selecting the source in the UI.
		/// </summary>
		public eControlOverride ControlOverride { get; set; }

		/// <summary>
		/// Overrides the availability of privacy mute and camera features while the source is routed.
		/// </summary>
		public eConferenceOverride ConferenceOverride { get; set; }

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
			writer.WriteElementString(CUE_NAME_ELEMENT, CueName);
			writer.WriteElementString(CONTROL_OVERRIDE_ELEMENT, IcdXmlConvert.ToString(ControlOverride));
			writer.WriteElementString(CONFERENCE_OVERRIDE_ELEMENT, IcdXmlConvert.ToString(ConferenceOverride));
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
			CueName = XmlUtils.TryReadChildElementContentAsString(xml, CUE_NAME_ELEMENT);
			ControlOverride = XmlUtils.TryReadChildElementContentAsEnum<eControlOverride>(xml, CONTROL_OVERRIDE_ELEMENT, true) ??
			              eControlOverride.Default;
			ConferenceOverride =
				XmlUtils.TryReadChildElementContentAsEnum<eConferenceOverride>(xml, CONFERENCE_OVERRIDE_ELEMENT, true) ??
				eConferenceOverride.None;
		}
	}
}
