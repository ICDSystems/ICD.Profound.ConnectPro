using System;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings.Administrative
{
	public interface ISettingsClockView : ITouchDisplayView
	{
		event EventHandler On24HourTogglePressed;
		event EventHandler OnAmPmTogglePressed;

		event EventHandler OnHourUpButtonPressed;
		event EventHandler OnHourDownButtonPressed;
		event EventHandler OnMinuteUpButtonPressed;
		event EventHandler OnMinuteDownButtonPressed;

		void Set24HourMode(bool selected);
		void SetAm(bool am);
		void SetHour(int hour);
		void SetMinute(int minute);
	}
}
