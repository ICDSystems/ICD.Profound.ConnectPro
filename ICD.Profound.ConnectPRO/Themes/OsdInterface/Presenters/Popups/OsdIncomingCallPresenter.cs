using System.Collections.Generic;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Popups
{
	public sealed class OsdIncomingCallPresenter : AbstractOsdPresenter<IOsdIncomingCallView>, IOsdIncomingCallPresenter
	{
		private const long REJECTED_LINGER_TIME_MS = 4 * 1000;
		private readonly SafeTimer m_CallIgnoredTimer;

		private IEnumerable<IDialingDeviceControl> m_DialingProviders;

		private IConferenceSource m_Source;

		private IConferenceSource Source
		{
			get { return m_Source; }
			set
			{
				if (m_Source == value)
					return;

				if (m_Source != null)
					Unsubscribe(m_Source);

				m_Source = value;

				if (m_Source != null)
					Subscribe(m_Source);

				m_CallIgnoredTimer.Stop();

				ShowView(Source != null);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdIncomingCallPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_CallIgnoredTimer = new SafeTimer(RemoveSource, -1L);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IOsdIncomingCallView view)
		{
			base.Refresh(view);

			if (Source == null)
				return;

			if (Source.GetIsRingingIncomingCall())
			{
				string info = Source.Name == null ? Source.Number : string.Format("{0} - {1}", Source.Name, Source.Number);
				view.SetIcon("call");
				view.SetCallerInfo(string.Format("Incoming Call from {0}", info));
				view.SetBackgroundMode(eOsdIncomingCallBackgroundMode.Ringing);
			}
			else if (Source.AnswerState == eConferenceSourceAnswerState.Ignored)
			{
				view.SetIcon("hangup");
				view.SetCallerInfo("Call Was Declined");
				view.SetBackgroundMode(eOsdIncomingCallBackgroundMode.Rejected);
			}
		}

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			m_DialingProviders = room.ConferenceManager.GetDialingProviders();
			
			if (m_DialingProviders == null)
				return;

			foreach (var dialer in m_DialingProviders)
			{
				dialer.OnSourceAdded += DialerOnSourceAdded;
				dialer.OnSourceChanged += DialerOnSourceChanged;
			}
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_DialingProviders == null)
				return;

			foreach (var dialer in m_DialingProviders)
			{
				dialer.OnSourceAdded -= DialerOnSourceAdded;
				dialer.OnSourceChanged -= DialerOnSourceChanged;
			}

			m_DialingProviders = null;
		}

		private void DialerOnSourceAdded(object sender, ConferenceSourceEventArgs args)
		{
			UpdateSource(args.Data);
		}

		private void DialerOnSourceChanged(object sender, ConferenceSourceEventArgs args)
		{
			UpdateSource(args.Data);
		}

		private void UpdateSource(IConferenceSource source)
		{
			if (source != null && source.GetIsRingingIncomingCall())
				Source = source;
		}

		private void RemoveSource()
		{
			Source = null;
		}

		private void Subscribe(IConferenceSource source)
		{
			source.OnAnswerStateChanged += SourceOnAnswerStateChanged;
		}

		private void Unsubscribe(IConferenceSource source)
		{
			source.OnAnswerStateChanged -= SourceOnAnswerStateChanged;
		}

		private void SourceOnAnswerStateChanged(object sender, ConferenceSourceAnswerStateEventArgs e)
		{
			if (Source.AnswerState == eConferenceSourceAnswerState.Answered ||
			    Source.AnswerState == eConferenceSourceAnswerState.Autoanswered)
				RemoveSource();
			else if (Source.AnswerState == eConferenceSourceAnswerState.Ignored)
				m_CallIgnoredTimer.Reset(REJECTED_LINGER_TIME_MS);

			RefreshIfVisible();
		}
	}
}