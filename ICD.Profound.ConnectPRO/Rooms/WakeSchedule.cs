using System;
using ICD.Common.Utils.Xml;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public sealed class WakeSchedule
	{
		private const string WEEKDAY_ELEMENT = "Weekday";
		private const string WEEKEND_ELEMENT = "Weekend";

		private const string WAKE_ELEMENT = "Wake";
		private const string SLEEP_ELEMENT = "Sleep";

		#region Properties

		/// <summary>
		/// Gets/sets the Weekday Wake Time in UTC.
		/// </summary>
		public TimeSpan? WeekdayWakeTime { get; set; }

		/// <summary>
		/// Gets/sets the Weekday Sleep Time in UTC.
		/// </summary>
		public TimeSpan? WeekdaySleepTime { get; set; }

		/// <summary>
		/// Gets/sets the Weekend Wake Time in UTC.
		/// </summary>
		public TimeSpan? WeekendWakeTime { get; set; }

		/// <summary>
		/// Gets/sets the Weekend Sleep Time in UTC.
		/// </summary>
		public TimeSpan? WeekendSleepTime { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Clears the stored times.
		/// </summary>
		public void Clear()
		{
			WeekdayWakeTime = null;
			WeekdaySleepTime = null;
			WeekendWakeTime = null;
			WeekendSleepTime = null;
		}

		/// <summary>
		/// Copies the properties from the other instance.
		/// </summary>
		/// <param name="other"></param>
		public void Copy(WakeSchedule other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			WeekdayWakeTime = other.WeekdayWakeTime;
			WeekdaySleepTime = other.WeekdaySleepTime;
			WeekendWakeTime = other.WeekendWakeTime;
			WeekendSleepTime = other.WeekendSleepTime;
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public void ParseXml(string xml)
		{
			Clear();

			string weekdayXml;
			if (XmlUtils.TryGetChildElementAsString(xml, WEEKDAY_ELEMENT, out weekdayXml))
				ParseWeekday(weekdayXml);

			string weekendXml;
			if (XmlUtils.TryGetChildElementAsString(xml, WEEKEND_ELEMENT, out weekendXml))
				ParseWeekend(weekendXml);
		}

		/// <summary>
		/// Writes the settings to xml.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		public void WriteElements(IcdXmlTextWriter writer, string element)
		{
			writer.WriteStartElement(element);
			{
				writer.WriteStartElement(WEEKDAY_ELEMENT);
				{
					writer.WriteElementString(WAKE_ELEMENT, WeekdayWakeTime == null ? null : IcdXmlConvert.ToString(WeekdayWakeTime));
					writer.WriteElementString(SLEEP_ELEMENT, WeekdaySleepTime == null ? null : IcdXmlConvert.ToString(WeekdaySleepTime));
				}
				writer.WriteEndElement();

				writer.WriteStartElement(WEEKEND_ELEMENT);
				{
					writer.WriteElementString(WAKE_ELEMENT, WeekendWakeTime == null ? null : IcdXmlConvert.ToString(WeekendWakeTime));
					writer.WriteElementString(SLEEP_ELEMENT, WeekendSleepTime == null ? null : IcdXmlConvert.ToString(WeekendSleepTime));
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the weekday settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		private void ParseWeekday(string xml)
		{
			string wakeXml;
			if (XmlUtils.TryGetChildElementAsString(xml, WAKE_ELEMENT, out wakeXml))
				WeekdayWakeTime = ParseTimeSpan(wakeXml);

			string sleepXml;
			if (XmlUtils.TryGetChildElementAsString(xml, SLEEP_ELEMENT, out sleepXml))
				WeekdaySleepTime = ParseTimeSpan(sleepXml);
		}

		/// <summary>
		/// Updates the weekend settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		private void ParseWeekend(string xml)
		{
			string wakeXml;
			if (XmlUtils.TryGetChildElementAsString(xml, WAKE_ELEMENT, out wakeXml))
				WeekendWakeTime = ParseTimeSpan(wakeXml);

			string sleepXml;
			if (XmlUtils.TryGetChildElementAsString(xml, SLEEP_ELEMENT, out sleepXml))
				WeekendSleepTime = ParseTimeSpan(sleepXml);
		}

		/// <summary>
		/// Parses the xml content as a TimeSpan.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		private static TimeSpan? ParseTimeSpan(string xml)
		{
			string content = XmlUtils.ReadElementContent(xml);
			if (string.IsNullOrEmpty(content))
				return null;

			return TimeSpan.Parse(content);
		}

		#endregion
	}
}
