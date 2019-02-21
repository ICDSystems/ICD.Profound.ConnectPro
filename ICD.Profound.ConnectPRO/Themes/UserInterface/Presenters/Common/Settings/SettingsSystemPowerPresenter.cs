using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	[PresenterBinding(typeof(ISettingsSystemPowerPresenter))]
	public sealed class SettingsSystemPowerPresenter : AbstractUiPresenter<ISettingsSystemPowerView>, ISettingsSystemPowerPresenter
	{
		private static readonly TimeSpan s_HourIncrement = TimeSpan.FromHours(1);
		private static readonly TimeSpan s_MinuteIncrement = TimeSpan.FromMinutes(1);

		private TimeSpan m_WeekdayWakeTime;
		private TimeSpan m_WeekendWakeTime;
		private TimeSpan m_WeekdaySleepTime;
		private TimeSpan m_WeekendSleepTime;
		private bool m_WeekdayEnable;
		private bool m_WeekendEnable;

		private bool m_Weekend;

		public bool Weekend
		{
			get { return m_Weekend; }
			set
			{
				if (m_Weekend == value)
					return;

				m_Weekend = value;
				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsSystemPowerPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_WeekendEnable = false;
			m_WeekdayEnable = false;

			m_WeekdayWakeTime = new TimeSpan(7, 0, 0);
			m_WeekdaySleepTime = new TimeSpan(19, 0, 0);

			m_WeekendWakeTime = new TimeSpan(7, 0, 0);
			m_WeekendSleepTime = new TimeSpan(19, 0, 0);
		}

		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			if (Room == null)
				return;

			var schedule = Room.WakeSchedule;

			m_WeekdayEnable = schedule.WeekdayEnable;
			m_WeekendEnable = schedule.WeekendEnable;

			if (schedule.WeekdayWakeTime.HasValue)
				m_WeekdayWakeTime = schedule.WeekdayWakeTime.Value;
			if (schedule.WeekdaySleepTime.HasValue)
				m_WeekdaySleepTime = schedule.WeekdaySleepTime.Value;

			if (schedule.WeekendWakeTime.HasValue)
				m_WeekendWakeTime = schedule.WeekendWakeTime.Value;
			if (schedule.WeekendSleepTime.HasValue)
				m_WeekendSleepTime = schedule.WeekendSleepTime.Value;
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (!args.Data && Room != null)
			{
				var schedule = Room.WakeSchedule;
				schedule.WeekdayEnable = m_WeekdayEnable;
				schedule.WeekendEnable = m_WeekendEnable;

				schedule.WeekdayWakeTime = m_WeekdayWakeTime;
				schedule.WeekdaySleepTime = m_WeekdaySleepTime;
				schedule.WeekendWakeTime = m_WeekendWakeTime;
				schedule.WeekendSleepTime = m_WeekendSleepTime;
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsSystemPowerView view)
		{
			base.Refresh(view);

			view.SetWeekdaysButtonSelected(!Weekend);
			view.SetWeekendsButtonSelected(Weekend);

			if (Weekend)
			{
				view.SetSleepHour((ushort) m_WeekendSleepTime.Hours);
				view.SetSleepMinute((ushort) m_WeekendSleepTime.Minutes);
			
				view.SetWakeHour((ushort) m_WeekendWakeTime.Hours);
				view.SetWakeMinute((ushort) m_WeekendWakeTime.Minutes);

				view.SetEnableButtonSelected(m_WeekendEnable);
				view.SetDisableButtonSelected(!m_WeekendEnable);
			}
			else
			{
				view.SetSleepHour((ushort) m_WeekdaySleepTime.Hours);
				view.SetSleepMinute((ushort) m_WeekdaySleepTime.Minutes);

				view.SetWakeHour((ushort) m_WeekdayWakeTime.Hours);
				view.SetWakeMinute((ushort) m_WeekdayWakeTime.Minutes);
				
				view.SetEnableButtonSelected(m_WeekdayEnable);
				view.SetDisableButtonSelected(!m_WeekdayEnable);
			}

			view.SetSleepButtonVisibility(true);

			bool hasOsd = Room != null && Room.Originators.HasInstances<OsdPanelDevice>();
			view.SetWakeButtonVisibility(hasOsd);
		}

		private void OffsetWakeTime(TimeSpan offset)
		{
			if (Weekend)
				m_WeekendWakeTime += offset;
			else
				m_WeekdayWakeTime += offset;

			Refresh();
		}

		private void OffsetSleepTime(TimeSpan offset)
		{
			if (Weekend)
				m_WeekendSleepTime += offset;
			else
				m_WeekdaySleepTime += offset;

			Refresh();
		}

		private void SetEnabled(bool enabled)
		{
			if (Weekend)
				m_WeekendEnable = enabled;
			else
				m_WeekdayEnable = enabled;

			Refresh();
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsSystemPowerView view)
		{
			base.Subscribe(view);

			view.OnWeekdaysButtonPressed += ViewOnWeekdaysButtonPressed;
			view.OnWeekendsButtonPressed += ViewOnWeekendsButtonPressed;

			view.OnWakeHourIncrementButtonPressed += ViewOnWakeHourIncrementButtonPressed;
			view.OnWakeHourDecrementButtonPressed += ViewOnWakeHourDecrementButtonPressed;
			view.OnWakeMinuteIncrementButtonPressed += ViewOnWakeMinuteIncrementButtonPressed;
			view.OnWakeMinuteDecrementButtonPressed += ViewOnWakeMinuteDecrementButtonPressed;

			view.OnSleepHourIncrementButtonPressed += ViewOnSleepHourIncrementButtonPressed;
			view.OnSleepHourDecrementButtonPressed += ViewOnSleepHourDecrementButtonPressed;
			view.OnSleepMinuteIncrementButtonPressed += ViewOnSleepMinuteIncrementButtonPressed;
			view.OnSleepMinuteDecrementButtonPressed += ViewOnSleepMinuteDecrementButtonPressed;

			view.OnEnableButtonPressed += ViewOnEnableButtonPressed;
			view.OnDisableButtonPressed += ViewOnDisableButtonPressed;

			view.OnWakeButtonPressed += ViewOnWakeButtonPressed;
			view.OnSleepButtonPressed += ViewOnSleepButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsSystemPowerView view)
		{
			base.Unsubscribe(view);

			view.OnWeekdaysButtonPressed -= ViewOnWeekdaysButtonPressed;
			view.OnWeekendsButtonPressed -= ViewOnWeekendsButtonPressed;

			view.OnWakeHourIncrementButtonPressed -= ViewOnWakeHourIncrementButtonPressed;
			view.OnWakeHourDecrementButtonPressed -= ViewOnWakeHourDecrementButtonPressed;
			view.OnWakeMinuteIncrementButtonPressed -= ViewOnWakeMinuteIncrementButtonPressed;
			view.OnWakeMinuteDecrementButtonPressed -= ViewOnWakeMinuteDecrementButtonPressed;

			view.OnSleepHourIncrementButtonPressed -= ViewOnSleepHourIncrementButtonPressed;
			view.OnSleepHourDecrementButtonPressed -= ViewOnSleepHourDecrementButtonPressed;
			view.OnSleepMinuteIncrementButtonPressed -= ViewOnSleepMinuteIncrementButtonPressed;
			view.OnSleepMinuteDecrementButtonPressed -= ViewOnSleepMinuteDecrementButtonPressed;

			view.OnEnableButtonPressed -= ViewOnEnableButtonPressed;
			view.OnDisableButtonPressed -= ViewOnDisableButtonPressed;

			view.OnWakeButtonPressed -= ViewOnWakeButtonPressed;
			view.OnSleepButtonPressed -= ViewOnSleepButtonPressed;
		}

		private void ViewOnWeekdaysButtonPressed(object sender, EventArgs eventArgs)
		{
			Weekend = false;
		}

		private void ViewOnWeekendsButtonPressed(object sender, EventArgs eventArgs)
		{
			Weekend = true;
		}

		private void ViewOnWakeHourIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			OffsetWakeTime(s_HourIncrement);
		}

		private void ViewOnWakeHourDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			OffsetWakeTime(-s_HourIncrement);
		}

		private void ViewOnWakeMinuteIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			OffsetWakeTime(s_MinuteIncrement);
		}

		private void ViewOnWakeMinuteDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			OffsetWakeTime(-s_MinuteIncrement);
		}

		private void ViewOnSleepHourIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			OffsetSleepTime(s_HourIncrement);
		}

		private void ViewOnSleepHourDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			OffsetSleepTime(-s_HourIncrement);
		}

		private void ViewOnSleepMinuteIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			OffsetSleepTime(s_MinuteIncrement);
		}

		private void ViewOnSleepMinuteDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			OffsetSleepTime(-s_MinuteIncrement);
		}

		private void ViewOnEnableButtonPressed(object sender, EventArgs eventArgs)
		{
			SetEnabled(true);
		}

		private void ViewOnDisableButtonPressed(object sender, EventArgs eventArgs)
		{
			SetEnabled(false);
		}

		private void ViewOnWakeButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Room.Wake();
		}

		private void ViewOnSleepButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Room.Sleep();
		}

		#endregion
	}
}
