using System;
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

		public MenuDisplaysPresenterDisplay Model { get; set; }

		public ReferencedDisplayPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_DisplayGaugeRefreshTimer = SafeTimer.Stopped(RefreshDisplayStatusGauge);
		}

		protected override void Refresh(IReferencedDisplayView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetDisplayColor(Model.Color);
				view.SetDisplayIcon(Model.Icon);
				view.SetDisplaySourceText(Model.SourceName);
				view.SetDisplayLine1Text(Model.Line1);
				view.SetDisplayLine2Text(Model.Line2);
				view.SetDisplaySpeakerButtonActive(Model.AudioActive);
				view.ShowDisplaySpeakerButton(Model.ShowSpeaker);

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
				bool displayShowGauge = Model.ShowStatusGauge;
				view.SetDisplayStatusGauge(displayShowGauge, Model.DurationGraphValue, Model.PowerStateText);

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

		protected override void Subscribe(IReferencedDisplayView view)
		{
			base.Subscribe(view);

			view.OnDisplayButtonPressed += ViewOnDisplayButtonPressed;
			view.OnDisplaySpeakerButtonPressed += ViewOnDisplaySpeakerButtonPressed;
		}

		protected override void Unsubscribe(IReferencedDisplayView view)
		{
			base.Subscribe(view);

			view.OnDisplayButtonPressed -= ViewOnDisplayButtonPressed;
			view.OnDisplaySpeakerButtonPressed -= ViewOnDisplaySpeakerButtonPressed;
		}

		private void ViewOnDisplayButtonPressed(object sender, EventArgs e)
		{
			OnDisplayPressed.Raise(this);
		}

		private void ViewOnDisplaySpeakerButtonPressed(object sender, EventArgs e)
		{
			OnDisplaySpeakerPressed.Raise(this);
		}

		#endregion
	}
}
