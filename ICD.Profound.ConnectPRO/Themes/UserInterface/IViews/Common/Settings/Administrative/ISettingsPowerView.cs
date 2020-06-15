using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Administrative
{
	public interface ISettingsPowerView : IUiView
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

		event EventHandler OnIncrementDecrementButtonReleased;

		event EventHandler OnDisplayPowerTogglePressed;
		event EventHandler OnEnableWakeTogglePressed;
		event EventHandler OnEnableSleepTogglePressed;

		void SetWeekdaysButtonSelected(bool selected);
		void SetWeekendsButtonSelected(bool selected);

		void SetDisplayPowerToggleSelected(bool selected);
		void SetEnableWakeToggleSelected(bool selected);
		void SetEnableSleepToggleSelected(bool selected);

		void SetWakeHour(int hour);
		void SetWakeMinute(int minute);

		void SetSleepHour(int hour);
		void SetSleepMinute(int minute);

		void EnableWakeControls(bool enabled);
		void EnableSleepControls(bool enabled);
	}
}
