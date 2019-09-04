using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Globalization;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Administrative;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Administrative;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Administrative
{
	[PresenterBinding(typeof(ISettingsClockPresenter))]
	public sealed class SettingsClockPresenter : AbstractUiPresenter<ISettingsClockView>, ISettingsClockPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private bool m_24HourMode;
		private bool m_AmMode;
		private TimeSpan m_Time;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsClockPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_24HourMode = IcdCultureInfo.CurrentCulture.Uses24HourFormat();
			m_AmMode = true;
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
				int hour = m_Time.Hours;
				if (!m_24HourMode)
					hour = DateTimeUtils.To12Hour(hour);

				int minute = m_Time.Minutes;

				view.Set24HourMode(m_24HourMode);
				view.SetAmMode(m_AmMode);
				view.SetHour(hour);
				view.SetMinute(minute);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

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
			{
				m_Time = IcdEnvironment.GetLocalTime().TimeOfDay;
			}
			// When the view is about to be hidden we set the current date
			else
			{
				// Keep precision
				DateTime now = IcdEnvironment.GetLocalTime();
				DateTime dateTime = new DateTime(now.Year, now.Month, now.Day,
				                                 m_Time.Hours, m_Time.Minutes, m_Time.Seconds,
				                                 now.Millisecond);

				IcdEnvironment.SetLocalTime(dateTime);
			}
		}

		private void ViewOnMinuteUpButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Time += new TimeSpan(0, 1, 0);
			RefreshIfVisible();
		}

		private void ViewOnMinuteDownButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Time += new TimeSpan(0, -1, 0);
			RefreshIfVisible();
		}

		private void ViewOnHourUpButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Time += new TimeSpan(1, 0, 0);
			RefreshIfVisible();
		}

		private void ViewOnHourDownButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Time += new TimeSpan(-1, 0, 0);
			RefreshIfVisible();
		}

		private void ViewOnAmPmTogglePressed(object sender, EventArgs eventArgs)
		{
			m_AmMode = !m_AmMode;
			RefreshIfVisible();
		}

		private void ViewOn24HourTogglePressed(object sender, EventArgs eventArgs)
		{
			m_24HourMode = !m_24HourMode;
			RefreshIfVisible();
		}

		#endregion
	}
}
