using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Profound.ConnectPRO.Rooms.Single
{
	[KrangSettings("ConnectProRoom", typeof(ConnectProRoom))]
	public sealed class ConnectProRoomSettings : AbstractConnectProRoomSettings
	{
		private const string ATC_NUMBER_ELEMENT = "AtcNumber";
		private const string PASSCODE_ELEMENT = "Passcode";
		private const string WAKE_SCHEDULE_ELEMENT = "WakeSchedule";
		private const string CALENDAR_DEVICE_ELEMENT = "CalendarDevice";

		private readonly WakeSchedule m_WakeScheduleSettings;

		public string AtcNumber { get; set; }

		[HiddenSettingsProperty]
		public WakeSchedule WakeSchedule { get { return m_WakeScheduleSettings; } }

		[UsedImplicitly]
		public bool WeekdayEnable
		{
			get { return m_WakeScheduleSettings.WeekdayEnable; }
			set { m_WakeScheduleSettings.WeekdayEnable = value; }
		}

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
		public bool WeekendEnable
		{
			get { return m_WakeScheduleSettings.WeekendEnable; }
			set { m_WakeScheduleSettings.WeekendEnable = value; }
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

		[OriginatorIdSettingsProperty(typeof(IDeviceBase))]
		public int? CalendarDevice { get; set; }

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

			writer.WriteElementString(ATC_NUMBER_ELEMENT, AtcNumber);
			writer.WriteElementString(PASSCODE_ELEMENT, Passcode);
			writer.WriteElementString(CALENDAR_DEVICE_ELEMENT, IcdXmlConvert.ToString(CalendarDevice));

			m_WakeScheduleSettings.WriteElements(writer, WAKE_SCHEDULE_ELEMENT);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			AtcNumber = XmlUtils.TryReadChildElementContentAsString(xml, ATC_NUMBER_ELEMENT);
			Passcode = XmlUtils.TryReadChildElementContentAsString(xml, PASSCODE_ELEMENT);
			CalendarDevice = XmlUtils.TryReadChildElementContentAsInt(xml, CALENDAR_DEVICE_ELEMENT);

			string wakeScheduleXml;
			if (XmlUtils.TryGetChildElementAsString(xml, WAKE_SCHEDULE_ELEMENT, out wakeScheduleXml))
				m_WakeScheduleSettings.ParseXml(wakeScheduleXml);
		}
	}
}
