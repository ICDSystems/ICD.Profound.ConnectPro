using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Administrative;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Administrative
{
	[ViewBinding(typeof(ISettingsPowerView))]
	public sealed partial class SettingsPowerView : AbstractUiView, ISettingsPowerView
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

		public event EventHandler OnIncrementDecrementButtonReleased;

		public event EventHandler OnDisplayPowerTogglePressed;
		public event EventHandler OnEnableWakeTogglePressed;
		public event EventHandler OnEnableSleepTogglePressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsPowerView(ISigInputOutput panel, IConnectProTheme theme)
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
			OnIncrementDecrementButtonReleased = null;
			OnDisplayPowerTogglePressed = null;
			OnEnableWakeTogglePressed = null;
			OnEnableSleepTogglePressed = null;

			base.Dispose();
		}

		#region Methods

		public void SetWeekdaysButtonSelected(bool selected)
		{
			m_DaysButtons.SetItemSelected(INDEX_WEEKDAYS, selected);
		}

		public void SetWeekendsButtonSelected(bool selected)
		{
			m_DaysButtons.SetItemSelected(INDEX_WEEKENDS, selected);
		}

		public void SetDisplayPowerToggleSelected(bool selected)
		{
			// Button graphic defaults to selected
			m_DisplayPowerToggleButton.SetSelected(!selected);
		}

		public void SetEnableWakeToggleSelected(bool selected)
		{
			// Wake Toggle Button graphic defaults to selected
			m_EnableWakeToggleButton.SetSelected(!selected);
		}
		public void SetEnableSleepToggleSelected(bool selected)
		{
			// Sleep Toggle Button graphic defaults to selected
			m_EnableSleepToggleButton.SetSelected(!selected);
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

		public void EnableWakeControls(bool enabled)
		{
			// All of the wake controls share the same join
			m_WakeMinuteIncrementButton.Enable(enabled);
		}

		public void EnableSleepControls(bool enabled)
		{
			// All of the sleep controls share the same join
			m_SleepMinuteIncrementButton.Enable(enabled);
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
			m_WakeHourIncrementButton.OnReleased += WakeHourIncrementButtonOnReleased;
			m_WakeHourDecrementButton.OnPressed += WakeHourDecrementButtonOnPressed;
			m_WakeHourDecrementButton.OnReleased += WakeHourDecrementButtonOnReleased;
			m_WakeMinuteIncrementButton.OnPressed += WakeMinuteIncrementButtonOnPressed;
			m_WakeMinuteIncrementButton.OnReleased += WakeMinuteIncrementButtonOnReleased;
			m_WakeMinuteDecrementButton.OnPressed += WakeMinuteDecrementButtonOnPressed;
			m_WakeMinuteDecrementButton.OnReleased += WakeMinuteDecrementButtonOnReleased;

			m_SleepHourIncrementButton.OnPressed += SleepHourIncrementButtonOnPressed;
			m_SleepHourIncrementButton.OnReleased += SleepHourIncrementButtonOnReleased;
			m_SleepHourDecrementButton.OnPressed += SleepHourDecrementButtonOnPressed;
			m_SleepHourDecrementButton.OnReleased += SleepHourDecrementButtonOnReleased;
			m_SleepMinuteIncrementButton.OnPressed += SleepMinuteIncrementButtonOnPressed;
			m_SleepMinuteIncrementButton.OnReleased += SleepMinuteIncrementButtonOnReleased;
			m_SleepMinuteDecrementButton.OnPressed += SleepMinuteDecrementButtonOnPressed;
			m_SleepMinuteDecrementButton.OnReleased += SleepMinuteDecrementButtonOnReleased;


			m_DisplayPowerToggleButton.OnPressed += DisplayPowerToggleButtonOnPressed;
			m_EnableWakeToggleButton.OnPressed += EnableWakeToggleButtonOnPressed;
			m_EnableSleepToggleButton.OnPressed += EnableSleepToggleButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DaysButtons.OnButtonPressed -= DaysButtonsOnButtonPressed;

			m_WakeHourIncrementButton.OnPressed -= WakeHourIncrementButtonOnPressed;
			m_WakeHourIncrementButton.OnReleased -= WakeHourIncrementButtonOnReleased;
			m_WakeHourDecrementButton.OnPressed -= WakeHourDecrementButtonOnPressed;
			m_WakeHourDecrementButton.OnReleased -= WakeHourDecrementButtonOnReleased;
			m_WakeMinuteIncrementButton.OnPressed -= WakeMinuteIncrementButtonOnPressed;
			m_WakeMinuteIncrementButton.OnReleased -= WakeMinuteIncrementButtonOnReleased;
			m_WakeMinuteDecrementButton.OnPressed -= WakeMinuteDecrementButtonOnPressed;
			m_WakeMinuteDecrementButton.OnReleased -= WakeMinuteDecrementButtonOnReleased;

			m_SleepHourIncrementButton.OnPressed -= SleepHourIncrementButtonOnPressed;
			m_SleepHourIncrementButton.OnReleased -= SleepHourIncrementButtonOnReleased;
			m_SleepHourDecrementButton.OnPressed -= SleepHourDecrementButtonOnPressed;
			m_SleepHourDecrementButton.OnReleased -= SleepHourDecrementButtonOnReleased;
			m_SleepMinuteIncrementButton.OnPressed -= SleepMinuteIncrementButtonOnPressed;
			m_SleepMinuteIncrementButton.OnReleased -= SleepMinuteIncrementButtonOnReleased;
			m_SleepMinuteDecrementButton.OnPressed -= SleepMinuteDecrementButtonOnPressed;
			m_SleepMinuteDecrementButton.OnReleased -= SleepMinuteDecrementButtonOnReleased;

			m_DisplayPowerToggleButton.OnPressed -= DisplayPowerToggleButtonOnPressed;
			m_EnableWakeToggleButton.OnPressed -= EnableWakeToggleButtonOnPressed;
			m_EnableSleepToggleButton.OnPressed -= EnableSleepToggleButtonOnPressed;
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

		private void WakeHourIncrementButtonOnReleased(object sender, EventArgs e)
		{
			OnIncrementDecrementButtonReleased.Raise(this);
		}

		private void WakeHourDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnWakeHourDecrementButtonPressed.Raise(this);
		}

		private void WakeHourDecrementButtonOnReleased(object sender, EventArgs e)
		{
			OnIncrementDecrementButtonReleased.Raise(this);
		}

		private void WakeMinuteIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnWakeMinuteIncrementButtonPressed.Raise(this);
		}

		private void WakeMinuteIncrementButtonOnReleased(object sender, EventArgs e)
		{
			OnIncrementDecrementButtonReleased.Raise(this);
		}

		private void WakeMinuteDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnWakeMinuteDecrementButtonPressed.Raise(this);
		}

		private void WakeMinuteDecrementButtonOnReleased(object sender, EventArgs e)
		{
			OnIncrementDecrementButtonReleased.Raise(this);
		}

		private void SleepHourIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSleepHourIncrementButtonPressed.Raise(this);
		}

		private void SleepHourIncrementButtonOnReleased(object sender, EventArgs e)
		{
			OnIncrementDecrementButtonReleased.Raise(this);
		}

		private void SleepHourDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSleepHourDecrementButtonPressed.Raise(this);
		}

		private void SleepHourDecrementButtonOnReleased(object sender, EventArgs e)
		{
			OnIncrementDecrementButtonReleased.Raise(this);
		}

		private void SleepMinuteIncrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSleepMinuteIncrementButtonPressed.Raise(this);
		}

		private void SleepMinuteIncrementButtonOnReleased(object sender, EventArgs e)
		{
			OnIncrementDecrementButtonReleased.Raise(this);
		}

		private void SleepMinuteDecrementButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSleepMinuteDecrementButtonPressed.Raise(this);
		}

		private void SleepMinuteDecrementButtonOnReleased(object sender, EventArgs e)
		{
			OnIncrementDecrementButtonReleased.Raise(this);
		}

		private void DisplayPowerToggleButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDisplayPowerTogglePressed.Raise(this);
		}

		private void EnableWakeToggleButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnEnableWakeTogglePressed.Raise(this);
		}

		private void EnableSleepToggleButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnEnableSleepTogglePressed.Raise(this);
		}

		#endregion
	}
}
