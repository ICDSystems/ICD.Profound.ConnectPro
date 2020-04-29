using System;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.Administrative
{
	public interface ISettingsPowerView : ITouchDisplayView
	{
		event EventHandler OnWeekdaysButtonPressed;
		event EventHandler OnWeekendsButtonPressed;

		event EventHandler OnWakeHourIncrementButtonPressed;
		event EventHandler OnWakeHourDecrementButtonPressed;
		event EventHandler OnWakeMinuteIncrementButtonPressed;
		event EventHandler OnWakeMinuteDecrementButtonPressed;

		event EventHandler OnSleepHourIncrementButtonPressed;
		event EventHandler OnSleepHourDecrementButtonPressed;
		event EventHandler OnSleepMinuteIncrementButtonPressed;
		event EventHandler OnSleepMinuteDecrementButtonPressed;

		event EventHandler OnEnableTogglePressed;
		event EventHandler OnSystemPowerPressed;

		void SetWeekdaysButtonSelected();
		void SetWeekendsButtonSelected();

		void SetEnableToggleSelected(bool selected);

		void SetWakeHour(int hour);
		void SetWakeMinute(int minute);

		void SetSleepHour(int hour);
		void SetSleepMinute(int minute);
	}
}
