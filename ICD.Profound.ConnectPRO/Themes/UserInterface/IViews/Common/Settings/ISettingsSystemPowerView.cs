using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings
{
	public interface ISettingsSystemPowerView : IUiView
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

		event EventHandler OnEnableButtonPressed;

		event EventHandler OnDisableButtonPressed;

		event EventHandler OnWakeButtonPressed;

		event EventHandler OnSleepButtonPressed;

		void SetWeekdaysButtonSelected(bool selected);

		void SetWeekendsButtonSelected(bool selected);

		void SetEnableButtonSelected(bool selected);

		void SetDisableButtonSelected(bool selected);

		void SetWakeHour(ushort hour);

		void SetWakeMinute(ushort minute);

		void SetSleepHour(ushort hour);

		void SetSleepMinute(ushort minute);

		void SetWakeButtonVisibility(bool visible);

		void SetSleepButtonVisibility(bool visible);
	}
}
