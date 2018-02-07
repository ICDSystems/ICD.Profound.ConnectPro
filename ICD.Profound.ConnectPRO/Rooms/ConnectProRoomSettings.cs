using System;
using ICD.Common.Utils.Xml;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public sealed class ConnectProRoomSettings : AbstractRoomSettings
	{
		private const string FACTORY_NAME = "ConnectProRoom";

		private const string DIALINGPLAN_ELEMENT = "DialingPlan";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(ConnectProRoom); } }

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
		/// Instantiates room settings from an xml element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[XmlFactoryMethod(FACTORY_NAME)]
		public static ConnectProRoomSettings FromXml(string xml)
		{
			string dialingPlan;
			XmlUtils.TryGetChildElementAsString(xml, DIALINGPLAN_ELEMENT, out dialingPlan);

			DialingPlanInfo dialingPlanInfo = string.IsNullOrEmpty(dialingPlan)
												  ? new DialingPlanInfo()
												  : DialingPlanInfo.FromXml(dialingPlan);

			ConnectProRoomSettings output = new ConnectProRoomSettings
			{
				DialingPlan = dialingPlanInfo
			};

			output.ParseXml(xml);
			return output;
		}
	}
}
