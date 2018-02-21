using System;
using ICD.Common.Utils.Xml;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings.Attributes;

namespace ICD.Profound.ConnectPRO.Rooms
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class ConnectProRoomSettings : AbstractRoomSettings
	{
		private const string FACTORY_NAME = "ConnectProRoom";

		private const string DIALINGPLAN_ELEMENT = "DialingPlan";
		private const string VOLUMEPOINT_ELEMENT = "VolumePoint";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(ConnectProRoom); } }

		public DialingPlanInfo DialingPlan { get; set; }

		public VolumePoint VolumePoint { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			DialingPlan.WriteToXml(writer, DIALINGPLAN_ELEMENT);

			if (VolumePoint != null)
				VolumePoint.WriteToXml(writer, VOLUMEPOINT_ELEMENT);
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

			string volumePoint;
			XmlUtils.TryGetChildElementAsString(xml, VOLUMEPOINT_ELEMENT, out volumePoint);

			DialingPlan = string.IsNullOrEmpty(dialingPlan)
				              ? new DialingPlanInfo()
				              : DialingPlanInfo.FromXml(dialingPlan);

			VolumePoint = string.IsNullOrEmpty(volumePoint)
				              ? null
				              : VolumePoint.FromXml(volumePoint);
		}
	}
}
