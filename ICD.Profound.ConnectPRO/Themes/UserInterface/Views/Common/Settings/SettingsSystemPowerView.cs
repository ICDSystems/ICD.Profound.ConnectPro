using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	public sealed partial class SettingsSystemPowerView : AbstractView, ISettingsSystemPowerView
	{
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
		public event EventHandler OnEnableButtonPressed;
		public event EventHandler OnDisableButtonPressed;
		public event EventHandler OnWakeButtonPressed;
		public event EventHandler OnSleepButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsSystemPowerView(ISigInputOutput panel, ConnectProTheme theme)
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
			OnEnableButtonPressed = null;
			OnDisableButtonPressed = null;
			OnWakeButtonPressed = null;
			OnSleepButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		public void SetWeekdaysButtonSelected(bool selected)
		{
			m_WeekdaysButton.SetSelected(selected);
		}

		public void SetWeekendsButtonSelected(bool selected)
		{
			m_WeekendsButton.SetSelected(selected);
		}

		public void SetEnableButtonSelected(bool selected)
		{
			m_EnableButton.SetSelected(selected);
		}

		public void SetDisableButtonSelected(bool selected)
		{
			m_DisableButton.SetSelected(selected);
		}

		public void SetWakeHour(ushort hour)
		{
			m_WakeHourLabel.SetLabelText(hour.ToString());
		}

		public void SetWakeMinute(ushort minute)
		{
			m_WakeMinuteLabel.SetLabelText(minute.ToString());
		}

		public void SetSleepHour(ushort hour)
		{
			m_SleepHourLabel.SetLabelText(hour.ToString());
		}

		public void SetSleepMinute(ushort minute)
		{
			m_SleepMinuteLabel.SetLabelText(minute.ToString());
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_WeekdaysButton.OnPressed += WeekdaysButtonOnPressed;
			m_WeekendsButton.OnPressed += WeekendsButtonOnPressed;

			m_WakeHourIncrementButton.OnPressed += WakeHourIncrementButtonOnPressed;
			m_WakeHourDecrementButton.OnPressed += WakeHourDecrementButtonOnPressed;
			m_WakeMinuteIncrementButton.OnPressed += WakeMinuteIncrementButtonOnPressed;
			m_WakeMinuteDecrementButton.OnPressed += WakeMinuteDecrementButtonOnPressed;

			m_SleepHourIncrementButton.OnPressed += SleepHourIncrementButtonOnPressed;
			m_SleepHourDecrementButton.OnPressed += SleepHourDecrementButtonOnPressed;
			m_SleepMinuteIncrementButton.OnPressed += SleepMinuteIncrementButtonOnPressed;
			m_SleepMinuteDecrementButton.OnPressed += SleepMinuteDecrementButtonOnPressed;

			m_EnableButton.OnPressed += EnableButtonOnPressed;
			m_DisableButton.OnPressed += DisableButtonOnPressed;

			m_WakeButton.OnPressed += WakeButtonOnPressed;
			m_SleepButton.OnPressed += SleepButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_WeekdaysButton.OnPressed -= WeekdaysButtonOnPressed;
			m_WeekendsButton.OnPressed -= WeekendsButtonOnPressed;

			m_WakeHourIncrementButton.OnPressed -= WakeHourIncrementButtonOnPressed;
			m_WakeHourDecrementButton.OnPressed -= WakeHourDecrementButtonOnPressed;
			m_WakeMinuteIncrementButton.OnPressed -= WakeMinuteIncrementButtonOnPressed;
			m_WakeMinuteDecrementButton.OnPressed -= WakeMinuteDecrementButtonOnPressed;

			m_SleepHourIncrementButton.OnPressed -= SleepHourIncrementButtonOnPressed;
			m_SleepHourDecrementButton.OnPressed -= SleepHourDecrementButtonOnPressed;
			m_SleepMinuteIncrementButton.OnPressed -= SleepMinuteIncrementButtonOnPressed;
			m_SleepMinuteDecrementButton.OnPressed -= SleepMinuteDecrementButtonOnPressed;

			m_EnableButton.OnPressed -= EnableButtonOnPressed;
			m_DisableButton.OnPressed -= DisableButtonOnPressed;

			m_WakeButton.OnPressed -= WakeButtonOnPressed;
			m_SleepButton.OnPressed -= SleepButtonOnPressed;
		}

		private void WeekdaysButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnWeekdaysButtonPressed.Raise(this);
		}

		private void WeekendsButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnWeekendsButtonPressed.Raise(this);
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

		private void EnableButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnEnableButtonPressed.Raise(this);
		}

		private void DisableButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDisableButtonPressed.Raise(this);
		}

		private void WakeButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnWakeButtonPressed.Raise(this);
		}

		private void SleepButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSleepButtonPressed.Raise(this);
		}

		#endregion
	}
}
