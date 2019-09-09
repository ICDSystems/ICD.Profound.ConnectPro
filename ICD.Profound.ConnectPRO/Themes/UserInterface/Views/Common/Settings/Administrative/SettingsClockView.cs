using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Administrative;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Administrative
{
	[ViewBinding(typeof(ISettingsClockView))]
	public sealed partial class SettingsClockView : AbstractUiView, ISettingsClockView
	{
		private const ushort MODE_AM = 0;
		private const ushort MODE_PM = 1;

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
		public SettingsClockView(ISigInputOutput panel, ConnectProTheme theme)
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

		public void Set24HourMode(bool selected)
		{
			m_AmPmButton.Show(!selected);
			m_24HourButton.SetSelected(selected);
		}

		public void SetAmMode(bool am)
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

			m_24HourButton.OnPressed -= HourButtonOnPressed;
			m_AmPmButton.OnPressed -= AmPmButtonOnPressed;
			m_HourIncrementButton.OnPressed -= HourIncrementButtonOnPressed;
			m_HourDecrementButton.OnPressed -= HourDecrementButtonOnPressed;
			m_MinuteIncrementButton.OnPressed -= MinuteIncrementButtonOnPressed;
			m_MinuteDecrementButton.OnPressed -= MinuteDecrementButtonOnPressed;
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