using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.Administrative;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Settings.Administrative
{
	[ViewBinding(typeof(ISettingsClockView))]
	public sealed partial class SettingsClockView : AbstractTouchDisplayView, ISettingsClockView
	{
		private const ushort MODE_AM = 0;
		private const ushort MODE_PM = 1;
		
		public event EventHandler OnDayUpButtonPressed;
		public event EventHandler OnDayDownButtonPressed;
		public event EventHandler OnMonthUpButtonPressed;
		public event EventHandler OnMonthDownButtonPressed;
		public event EventHandler OnYearUpButtonPressed;
		public event EventHandler OnYearDownButtonPressed;
		public event EventHandler On24HourTogglePressed;
		public event EventHandler OnAmPmTogglePressed;
		public event EventHandler OnHourUpButtonPressed;
		public event EventHandler OnHourDownButtonPressed;
		public event EventHandler OnMinuteUpButtonPressed;
		public event EventHandler OnMinuteDownButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsClockView(ISigInputOutput panel, TouchCueTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			On24HourTogglePressed = null;
			OnAmPmTogglePressed = null;
			OnHourUpButtonPressed = null;
			OnHourDownButtonPressed = null;
			OnMinuteUpButtonPressed = null;
			OnMinuteDownButtonPressed = null;
		}

		#region Methods

		public void SetDay(int day)
		{
			m_DayLabel.SetLabelText(day.ToString("00"));
		}

		public void SetMonth(int month)
		{
			m_MonthLabel.SetLabelText(month.ToString("00"));
		}

		public void SetYear(int year)
		{
			m_YearLabel.SetLabelText(year.ToString("0000"));
		}

		public void Set24HourMode(bool selected)
		{
			m_AmPmButton.Show(!selected);
			m_24HourButton.SetSelected(selected);
		}

		public void SetAm(bool am)
		{
			m_AmPmButton.SetSelected(!am);
			m_BackgroundImage.SetMode(am ? MODE_AM : MODE_PM);
		}

		public void SetHour(int hour)
		{
			m_HourLabel.SetLabelText(hour.ToString("00"));
		}

		public void SetMinute(int minute)
		{
			m_MinuteLabel.SetLabelText(minute.ToString("00"));
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();
			
			m_DayIncrementButton.OnPressed += DayIncrementButtonOnPressed;
			m_DayDecrementButton.OnPressed += DayDecrementButtonOnPressed;
			m_MonthIncrementButton.OnPressed += MonthIncrementButtonOnPressed;
			m_MonthDecrementButton.OnPressed += MonthDecrementButtonOnPressed;
			m_YearIncrementButton.OnPressed += YearIncrementButtonOnPressed;
			m_YearDecrementButton.OnPressed += YearDecrementButtonOnPressed;
			m_24HourButton.OnPressed += HourButtonOnPressed;
			m_AmPmButton.OnPressed += AmPmButtonOnPressed;
			m_HourIncrementButton.OnPressed += HourIncrementButtonOnPressed;
			m_HourDecrementButton.OnPressed += HourDecrementButtonOnPressed;
			m_MinuteIncrementButton.OnPressed += MinuteIncrementButtonOnPressed;
			m_MinuteDecrementButton.OnPressed += MinuteDecrementButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DayIncrementButton.OnPressed -= DayIncrementButtonOnPressed;
			m_DayDecrementButton.OnPressed -= DayDecrementButtonOnPressed;
			m_MonthIncrementButton.OnPressed -= MonthIncrementButtonOnPressed;
			m_MonthDecrementButton.OnPressed -= MonthDecrementButtonOnPressed;
			m_YearIncrementButton.OnPressed -= YearIncrementButtonOnPressed;
			m_YearDecrementButton.OnPressed -= YearDecrementButtonOnPressed;
			m_24HourButton.OnPressed -= HourButtonOnPressed;
			m_AmPmButton.OnPressed -= AmPmButtonOnPressed;
			m_HourIncrementButton.OnPressed -= HourIncrementButtonOnPressed;
			m_HourDecrementButton.OnPressed -= HourDecrementButtonOnPressed;
			m_MinuteIncrementButton.OnPressed -= MinuteIncrementButtonOnPressed;
			m_MinuteDecrementButton.OnPressed -= MinuteDecrementButtonOnPressed;
		}

		private void DayDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDayDownButtonPressed.Raise(this);
		}

		private void DayIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDayUpButtonPressed.Raise(this);
		}

		private void MonthDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnMonthDownButtonPressed.Raise(this);
		}

		private void MonthIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnMonthUpButtonPressed.Raise(this);
		}

		private void YearDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnYearDownButtonPressed.Raise(this);
		}

		private void YearIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnYearUpButtonPressed.Raise(this);
		}

		private void MinuteDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnMinuteDownButtonPressed.Raise(this);
		}

		private void MinuteIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnMinuteUpButtonPressed.Raise(this);
		}

		private void HourDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnHourDownButtonPressed.Raise(this);
		}

		private void HourIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnHourUpButtonPressed.Raise(this);
		}

		private void AmPmButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnAmPmTogglePressed.Raise(this);
		}

		private void HourButtonOnPressed(object sender, EventArgs eventArgs)
		{
			On24HourTogglePressed.Raise(this);
		}

		#endregion
	}
}