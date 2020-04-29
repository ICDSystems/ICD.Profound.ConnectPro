using System;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.Administrative
{
	public interface ISettingsClockView : ITouchDisplayView
	{
		event EventHandler OnDayUpButtonPressed;
		event EventHandler OnDayDownButtonPressed;
		event EventHandler OnMonthUpButtonPressed;
		event EventHandler OnMonthDownButtonPressed;
		event EventHandler OnYearUpButtonPressed;
		event EventHandler OnYearDownButtonPressed;

		event EventHandler On24HourTogglePressed;
		event EventHandler OnAmPmTogglePressed;

		event EventHandler OnHourUpButtonPressed;
		event EventHandler OnHourDownButtonPressed;
		event EventHandler OnMinuteUpButtonPressed;
		event EventHandler OnMinuteDownButtonPressed;

		
		void SetDay(int day);
		void SetMonth(int month);
		void SetYear(int year);
		void Set24HourMode(bool selected);
		void SetAm(bool am);
		void SetHour(int hour);
		void SetMinute(int minute);
	}
}
