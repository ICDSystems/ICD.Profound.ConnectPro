using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	[PresenterBinding(typeof(IReferencedDisplayPresenter))]
	public sealed class ReferencedDisplayPresenter : AbstractUiComponentPresenter<IReferencedDisplayView>, IReferencedDisplayPresenter
	{
		private const long DISPLAY_GAUGE_REFRESH_INTERVAL = 250;

		public event EventHandler OnDisplayPressed;
		public event EventHandler OnDisplaySpeakerPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private readonly SafeTimer m_DisplayGaugeRefreshTimer;

		[CanBeNull]
		public MenuDisplaysPresenterDisplay Model { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ReferencedDisplayPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_DisplayGaugeRefreshTimer = SafeTimer.Stopped(RefreshDisplayStatusGauge);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IReferencedDisplayView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				eDisplayColor color = Model == null ? eDisplayColor.Grey : Model.Color;
				string icon = Model == null ? string.Empty : Model.Icon;
				string sourceName = Model == null ? string.Empty : Model.SourceName;
				string line1 = Model == null ? string.Empty : Model.Line1;
				string line2 = Model == null ? string.Empty : Model.Line2;
				bool audioActive = Model != null && Model.AudioActive;
				bool showSpeaker = Model != null && Model.ShowSpeaker;

				view.SetDisplayColor(color);
				view.SetDisplayIcon(icon);
				view.SetDisplaySourceText(sourceName);
				view.SetDisplayLine1Text(line1);
				view.SetDisplayLine2Text(line2);
				view.SetDisplaySpeakerButtonActive(audioActive);
				view.ShowDisplaySpeakerButton(showSpeaker);

				RefreshDisplayStatusGauge(view);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void RefreshDisplayStatusGauge()
		{
			IReferencedDisplayView view = GetView();

			if (view != null && IsViewVisible)
				RefreshDisplayStatusGauge(view);
		}

		private void RefreshDisplayStatusGauge(IReferencedDisplayView view)
		{
			m_RefreshSection.Enter();

			try
			{
				bool showGauge = Model != null && Model.ShowPowerState;
				string text = Model == null ? string.Empty : Model.PowerStateText;

				view.SetWarmupStatusText(text);

				if (showGauge)
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
		protected override void Subscribe(IReferencedDisplayView view)
		{
			base.Subscribe(view);

			view.OnDisplayButtonPressed += ViewOnDisplayButtonPressed;
			view.OnDisplaySpeakerButtonPressed += ViewOnDisplaySpeakerButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedDisplayView view)
		{
			base.Subscribe(view);

			view.OnDisplayButtonPressed -= ViewOnDisplayButtonPressed;
			view.OnDisplaySpeakerButtonPressed -= ViewOnDisplaySpeakerButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the display button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnDisplayButtonPressed(object sender, EventArgs e)
		{
			OnDisplayPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the speaker button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnDisplaySpeakerButtonPressed(object sender, EventArgs e)
		{
			OnDisplaySpeakerPressed.Raise(this);
		}

		#endregion
	}
}
