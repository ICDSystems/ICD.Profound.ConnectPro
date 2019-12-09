using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.SettingsTree.Administrative;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Settings.Administrative;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings.Administrative;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Settings.Administrative
{
	[PresenterBinding(typeof(ISettingsClockPresenter))]
	public sealed class SettingsClockPresenter : AbstractSettingsNodeBasePresenter<ISettingsClockView, ClockSettingsLeaf>, ISettingsClockPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private bool m_Am;
		private DateTime m_Time;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsClockPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsClockView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				int day = m_Time.Day;
				int month = m_Time.Month;
				int year = m_Time.Year;

				int hour = m_Time.Hour;
				bool is24HourMode = Node != null && Node.Is24HourMode;
				bool am = is24HourMode ? hour < 12 : m_Am;
				
				if (!is24HourMode)
					hour = DateTimeUtils.To12Hour(hour);

				int minute = m_Time.Minute;

				view.Set24HourMode(is24HourMode);
				view.SetAm(am);
				view.SetHour(hour);
				view.SetMinute(minute);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Called when the wrapped node changes.
		/// </summary>
		/// <param name="node"></param>
		protected override void NodeChanged(ClockSettingsLeaf node)
		{
			base.NodeChanged(node);

			m_Time = Node == null ? default(DateTime) : Node.ClockTime;
		}

		#region Private Methods

		private void AddMinutesAndWrap(int minutes)
		{
			if (Node == null)
				return;

			m_Time = m_Time.AddMinutesAndWrap(minutes);

			Node.SetClockTime(m_Time);

			RefreshIfVisible();
		}

		private void AddHoursAndWrap(int hours)
		{
			if (Node == null)
				return;

			m_Time =
				Node.Is24HourMode
					? m_Time.AddHoursAndWrap(hours)
					: m_Time.AddHoursAndWrap12Hour(hours);

			Node.SetClockTime(m_Time);

			RefreshIfVisible();
		}

		private void Set24HourMode(bool hours24Mode)
		{
			if (Node == null)
				return;

			if (hours24Mode == Node.Is24HourMode)
				return;

			Node.Set24HourMode(hours24Mode);

			SetAmMode(m_Time.Hour < 12);

			RefreshIfVisible();
		}

		private void SetAmMode(bool amMode)
		{
			if (amMode == m_Am)
				return;

			m_Am = amMode;

			// Fix the time back into AM/PM
			if (!Node.Is24HourMode)
			{
				if (m_Am && m_Time.Hour >= 12)
					m_Time -= TimeSpan.FromHours(12);
				else if (!m_Am && m_Time.Hour < 12)
					m_Time += TimeSpan.FromHours(12);
			}

			Node.SetClockTime(m_Time);

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsClockView view)
		{
			base.Subscribe(view);

			view.On24HourTogglePressed += ViewOn24HourTogglePressed;
			view.OnAmPmTogglePressed += ViewOnAmPmTogglePressed;
			view.OnHourDownButtonPressed += ViewOnHourDownButtonPressed;
			view.OnHourUpButtonPressed += ViewOnHourUpButtonPressed;
			view.OnMinuteDownButtonPressed += ViewOnMinuteDownButtonPressed;
			view.OnMinuteUpButtonPressed += ViewOnMinuteUpButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsClockView view)
		{
			base.Unsubscribe(view);

			view.On24HourTogglePressed -= ViewOn24HourTogglePressed;
			view.OnAmPmTogglePressed -= ViewOnAmPmTogglePressed;
			view.OnHourDownButtonPressed -= ViewOnHourDownButtonPressed;
			view.OnHourUpButtonPressed -= ViewOnHourUpButtonPressed;
			view.OnMinuteDownButtonPressed -= ViewOnMinuteDownButtonPressed;
			view.OnMinuteUpButtonPressed -= ViewOnMinuteUpButtonPressed;
		}

		/// <summary>
		/// Called when the view visibility is about to change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			// When the view is about to be shown we update the current date
			if (args.Data)
				m_Time = IcdEnvironment.GetLocalTime().TimeOfDay;
		}

		private void ViewOnMinuteUpButtonPressed(object sender, EventArgs eventArgs)
		{
			AddMinutesAndWrap(1);
		}

		private void ViewOnMinuteDownButtonPressed(object sender, EventArgs eventArgs)
		{
			AddMinutesAndWrap(-1);
		}

		private void ViewOnHourUpButtonPressed(object sender, EventArgs eventArgs)
		{
			AddHoursAndWrap(1);
		}

		private void ViewOnHourDownButtonPressed(object sender, EventArgs eventArgs)
		{
			AddHoursAndWrap(-1);
		}

		private void ViewOnAmPmTogglePressed(object sender, EventArgs eventArgs)
		{
			SetAmMode(!m_Am);
		}

		private void ViewOn24HourTogglePressed(object sender, EventArgs eventArgs)
		{
			if (Node != null)
				Set24HourMode(!Node.Is24HourMode);
		}

		#endregion
	}
}
