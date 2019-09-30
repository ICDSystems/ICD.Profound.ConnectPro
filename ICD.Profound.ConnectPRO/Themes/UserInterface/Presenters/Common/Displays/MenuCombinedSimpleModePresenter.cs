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

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	[PresenterBinding(typeof(IMenuCombinedSimpleModePresenter))]
	public sealed class MenuCombinedSimpleModePresenter : AbstractDisplaysPresenter<IMenuCombinedSimpleModeView>, IMenuCombinedSimpleModePresenter
	{
		private const eDisplayColor DEFAULT_COLOR = eDisplayColor.White;
		private const string DEFAULT_LINE_1 = "ALL DISPLAYS";
		private const string DEFAULT_LINE_2 = "";
		private const string DEFAULT_SOURCE_NAME = "Mixed Sources";
		private const string DEFAULT_ICON = "";

		public event EventHandler OnAdvancedModePressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private readonly SafeTimer m_DisplayGaugeRefreshTimer;


		public MenuCombinedSimpleModePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
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
				view.SetAdvancedModeEnabled(true);

				// TODO

				eDisplayColor color;
				string icon;
				string line1;
				string line2;
				string sourceName;

				if (!Displays.Select(d => d.Color).Unanimous(out color))
					color = DEFAULT_COLOR;
				string textColor = Colors.DisplayColorToTextColor(color);

				if (!Displays.Select(d => d.Icon).Unanimous(out icon))
					icon = Icons.GetDisplayIcon(DEFAULT_ICON, eDisplayColor.White);

				if (!Displays.Select(d => d.Line1).Unanimous(out line1))
					line1 = HtmlUtils.FormatColoredText(DEFAULT_LINE_1, textColor);

				if (!Displays.Select(d => d.Line2).Unanimous(out line2))
					line2 = HtmlUtils.FormatColoredText(DEFAULT_LINE_2, textColor);
				
				if (!Displays.Select(d => d.SourceName).Unanimous(out sourceName))
					sourceName = HtmlUtils.FormatColoredText(DEFAULT_SOURCE_NAME, textColor);

				view.SetDisplayColor(color);
				view.SetDisplayIcon(icon);
				view.SetDisplayLine1Text(line1);
				view.SetDisplayLine2Text(line2);
				view.SetDisplaySourceText(sourceName);
				view.SetDisplaySpeakerButtonActive(Displays.Any(d => d.AudioActive));

				RefreshDisplayStatusGauge(view);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void RefreshDisplayStatusGauge()
		{
			IMenuCombinedSimpleModeView view = GetView();

			if (view != null && IsViewVisible)
				RefreshDisplayStatusGauge(view);
		}

		private void RefreshDisplayStatusGauge(IMenuCombinedSimpleModeView view)
		{
			MenuDisplaysPresenterDisplay display = Displays[0];

			if (display == null)
				return;

			m_RefreshSection.Enter();

			try
			{
				bool displayShowGauge = display.ShowStatusGauge;
				view.SetDisplayStatusGauge(displayShowGauge, display.DurationGraphValue, display.PowerStateText);

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
