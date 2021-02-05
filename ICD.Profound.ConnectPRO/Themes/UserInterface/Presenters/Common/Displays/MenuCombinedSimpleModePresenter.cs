using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	[PresenterBinding(typeof(IMenuCombinedSimpleModePresenter))]
	public sealed class MenuCombinedSimpleModePresenter : AbstractDisplaysPresenter<IMenuCombinedSimpleModeView>, IMenuCombinedSimpleModePresenter
	{
		private const eDisplayColor DEFAULT_COLOR = eDisplayColor.White;
		private const string DEFAULT_LINE_1 = "ALL DISPLAYS";
		private const string DEFAULT_SOURCE_NAME = "Mixed Sources";
		private const string DEFAULT_ICON = "";

		public event EventHandler OnAdvancedModePressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private readonly SafeTimer m_DisplayGaugeRefreshTimer;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public MenuCombinedSimpleModePresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_DisplayGaugeRefreshTimer = SafeTimer.Stopped(RefreshDisplayStatusGauge);
		}

		protected override void Refresh(IMenuCombinedSimpleModeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				// Advanced Mode button
				bool advancedModeEnabled = Displays.Count > 1;
				view.SetAdvancedModeEnabled(advancedModeEnabled);

				// Color
				eDisplayColor color;
				if (!Displays.Select(d => d.Color).Unanimous(out color))
					color = DEFAULT_COLOR;
				string textColor = Colors.DisplayColorToTextColor(color);

				// Icon
				string icon;
				if (!Displays.Select(d => d.Icon).Unanimous(out icon))
					icon = Icons.GetDisplayIcon(DEFAULT_ICON, color);

				// Label
				string line1 = HtmlUtils.FormatColoredText(DEFAULT_LINE_1, textColor);

				// Source name
				string sourceName;
				if (!Displays.Select(d => d.SourceName).Unanimous(out sourceName))
					sourceName = HtmlUtils.FormatColoredText(DEFAULT_SOURCE_NAME, textColor);

				view.SetDisplayColor(color);
				view.SetDisplayIcon(icon);
				view.SetDisplayLine1Text(line1);
				view.SetDisplaySourceText(sourceName);
				view.SetDisplaySpeakerButtonActive(Displays.Any(d => d.AudioActive));

				RefreshDisplayStatusGauge(view);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private void RefreshDisplayStatusGauge()
		{
			IMenuCombinedSimpleModeView view = GetView();

			if (view != null && IsViewVisible)
				RefreshDisplayStatusGauge(view);
		}

		private void RefreshDisplayStatusGauge(IMenuCombinedSimpleModeView view)
		{
			if (Displays.Count < 1)
				return;

			MenuDisplaysPresenterDisplay display = Displays[0];

			if (display == null)
				return;

			m_RefreshSection.Enter();

			try
			{
				bool displayShowGauge = display.ShowPowerState;
				view.SetDisplayWarmupStatusText(display.PowerStateText);

				if (displayShowGauge)
					m_DisplayGaugeRefreshTimer.Reset(DISPLAY_GAUGE_REFRESH_INTERVAL);
				else
					m_DisplayGaugeRefreshTimer.Stop();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IMenuCombinedSimpleModeView view)
		{
			base.Subscribe(view);

			view.OnAdvancedModeButtonPressed += ViewOnAdvancedModeButtonPressed;
			view.OnDisplayButtonPressed += ViewOnDisplayButtonPressed;
			view.OnDisplaySpeakerButtonPressed += ViewOnDisplaySpeakerButtonPressed;
		}

		protected override void Unsubscribe(IMenuCombinedSimpleModeView view)
		{
			base.Unsubscribe(view);

			view.OnAdvancedModeButtonPressed -= ViewOnAdvancedModeButtonPressed;
			view.OnDisplayButtonPressed -= ViewOnDisplayButtonPressed;
			view.OnDisplaySpeakerButtonPressed -= ViewOnDisplaySpeakerButtonPressed;
		}

		private void ViewOnAdvancedModeButtonPressed(object sender, EventArgs args)
		{
			if (Displays.Count < 2)
				return;

			OnAdvancedModePressed.Raise(this);
		}

		private void ViewOnDisplayButtonPressed(object sender, EventArgs args)
		{
			if (!Displays.Any())
				return;

			DisplayButtonPressed(Displays[0]);
		}

		private void ViewOnDisplaySpeakerButtonPressed(object sender, EventArgs args)
		{
			if (!Displays.Any())
				return;

			DisplaySpeakerButtonPressed(Displays[0]);
		}

		#endregion
	}
}
