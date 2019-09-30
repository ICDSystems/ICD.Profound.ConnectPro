using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	[PresenterBinding(typeof(IMenu2DisplaysPresenter))]
	public sealed class Menu2DisplaysPresenter : AbstractDisplaysPresenter<IMenu2DisplaysView>, IMenu2DisplaysPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private MenuDisplaysPresenterDisplay Display1 { get { return Displays[0]; } }
		private MenuDisplaysPresenterDisplay Display2 { get { return Displays[1]; } }

		private readonly SafeTimer m_DisplayGaugeRefreshTimer;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public Menu2DisplaysPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_DisplayGaugeRefreshTimer = SafeTimer.Stopped(RefreshDisplayStatusGauge);
		}

		protected override void Refresh(IMenu2DisplaysView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				// Display 1
				view.SetDisplay1Color(Display1.Color);
				view.SetDisplay1SourceText(Display1.SourceName);
				view.SetDisplay1Line1Text(Display1.Line1);
				view.SetDisplay1Line2Text(Display1.Line2);
				view.SetDisplay1Icon(Display1.Icon);
				view.ShowDisplay1SpeakerButton(Display1.ShowSpeaker);
				view.SetDisplay1SpeakerButtonActive(Display1.AudioActive);

				// Display 2
				view.SetDisplay2Color(Display2.Color);
				view.SetDisplay2SourceText(Display2.SourceName);
				view.SetDisplay2Line1Text(Display2.Line1);
				view.SetDisplay2Line2Text(Display2.Line2);
				view.SetDisplay2Icon(Display2.Icon);
				view.ShowDisplay2SpeakerButton(Display2.ShowSpeaker);
				view.SetDisplay2SpeakerButtonActive(Display2.AudioActive);
				view.SetDisplay2StatusGauge(Display2.ShowStatusGauge, Display2.DurationGraphValue, Display2.PowerStateText);

				RefreshDisplayStatusGauge(view);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void RefreshDisplayStatusGauge()
		{
			IMenu2DisplaysView view = GetView();

			if (view != null && IsViewVisible)
				RefreshDisplayStatusGauge(view);
		}

		private void RefreshDisplayStatusGauge(IMenu2DisplaysView view)
		{
			m_RefreshSection.Enter();

			try
			{
				bool doTimerRefresh = false;

				// Display1
				bool display1ShowGauge = Display1.ShowStatusGauge;
				view.SetDisplay1StatusGauge(display1ShowGauge, Display1.DurationGraphValue, Display1.PowerStateText);
				doTimerRefresh |= display1ShowGauge;

				// Display2
				bool display2ShowGauge = Display2.ShowStatusGauge;
				view.SetDisplay2StatusGauge(display2ShowGauge, Display2.DurationGraphValue, Display2.PowerStateText);
				doTimerRefresh |= display2ShowGauge;

				if (doTimerRefresh)
					m_DisplayGaugeRefreshTimer.Reset(DISPLAY_GAUGE_REFRESH_INTERVAL);
				else
					m_DisplayGaugeRefreshTimer.Stop();
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
		protected override void Subscribe(IMenu2DisplaysView view)
		{
			base.Subscribe(view);

			view.OnDisplay1ButtonPressed += ViewOnDisplay1ButtonPressed;
			view.OnDisplay1SpeakerButtonPressed += ViewOnDisplay1SpeakerButtonPressed;
			view.OnDisplay2ButtonPressed += ViewOnDisplay2ButtonPressed;
			view.OnDisplay2SpeakerButtonPressed += ViewOnDisplay2SpeakerButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IMenu2DisplaysView view)
		{
			base.Unsubscribe(view);

			view.OnDisplay1ButtonPressed -= ViewOnDisplay1ButtonPressed;
			view.OnDisplay1SpeakerButtonPressed -= ViewOnDisplay1SpeakerButtonPressed;
			view.OnDisplay2ButtonPressed -= ViewOnDisplay2ButtonPressed;
			view.OnDisplay2SpeakerButtonPressed -= ViewOnDisplay2SpeakerButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the display button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDisplay1ButtonPressed(object sender, EventArgs eventArgs)
		{
			DisplayButtonPressed(Displays[0]);
		}

		/// <summary>
		/// Called when the user presses the speaker button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDisplay1SpeakerButtonPressed(object sender, EventArgs eventArgs)
		{
			DisplaySpeakerButtonPressed(Displays[0]);
		}

		/// <summary>
		/// Called when the user presses the display button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDisplay2ButtonPressed(object sender, EventArgs eventArgs)
		{
			DisplayButtonPressed(Displays[1]);
		}

		/// <summary>
		/// Called when the user presses the speaker button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDisplay2SpeakerButtonPressed(object sender, EventArgs eventArgs)
		{
			DisplaySpeakerButtonPressed(Displays[1]);
		}

		#endregion
	}
}
