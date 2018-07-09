using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	public sealed class SettingsSystemPowerPresenter : AbstractPresenter<ISettingsSystemPowerView>, ISettingsSystemPowerPresenter
	{
		private bool m_Weekend;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsSystemPowerPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsSystemPowerView view)
		{
			base.Refresh(view);
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
			throw new NotImplementedException();
		}

		private void ViewOnWeekendsButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnWakeHourIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnWakeHourDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnWakeMinuteIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnWakeMinuteDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnSleepHourIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnSleepHourDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnSleepMinuteIncrementButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnSleepMinuteDecrementButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnEnableButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnDisableButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
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
