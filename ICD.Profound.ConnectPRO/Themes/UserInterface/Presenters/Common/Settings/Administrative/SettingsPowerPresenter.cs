using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Partitioning.Commercial;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Administrative;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Administrative;
using ICD.Profound.ConnectPROCommon.SettingsTree.Administrative;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Administrative
{
	[PresenterBinding(typeof(ISettingsPowerPresenter))]
	public sealed class SettingsPowerPresenter : AbstractSettingsNodeBasePresenter<ISettingsPowerView, WakeSleepSettingsLeaf>, ISettingsPowerPresenter
	{
		private readonly Repeater m_Repeater;
		private readonly SafeCriticalSection m_RefreshSection;

		private bool m_Weekend;
		private const long BEFORE_REPEAT = 500;
		private const long HOUR_BETWEEN_REPEAT = 250;
		private const long MINUTE_BETWEEN_REPEAT = 220;

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
		public SettingsPowerPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Repeater = new Repeater();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsPowerView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				// Power
				bool isAwake = Node != null && Node.IsAwake;
				view.SetDisplayPowerToggleSelected(isAwake);

				// Schedule
				WakeSchedule wakeSchedule = Node == null ? null : Node.WakeSchedule;

				TimeSpan? sleepTime =
					wakeSchedule == null
						? null
						: Weekend
							  ? wakeSchedule.WeekendSleepTime
							  : wakeSchedule.WeekdaySleepTime;
				TimeSpan? wakeTime =
					wakeSchedule == null
						? null
						: Weekend
							  ? wakeSchedule.WeekendWakeTime
							  : wakeSchedule.WeekdayWakeTime;

				int sleepHour = sleepTime.HasValue ? sleepTime.Value.Hours : 0;
				int sleepMinute = sleepTime.HasValue ? sleepTime.Value.Minutes : 0;
				int wakeHour = wakeTime.HasValue ? wakeTime.Value.Hours : 0;
				int wakeMinute = wakeTime.HasValue ? wakeTime.Value.Minutes : 0;

				view.SetSleepHour(sleepHour);
				view.SetSleepMinute(sleepMinute);
				view.SetWakeHour(wakeHour);
				view.SetWakeMinute(wakeMinute);

				view.SetWeekendsButtonSelected(Weekend);
				view.SetWeekdaysButtonSelected(!Weekend);

				bool wakeEnabled =
					wakeSchedule != null &&
					(Weekend
						? wakeSchedule.WeekendEnableWake 
						: wakeSchedule.WeekdayEnableWake);

				bool sleepEnabled =
					wakeSchedule != null &&
					(Weekend
						? wakeSchedule.WeekendEnableSleep
						: wakeSchedule.WeekdayEnableSleep);

				view.SetEnableWakeToggleSelected(wakeEnabled);
				view.EnableWakeControls(wakeEnabled);

				view.SetEnableSleepToggleSelected(sleepEnabled);
				view.EnableSleepControls(sleepEnabled);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private void ToggleIsAwake()
		{
			if (Node != null)
				Node.ToggleIsAwake();
		}

		private void IncrementWakeHours(int hours)
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				if (Weekend)
				{
					Node.WakeSchedule.WeekendWakeTime = Node.WakeSchedule.WeekendWakeTime ?? default(TimeSpan);
					Node.WakeSchedule.WeekendWakeTime = Node.WakeSchedule.WeekendWakeTime.Value.AddHoursAndWrap(hours);
				}
				else
				{
					Node.WakeSchedule.WeekdayWakeTime = Node.WakeSchedule.WeekdayWakeTime ?? default(TimeSpan);
					Node.WakeSchedule.WeekdayWakeTime = Node.WakeSchedule.WeekdayWakeTime.Value.AddHoursAndWrap(hours);
				}

				Node.SetDirty(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		private void IncrementWakeMinutes(int minutes)
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				if (Weekend)
				{
					Node.WakeSchedule.WeekendWakeTime = Node.WakeSchedule.WeekendWakeTime ?? default(TimeSpan);
					Node.WakeSchedule.WeekendWakeTime = Node.WakeSchedule.WeekendWakeTime.Value.AddMinutesAndWrap(minutes);
				}
				else
				{
					Node.WakeSchedule.WeekdayWakeTime = Node.WakeSchedule.WeekdayWakeTime ?? default(TimeSpan);
					Node.WakeSchedule.WeekdayWakeTime = Node.WakeSchedule.WeekdayWakeTime.Value.AddMinutesAndWrap(minutes);
				}

				Node.SetDirty(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		private void IncrementSleepHours(int hours)
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				if (Weekend)
				{
					Node.WakeSchedule.WeekendSleepTime = Node.WakeSchedule.WeekendSleepTime ?? default(TimeSpan);
					Node.WakeSchedule.WeekendSleepTime = Node.WakeSchedule.WeekendSleepTime.Value.AddHoursAndWrap(hours);
				}
				else
				{
					Node.WakeSchedule.WeekdaySleepTime = Node.WakeSchedule.WeekdaySleepTime ?? default(TimeSpan);
					Node.WakeSchedule.WeekdaySleepTime = Node.WakeSchedule.WeekdaySleepTime.Value.AddHoursAndWrap(hours);
				}

				Node.SetDirty(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		private void IncrementSleepMinutes(int minutes)
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				if (Weekend)
				{
					Node.WakeSchedule.WeekendSleepTime = Node.WakeSchedule.WeekendSleepTime ?? default(TimeSpan);
					Node.WakeSchedule.WeekendSleepTime = Node.WakeSchedule.WeekendSleepTime.Value.AddMinutesAndWrap(minutes);
				}
				else
				{
					Node.WakeSchedule.WeekdaySleepTime = Node.WakeSchedule.WeekdaySleepTime ?? default(TimeSpan);
					Node.WakeSchedule.WeekdaySleepTime = Node.WakeSchedule.WeekdaySleepTime.Value.AddMinutesAndWrap(minutes);
				}

				Node.SetDirty(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		private void ToggleEnabledWake()
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				if (Weekend)
					Node.WakeSchedule.WeekendEnableWake = !Node.WakeSchedule.WeekendEnableWake;
				else
					Node.WakeSchedule.WeekdayEnableWake = !Node.WakeSchedule.WeekdayEnableWake;

				Node.SetDirty(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			Refresh();
		}

		private void ToggleEnabledSleep()
		{
			m_RefreshSection.Enter();

			try
			{
				if (Node == null)
					return;

				if (Weekend)
					Node.WakeSchedule.WeekendEnableSleep = !Node.WakeSchedule.WeekendEnableSleep;
				else
					Node.WakeSchedule.WeekdayEnableSleep = !Node.WakeSchedule.WeekdayEnableSleep;

				Node.SetDirty(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			Refresh();
		}

		#endregion

		#region Node Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(WakeSleepSettingsLeaf node)
		{
			base.Subscribe(node);

			if (node == null)
				return;

			node.OnRoomIsAwakeStateChanged += NodeOnRoomIsAwakeStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Unsubscribe(WakeSleepSettingsLeaf node)
		{
			base.Unsubscribe(node);

			if (node == null)
				return;

			node.OnRoomIsAwakeStateChanged -= NodeOnRoomIsAwakeStateChanged;
		}

		/// <summary>
		/// Called when the is awake state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void NodeOnRoomIsAwakeStateChanged(object sender, EventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion

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

			view.OnIncrementDecrementButtonReleased += ViewOnIncrementDecrementButtonReleased;

			view.OnDisplayPowerTogglePressed += ViewOnDisplayPowerTogglePressed;
			view.OnEnableWakeTogglePressed += ViewOnEnableWakeTogglePressed;
			view.OnEnableSleepTogglePressed += ViewOnEnableSleepTogglePressed;
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

			view.OnIncrementDecrementButtonReleased -= ViewOnIncrementDecrementButtonReleased;

			view.OnDisplayPowerTogglePressed -= ViewOnDisplayPowerTogglePressed;
			view.OnEnableWakeTogglePressed -= ViewOnEnableWakeTogglePressed;
			view.OnEnableSleepTogglePressed -= ViewOnEnableSleepTogglePressed;
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
			m_Repeater.Start(b => IncrementWakeHours(1), BEFORE_REPEAT, HOUR_BETWEEN_REPEAT);
		}

		private void ViewOnWakeHourDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Repeater.Start(b => IncrementWakeHours(-1), BEFORE_REPEAT, HOUR_BETWEEN_REPEAT);
		}

		private void ViewOnWakeMinuteIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Repeater.Start(b => IncrementWakeMinutes(1), BEFORE_REPEAT, MINUTE_BETWEEN_REPEAT);
		}

		private void ViewOnWakeMinuteDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Repeater.Start(b => IncrementWakeMinutes(-1), BEFORE_REPEAT, MINUTE_BETWEEN_REPEAT);
		}

		private void ViewOnSleepHourIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Repeater.Start(b => IncrementSleepHours(1), BEFORE_REPEAT, HOUR_BETWEEN_REPEAT);
		}

		private void ViewOnSleepHourDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Repeater.Start(b => IncrementSleepHours(-1), BEFORE_REPEAT, HOUR_BETWEEN_REPEAT);
		}

		private void ViewOnSleepMinuteIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Repeater.Start(b => IncrementSleepMinutes(1), BEFORE_REPEAT, MINUTE_BETWEEN_REPEAT);
		}

		private void ViewOnSleepMinuteDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Repeater.Start(b => IncrementSleepMinutes(-1), BEFORE_REPEAT, MINUTE_BETWEEN_REPEAT);
		}

		private void ViewOnIncrementDecrementButtonReleased(object sender, EventArgs eventArgs)
		{
			m_Repeater.Stop();
		}

		private void ViewOnDisplayPowerTogglePressed(object sender, EventArgs e)
		{
			ToggleIsAwake();
		}

		private void ViewOnEnableWakeTogglePressed(object sender, EventArgs eventArgs)
		{
			ToggleEnabledWake();
		}
		private void ViewOnEnableSleepTogglePressed(object sender, EventArgs eventArgs)
		{
			ToggleEnabledSleep();
		}

		#endregion
	}
}
