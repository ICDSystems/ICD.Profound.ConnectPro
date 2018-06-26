using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.ActiveCalls
{
	public sealed class VtcReferencedActiveCallsPresenter : AbstractComponentPresenter<IVtcReferencedActiveCallsView>,
	                                                   IVtcReferencedActiveCallsPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_RefreshTimer;

		private IConferenceSource m_Source;

		/// <summary>
		/// Sets the conference source for this presenter.
		/// </summary>
		/// <value></value>
		public IConferenceSource ConferenceSource
		{
			get { return m_Source; }
			set
			{
				if (value == m_Source)
					return;

				Unsubscribe(m_Source);
				m_Source = value;
				Subscribe(m_Source);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcReferencedActiveCallsPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_RefreshTimer = new SafeTimer(RefreshIfVisible, 1 * 1000);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_RefreshTimer.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcReferencedActiveCallsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string name =
					m_Source == null
						? string.Empty
						: string.Format("{0:hh\\:mm\\:ss} - {1} - {2}", m_Source.GetDuration(), m_Source.Name, m_Source.Number);

				view.SetLabel(name);
				view.SetHangupButtonVisible(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Source Callbacks

		/// <summary>
		/// Subscribe to the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Subscribe(IConferenceSource source)
		{
			if (source == null)
				return;

			source.OnNameChanged += SourceOnNameChanged;
			source.OnNumberChanged += SourceOnNumberChanged;
		}

		/// <summary>
		/// Unsubscribe from the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Unsubscribe(IConferenceSource source)
		{
			if (source == null)
				return;

			source.OnNameChanged -= SourceOnNameChanged;
			source.OnNumberChanged -= SourceOnNumberChanged;
		}

		private void SourceOnNumberChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		private void SourceOnNameChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IVtcReferencedActiveCallsView view)
		{
			base.Subscribe(view);

			view.OnHangupButtonPressed += ViewOnHangupButtonPressed;
		}

		protected override void Unsubscribe(IVtcReferencedActiveCallsView view)
		{
			base.Unsubscribe(view);

			view.OnHangupButtonPressed -= ViewOnHangupButtonPressed;
		}

		private void ViewOnHangupButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_Source != null)
				m_Source.Hangup();
		}

		#endregion
	}
}
