using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Commercial;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.SettingsTree.Administrative;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings.Administrative;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.Administrative;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Settings.Administrative
{
	[PresenterBinding(typeof(ISettingsPowerPresenter))]
	public sealed class SettingsPowerPresenter : AbstractSettingsNodeBasePresenter<ISettingsPowerView, WakeSleepSettingsLeaf>, ISettingsPowerPresenter
	{
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
		public SettingsPowerPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_WeekendEnable = false;
			m_WeekdayEnable = false;

			m_WeekdayWakeTime = new TimeSpan(7, 0, 0);
			m_WeekdaySleepTime = new TimeSpan(19, 0, 0);

			m_WeekendWakeTime = new TimeSpan(7, 0, 0);
			m_WeekendSleepTime = new TimeSpan(19, 0, 0);
		}

		/// <summary>
		/// Called when the wrapped node changes.
		/// </summary>
		/// <param name="node"></param>
		protected override void NodeChanged(WakeSleepSettingsLeaf node)
		{
			base.NodeChanged(node);

			if (node == null)
				return;

			WakeSchedule schedule = node.WakeSchedule;

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

			// Save when the view is hidden
			if (args.Data || Node == null)
				return;

			WakeSchedule schedule = Node.WakeSchedule;
			schedule.WeekdayEnable = m_WeekdayEnable;
			schedule.WeekendEnable = m_WeekendEnable;

			schedule.WeekdayWakeTime = m_WeekdayWakeTime;
			schedule.WeekdaySleepTime = m_WeekdaySleepTime;
			schedule.WeekendWakeTime = m_WeekendWakeTime;
			schedule.WeekendSleepTime = m_WeekendSleepTime;

			Node.SetDirty(true);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsPowerView view)
		{
			base.Refresh(view);

			if (Weekend)
			{
				view.SetWeekendsButtonSelected();

				view.SetSleepHour((ushort) m_WeekendSleepTime.Hours);
				view.SetSleepMinute((ushort) m_WeekendSleepTime.Minutes);
			
				view.SetWakeHour((ushort) m_WeekendWakeTime.Hours);
				view.SetWakeMinute((ushort) m_WeekendWakeTime.Minutes);

				view.SetEnableToggleSelected(m_WeekendEnable);
			}
			else
			{
				view.SetWeekdaysButtonSelected();

				view.SetSleepHour((ushort) m_WeekdaySleepTime.Hours);
				view.SetSleepMinute((ushort) m_WeekdaySleepTime.Minutes);

				view.SetWakeHour((ushort) m_WeekdayWakeTime.Hours);
				view.SetWakeMinute((ushort) m_WeekdayWakeTime.Minutes);

				view.SetEnableToggleSelected(m_WeekdayEnable);
			}
		}

		private void IncrementWakeHours(int hours)
		{
			if (Weekend)
				m_WeekendWakeTime = m_WeekendWakeTime.AddHoursAndWrap(hours);
			else
				m_WeekdayWakeTime = m_WeekdayWakeTime.AddHoursAndWrap(hours);

			Refresh();
		}

		private void IncrementWakeMinutes(int minutes)
		{
			if (Weekend)
				m_WeekendWakeTime = m_WeekendWakeTime.AddMinutesAndWrap(minutes);
			else
				m_WeekdayWakeTime = m_WeekdayWakeTime.AddMinutesAndWrap(minutes);

			Refresh();
		}

		private void IncrementSleepHours(int hours)
		{
			if (Weekend)
				m_WeekendSleepTime = m_WeekendSleepTime.AddHoursAndWrap(hours);
			else
				m_WeekdaySleepTime = m_WeekdaySleepTime.AddHoursAndWrap(hours);

			Refresh();
		}

		private void IncrementSleepMinutes(int minutes)
		{
			if (Weekend)
				m_WeekendSleepTime = m_WeekendSleepTime.AddMinutesAndWrap(minutes);
			else
				m_WeekdaySleepTime = m_WeekdaySleepTime.AddMinutesAndWrap(minutes);

			Refresh();
		}

		private void ToggleEnabled()
		{
			if (Weekend)
				m_WeekendEnable = !m_WeekendEnable;
			else
				m_WeekdayEnable = !m_WeekdayEnable;

			Refresh();
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsPowerView view)
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

			view.OnEnableTogglePressed += ViewOnEnableTogglePressed;
			view.OnSystemPowerPressed += ViewOnSystemPowerPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsPowerView view)
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

			view.OnEnableTogglePressed -= ViewOnEnableTogglePressed;
			view.OnSystemPowerPressed -= ViewOnSystemPowerPressed;
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
			IncrementWakeHours(1);
		}

		private void ViewOnWakeHourDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			IncrementWakeHours(-1);
		}

		private void ViewOnWakeMinuteIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			IncrementWakeMinutes(1);
		}

		private void ViewOnWakeMinuteDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			IncrementWakeMinutes(-1);
		}

		private void ViewOnSleepHourIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			IncrementSleepHours(1);
		}

		private void ViewOnSleepHourDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			IncrementSleepHours(-1);
		}

		private void ViewOnSleepMinuteIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			IncrementSleepMinutes(1);
		}

		private void ViewOnSleepMinuteDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			IncrementSleepMinutes(-1);
		}

		private void ViewOnEnableTogglePressed(object sender, EventArgs eventArgs)
		{
			ToggleEnabled();
		}

		private void ViewOnSystemPowerPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			Room.Sleep();
		}

		#endregion
	}
}
