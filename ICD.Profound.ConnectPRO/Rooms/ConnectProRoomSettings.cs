using ICD.Common.Utils.Xml;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.ConnectPRO.Rooms
{
	[KrangSettings("ConnectProRoom", typeof(ConnectProRoom))]
	public sealed class ConnectProRoomSettings : AbstractRoomSettings
	{
		private const string DIALINGPLAN_ELEMENT = "DialingPlan";

		public DialingPlanInfo DialingPlan { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			DialingPlan.WriteToXml(writer, DIALINGPLAN_ELEMENT);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			string dialingPlan;
			XmlUtils.TryGetChildElementAsString(xml, DIALINGPLAN_ELEMENT, out dialingPlan);

			DialingPlan = string.IsNullOrEmpty(dialingPlan)
				              ? new DialingPlanInfo()
				              : DialingPlanInfo.FromXml(dialingPlan);
		}
	}
}
