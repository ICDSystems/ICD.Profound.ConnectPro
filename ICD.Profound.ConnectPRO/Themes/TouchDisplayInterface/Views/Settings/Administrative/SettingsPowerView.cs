using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings.Administrative;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Settings.Administrative
{
	[ViewBinding(typeof(ISettingsPowerView))]
	public sealed partial class SettingsPowerView : AbstractTouchDisplayView, ISettingsPowerView
	{
		private const ushort INDEX_WEEKDAYS = 0;
		private const ushort INDEX_WEEKENDS = 1;

		public event EventHandler OnWeekdaysButtonPressed;
		public event EventHandler OnWeekendsButtonPressed;
		public event EventHandler OnWakeHourIncrementButtonPressed;
		public event EventHandler OnWakeHourDecrementButtonPressed;
		public event EventHandler OnWakeMinuteIncrementButtonPressed;
		public event EventHandler OnWakeMinuteDecrementButtonPressed;
		public event EventHandler OnSleepHourIncrementButtonPressed;
		public event EventHandler OnSleepHourDecrementButtonPressed;
		public event EventHandler OnSleepMinuteIncrementButtonPressed;
		public event EventHandler OnSleepMinuteDecrementButtonPressed;
		public event EventHandler OnEnableTogglePressed;
		public event EventHandler OnSystemPowerPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsPowerView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnWeekdaysButtonPressed = null;
			OnWeekendsButtonPressed = null;
			OnWakeHourIncrementButtonPressed = null;
			OnWakeHourDecrementButtonPressed = null;
			OnWakeMinuteIncrementButtonPressed = null;
			OnWakeMinuteDecrementButtonPressed = null;
			OnSleepHourIncrementButtonPressed = null;
			OnSleepHourDecrementButtonPressed = null;
			OnSleepMinuteIncrementButtonPressed = null;
			OnSleepMinuteDecrementButtonPressed = null;
			OnEnableTogglePressed = null;
			OnSystemPowerPressed = null;

			base.Dispose();
		}

		#region Methods

		public void SetWeekdaysButtonSelected()
		{
			m_DaysButtons.SetItemSelected(INDEX_WEEKENDS, false);
			m_DaysButtons.SetItemSelected(INDEX_WEEKDAYS, true);
		}

		public void SetWeekendsButtonSelected()
		{
			m_DaysButtons.SetItemSelected(INDEX_WEEKDAYS, false);
			m_DaysButtons.SetItemSelected(INDEX_WEEKENDS, true);
		}

		public void SetEnableToggleSelected(bool selected)
		{
			m_EnableToggleButton.SetSelected(selected);
		}

		public void SetWakeHour(int hour)
		{
			m_WakeHourLabel.SetLabelText(hour.ToString());
		}

		public void SetWakeMinute(int minute)
		{
			m_WakeMinuteLabel.SetLabelText(minute.ToString("00"));
		}

		public void SetSleepHour(int hour)
		{
			m_SleepHourLabel.SetLabelText(hour.ToString());
		}

		public void SetSleepMinute(int minute)
		{
			m_SleepMinuteLabel.SetLabelText(minute.ToString("00"));
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DaysButtons.OnButtonPressed += DaysButtonsOnButtonPressed;

			m_WakeHourIncrementButton.OnPressed += WakeHourIncrementButtonOnPressed;
			m_WakeHourDecrementButton.OnPressed += WakeHourDecrementButtonOnPressed;
			m_WakeMinuteIncrementButton.OnPressed += WakeMinuteIncrementButtonOnPressed;
			m_WakeMinuteDecrementButton.OnPressed += WakeMinuteDecrementButtonOnPressed;

			m_SleepHourIncrementButton.OnPressed += SleepHourIncrementButtonOnPressed;
			m_SleepHourDecrementButton.OnPressed += SleepHourDecrementButtonOnPressed;
			m_SleepMinuteIncrementButton.OnPressed += SleepMinuteIncrementButtonOnPressed;
			m_SleepMinuteDecrementButton.OnPressed += SleepMinuteDecrementButtonOnPressed;

			m_EnableToggleButton.OnPressed += EnableToggleButtonOnPressed;
			m_SystemPowerButton.OnPressed += SystemPowerButtonOnOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DaysButtons.OnButtonPressed -= DaysButtonsOnButtonPressed;

			m_WakeHourIncrementButton.OnPressed -= WakeHourIncrementButtonOnPressed;
			m_WakeHourDecrementButton.OnPressed -= WakeHourDecrementButtonOnPressed;
			m_WakeMinuteIncrementButton.OnPressed -= WakeMinuteIncrementButtonOnPressed;
			m_WakeMinuteDecrementButton.OnPressed -= WakeMinuteDecrementButtonOnPressed;

			m_SleepHourIncrementButton.OnPressed -= SleepHourIncrementButtonOnPressed;
			m_SleepHourDecrementButton.OnPressed -= SleepHourDecrementButtonOnPressed;
			m_SleepMinuteIncrementButton.OnPressed -= SleepMinuteIncrementButtonOnPressed;
			m_SleepMinuteDecrementButton.OnPressed -= SleepMinuteDecrementButtonOnPressed;

			m_EnableToggleButton.OnPressed -= EnableToggleButtonOnPressed;
			m_SystemPowerButton.OnPressed -= SystemPowerButtonOnOnPressed;
		}

		private void DaysButtonsOnButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case INDEX_WEEKDAYS:
					OnWeekdaysButtonPressed.Raise(this);
					break;

				case INDEX_WEEKENDS:
					OnWeekendsButtonPressed.Raise(this);
					break;
			}
		}

		private void WakeHourIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnWakeHourIncrementButtonPressed.Raise(this);
		}

		private void WakeHourDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnWakeHourDecrementButtonPressed.Raise(this);
		}

		private void WakeMinuteIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnWakeMinuteIncrementButtonPressed.Raise(this);
		}

		private void WakeMinuteDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnWakeMinuteDecrementButtonPressed.Raise(this);
		}

		private void SleepHourIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSleepHourIncrementButtonPressed.Raise(this);
		}

		private void SleepHourDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSleepHourDecrementButtonPressed.Raise(this);
		}

		private void SleepMinuteIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSleepMinuteIncrementButtonPressed.Raise(this);
		}

		private void SleepMinuteDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSleepMinuteDecrementButtonPressed.Raise(this);
		}

		private void EnableToggleButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnEnableTogglePressed.Raise(this);
		}

		private void SystemPowerButtonOnOnPressed(object sender, EventArgs e)
		{
			OnSystemPowerPressed.Raise(this);
		}

		#endregion
	}
}
