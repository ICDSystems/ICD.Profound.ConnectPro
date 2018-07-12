using System;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Scheduler;
using ICD.Common.Utils.Xml;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public sealed class WakeSchedule : AbstractScheduledAction
	{
		public event EventHandler WakeActionRequested;
		public event EventHandler SleepActionRequested;

		private const string WEEKDAY_ELEMENT = "Weekday";
		private const string WEEKEND_ELEMENT = "Weekend";

		private const string WAKE_ELEMENT = "Wake";
		private const string SLEEP_ELEMENT = "Sleep";

		private TimeSpan? _weekdayWakeTime;
		private TimeSpan? _weekdaySleepTime;
		private TimeSpan? _weekendWakeTime;
		private TimeSpan? _weekendSleepTime;
		
		#region Properties

		/// <summary>
		/// Gets/sets the Weekday Wake Time.
		/// </summary>
		public TimeSpan? WeekdayWakeTime
		{
			get { return _weekdayWakeTime; }
			set
			{ 
				_weekdayWakeTime = value;
				UpdateNextRunTime();
			}
		}

		/// <summary>
		/// Gets/sets the Weekday Sleep Time.
		/// </summary>
		public TimeSpan? WeekdaySleepTime
		{
			get { return _weekdaySleepTime; }
			set
			{
				_weekdaySleepTime = value;
				UpdateNextRunTime();
			}
		}

		/// <summary>
		/// Gets/sets the Weekend Wake Time.
		/// </summary>
		public TimeSpan? WeekendWakeTime
		{
			get { return _weekendWakeTime; }
			set
			{
				_weekendWakeTime = value;
				UpdateNextRunTime();
			}
		}

		/// <summary>
		/// Gets/sets the Weekend Sleep Time.
		/// </summary>
		public TimeSpan? WeekendSleepTime
		{
			get { return _weekendSleepTime; }
			set
			{
				_weekendSleepTime = value;
				UpdateNextRunTime();
			}
		}

		#endregion

		#region Methods

		public override void RunFinal()
		{
			var now = DateTime.Now;
			var wakeTime = GetWakeTimeForDay(DateTime.Today);
			var sleepTime = GetSleepTimeForDay(DateTime.Today);

			if (sleepTime != null && wakeTime != null)
			{
				var time = now.PreviousLatestTime(sleepTime.Value, wakeTime.Value);
				if (time != null && time == sleepTime.Value)
					Sleep();
				else if (time != null && time == wakeTime.Value)
					Wake();
			}
			else if (wakeTime != null && now > wakeTime)
				Wake();
			else if (sleepTime != null && now > sleepTime)
				Sleep();
		}

		public override DateTime? GetNextRunTime()
		{
			var now = DateTime.Now;
			var currentDay = DateTime.Today;
			while (currentDay < now.AddDays(7)) // if no action found for a week, means all 4 times are null
			{
				var sleepTime = GetSleepTimeForDay(currentDay);
				var wakeTime = GetWakeTimeForDay(currentDay);

				if (sleepTime != null && wakeTime != null)
				{
					var time = now.NextEarliestTime(sleepTime.Value, wakeTime.Value);
					if (time != null)
						return time;
				}
				else if (wakeTime != null && now < wakeTime)
					return wakeTime.Value;
				else if (sleepTime != null && now < sleepTime)
					return sleepTime.Value;

				currentDay = currentDay.AddDays(1); // no actions scheduled for today, check next day
			}

			return null;
		}

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

		private DateTime? GetSleepTimeForDay(DateTime day)
		{
			day = day.Date; // remove any time info
			if (day.DayOfWeek.IsWeekday() && WeekdaySleepTime != null)
				return day.Add(WeekdaySleepTime.Value);
			else if (day.DayOfWeek.IsWeekend() && WeekendSleepTime != null)
				return day.Add(WeekendSleepTime.Value);

			return null; // should not sleep today
		}

		private DateTime? GetWakeTimeForDay(DateTime day)
		{
			day = day.Date; // remove any time info
			if (day.DayOfWeek.IsWeekday() && WeekdayWakeTime != null)
				return day.Add(WeekdayWakeTime.Value);
			else if (day.DayOfWeek.IsWeekend() && WeekendWakeTime != null)
				return day.Add(WeekendWakeTime.Value);

			return null; // should not wake today
		}

		private void Wake()
		{
			WakeActionRequested.Raise(this);
		}

		private void Sleep()
		{
			SleepActionRequested.Raise(this);
		}

		#endregion
	}
}
