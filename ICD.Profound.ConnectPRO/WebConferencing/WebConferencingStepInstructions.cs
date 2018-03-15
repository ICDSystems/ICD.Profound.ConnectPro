using ICD.Common.Utils.Xml;

namespace ICD.Profound.ConnectPRO.WebConferencing
{
	public sealed class WebConferencingStepInstructions
	{
		public string Image { get; set; }
		public string Text { get; set; }

		public static WebConferencingStepInstructions FromXml(string baseUrl, string xml)
		{
			return new WebConferencingStepInstructions
			{
				Image = (baseUrl ?? string.Empty) + XmlUtils.TryReadChildElementContentAsString(xml, "Image"),
				Text = XmlUtils.TryReadChildElementContentAsString(xml, "Text")
			};
		}
	}
}
