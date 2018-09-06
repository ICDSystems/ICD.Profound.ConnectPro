using System;
using ICD.Common.Utils.Xml;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Profound.ConnectPRO.Rooms
{
	[KrangSettings("ConnectProRoom", typeof(ConnectProRoom))]
	public sealed class ConnectProRoomSettings : AbstractRoomSettings
	{
		private const string PASSCODE_ELEMENT = "Passcode";
		private const string WAKE_SCHEDULE_ELEMENT = "WakeSchedule";

		private readonly WakeSchedule m_WakeScheduleSettings;

		[HiddenSettingsProperty]
		public WakeSchedule WakeSchedule { get { return m_WakeScheduleSettings; } }

		[UsedImplicitly]
		public TimeSpan? WeekdayWakeTime
		{
			get { return m_WakeScheduleSettings.WeekdayWakeTime; }
			set { m_WakeScheduleSettings.WeekdayWakeTime = value; }
		}

		[UsedImplicitly]
		public TimeSpan? WeekdaySleepTime
		{
			get { return m_WakeScheduleSettings.WeekdaySleepTime; }
			set { m_WakeScheduleSettings.WeekdaySleepTime = value; }
		}

		[UsedImplicitly]
		public TimeSpan? WeekendWakeTime
		{
			get { return m_WakeScheduleSettings.WeekendWakeTime; }
			set { m_WakeScheduleSettings.WeekendWakeTime = value; }
		}

		[UsedImplicitly]
		public TimeSpan? WeekendSleepTime
		{
			get { return m_WakeScheduleSettings.WeekendSleepTime; }
			set { m_WakeScheduleSettings.WeekendSleepTime = value; }
		}

		public string Passcode { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProRoomSettings()
		{
			m_WakeScheduleSettings = new WakeSchedule();
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PASSCODE_ELEMENT, Passcode);

			m_WakeScheduleSettings.WriteElements(writer, WAKE_SCHEDULE_ELEMENT);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Passcode = XmlUtils.TryReadChildElementContentAsString(xml, PASSCODE_ELEMENT);

			string wakeScheduleXml;
			if (XmlUtils.TryGetChildElementAsString(xml, WAKE_SCHEDULE_ELEMENT, out wakeScheduleXml))
				m_WakeScheduleSettings.ParseXml(wakeScheduleXml);
		}
	}
}
