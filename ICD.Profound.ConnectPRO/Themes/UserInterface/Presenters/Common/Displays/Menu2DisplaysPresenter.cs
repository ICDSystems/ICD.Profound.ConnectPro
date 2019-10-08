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
		private readonly SafeTimer m_DisplayGaugeRefreshTimer;

		private MenuDisplaysPresenterDisplay Display1 { get { return Displays.Count >= 1 ? Displays[0] : null; } }

		private MenuDisplaysPresenterDisplay Display2 { get { return Displays.Count >= 2 ? Displays[1] : null; } }

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
				MenuDisplaysPresenterDisplay display1 = Display1;
				MenuDisplaysPresenterDisplay display2 = Display2;

				// Display 1
				if (display1 != null)
				{
					view.SetDisplay1Color(display1.Color);
					view.SetDisplay1SourceText(display1.SourceName);
					view.SetDisplay1Line1Text(display1.Line1);
					view.SetDisplay1Line2Text(display1.Line2);
					view.SetDisplay1Icon(display1.Icon);
					view.ShowDisplay1SpeakerButton(display1.ShowSpeaker);
					view.SetDisplay1SpeakerButtonActive(display1.AudioActive);
				}

				// Display 2
				if (display2 != null)
				{
					view.SetDisplay2Color(display2.Color);
					view.SetDisplay2SourceText(display2.SourceName);
					view.SetDisplay2Line1Text(display2.Line1);
					view.SetDisplay2Line2Text(display2.Line2);
					view.SetDisplay2Icon(display2.Icon);
					view.ShowDisplay2SpeakerButton(display2.ShowSpeaker);
					view.SetDisplay2SpeakerButtonActive(display2.AudioActive);
				}

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
				MenuDisplaysPresenterDisplay display1 = Display1;
				MenuDisplaysPresenterDisplay display2 = Display2;

				// Display1
				if (display1 != null)
				{
					bool display1ShowGauge = display1.ShowPowerState;
					view.SetDisplay1WarmupStatusText(display1.PowerStateText);
					doTimerRefresh |= display1ShowGauge;
				}

				// Display2
				if (display2 != null)
				{
					bool display2ShowGauge = display2.ShowPowerState;
					view.SetDisplay2WarmupStatusText(display2.PowerStateText);
					doTimerRefresh |= display2ShowGauge;
				}

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
