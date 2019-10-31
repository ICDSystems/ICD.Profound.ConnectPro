using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Commercial;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.SettingsTree.Administrative;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Administrative;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Administrative;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Administrative
{
	[PresenterBinding(typeof(ISettingsPowerPresenter))]
	public sealed class SettingsPowerPresenter : AbstractSettingsNodeBasePresenter<ISettingsPowerView, PowerSettingsLeaf>, ISettingsPowerPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

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
		public SettingsPowerPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

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
		protected override void NodeChanged(PowerSettingsLeaf node)
		{
			base.NodeChanged(node);

			if (node == null)
				return;

			m_RefreshSection.Enter();

			try
			{
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
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_RefreshSection.Enter();

			try
			{
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
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			Node.SetDirty(true);
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
				if (Weekend)
				{
					view.SetWeekendsButtonSelected();

					view.SetSleepHour((ushort)m_WeekendSleepTime.Hours);
					view.SetSleepMinute((ushort)m_WeekendSleepTime.Minutes);

					view.SetWakeHour((ushort)m_WeekendWakeTime.Hours);
					view.SetWakeMinute((ushort)m_WeekendWakeTime.Minutes);

					view.SetEnableToggleSelected(m_WeekendEnable);
				}
				else
				{
					view.SetWeekdaysButtonSelected();

					view.SetSleepHour((ushort)m_WeekdaySleepTime.Hours);
					view.SetSleepMinute((ushort)m_WeekdaySleepTime.Minutes);

					view.SetWakeHour((ushort)m_WeekdayWakeTime.Hours);
					view.SetWakeMinute((ushort)m_WeekdayWakeTime.Minutes);

					view.SetEnableToggleSelected(m_WeekdayEnable);
				}

				bool isAwake = Node != null && Node.IsAwake;
				view.SetIsAwakeToggleSelected(isAwake);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void IncrementWakeHours(int hours)
		{
			m_RefreshSection.Enter();

			try
			{
				if (Weekend)
					m_WeekendWakeTime = m_WeekendWakeTime.AddHoursAndWrap(hours);
				else
					m_WeekdayWakeTime = m_WeekdayWakeTime.AddHoursAndWrap(hours);
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
				if (Weekend)
					m_WeekendWakeTime = m_WeekendWakeTime.AddMinutesAndWrap(minutes);
				else
					m_WeekdayWakeTime = m_WeekdayWakeTime.AddMinutesAndWrap(minutes);
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
				if (Weekend)
					m_WeekendSleepTime = m_WeekendSleepTime.AddHoursAndWrap(hours);
				else
					m_WeekdaySleepTime = m_WeekdaySleepTime.AddHoursAndWrap(hours);
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
				if (Weekend)
					m_WeekendSleepTime = m_WeekendSleepTime.AddMinutesAndWrap(minutes);
				else
					m_WeekdaySleepTime = m_WeekdaySleepTime.AddMinutesAndWrap(minutes);
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshIfVisible();
		}

		private void ToggleIsAwake()
		{
			if (Node != null)
				Node.ToggleIsAwake();
		}

		private void ToggleEnabled()
		{
			m_RefreshSection.Enter();

			try
			{
				if (Weekend)
					m_WeekendEnable = !m_WeekendEnable;
				else
					m_WeekdayEnable = !m_WeekdayEnable;
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			Refresh();
		}

		#region Node Callbacks

		/// <summary>
		/// Subscribe to the node events.
		/// </summary>
		/// <param name="node"></param>
		protected override void Subscribe(PowerSettingsLeaf node)
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
		protected override void Unsubscribe(PowerSettingsLeaf node)
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

			view.OnIsAwakeTogglePressed += ViewOnIsAwakeTogglePressed;
			view.OnEnableTogglePressed += ViewOnEnableTogglePressed;
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

			view.OnIsAwakeTogglePressed -= ViewOnIsAwakeTogglePressed;
			view.OnEnableTogglePressed -= ViewOnEnableTogglePressed;
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

		private void ViewOnIsAwakeTogglePressed(object sender, EventArgs e)
		{
			ToggleIsAwake();
		}

		private void ViewOnEnableTogglePressed(object sender, EventArgs eventArgs)
		{
			ToggleEnabled();
		}

		#endregion
	}
}
