using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.IncomingCalls;
using ICD.Connect.Conferencing.Participants.Enums;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.FooterNotifications;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.FooterNotifications;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.FooterNotifications
{
	[PresenterBinding(typeof(IOsdIncomingCallFooterNotificationPresenter))]
	public sealed class OsdIncomingCallFooterNotificationPresenter : AbstractOsdPresenter<IOsdIncomingCallFooterNotificationView>, IOsdIncomingCallFooterNotificationPresenter
	{
		private const long REJECTED_LINGER_TIME_MS = 4 * 1000;
		private readonly SafeTimer m_CallIgnoredTimer;

		private IEnumerable<IConferenceDeviceControl> m_DialingProviders;

		private IIncomingCall m_IncomingCall;

		private IIncomingCall IncomingCall
		{
			get { return m_IncomingCall; }
			set
			{
				if (m_IncomingCall == value)
					return;

				if (m_IncomingCall != null)
					Unsubscribe(m_IncomingCall);

				m_IncomingCall = value;

				if (m_IncomingCall != null)
					Subscribe(m_IncomingCall);

				m_CallIgnoredTimer.Stop();

				ShowView(IncomingCall != null);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdIncomingCallFooterNotificationPresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme) : base(nav, views, theme)
		{
			m_CallIgnoredTimer = SafeTimer.Stopped(RemoveSource);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IOsdIncomingCallFooterNotificationView view)
		{
			base.Refresh(view);

			if (IncomingCall == null)
				return;

			if (IncomingCall.GetIsRingingIncomingCall())
			{
				string info = IncomingCall.Name == null ? IncomingCall.Number : string.Format("{0} - {1}", IncomingCall.Name, IncomingCall.Number);
				view.SetIcon("call");
				view.SetCallerInfo(string.Format("Incoming Call from {0}", info));
				view.SetBackgroundMode(eOsdIncomingCallBackgroundMode.Ringing);
				//view.PlayRingtone(true);
			}
			else if (IncomingCall.AnswerState == eCallAnswerState.Ignored)
			{
				view.SetIcon("hangup");
				view.SetCallerInfo("Call Was Declined");
				view.SetBackgroundMode(eOsdIncomingCallBackgroundMode.Rejected);
				//view.PlayRingtone(false);
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

			m_DialingProviders = room.ConferenceManager.Dialers.GetDialingProviders();
			
			if (m_DialingProviders == null)
				return;

			foreach (var dialer in m_DialingProviders)
			{
				dialer.OnIncomingCallAdded += DialerOnIncomingCallAdded;
				dialer.OnIncomingCallRemoved += DialerOnIncomingCallRemoved;
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
				dialer.OnIncomingCallAdded -= DialerOnIncomingCallAdded;
				dialer.OnIncomingCallRemoved -= DialerOnIncomingCallRemoved;
			}

			m_DialingProviders = null;
		}

		private void DialerOnIncomingCallAdded(object sender, GenericEventArgs<IIncomingCall> args)
		{
			UpdateSource(args.Data);
		}

		private void DialerOnIncomingCallRemoved(object sender, GenericEventArgs<IIncomingCall> args)
		{
			if (args.Data == IncomingCall)
				RemoveSource();
		}

		private void UpdateSource(IIncomingCall source)
		{
			if (source != null && source.GetIsRingingIncomingCall())
				IncomingCall = source;
		}

		private void RemoveSource()
		{
			IncomingCall = null;
		}

		private void Subscribe(IIncomingCall source)
		{
			source.OnAnswerStateChanged += SourceOnAnswerStateChanged;
		}

		private void Unsubscribe(IIncomingCall source)
		{
			source.OnAnswerStateChanged -= SourceOnAnswerStateChanged;
		}

		private void SourceOnAnswerStateChanged(object sender, CallAnswerStateEventArgs e)
		{
			if (IncomingCall.AnswerState == eCallAnswerState.Answered ||
			    IncomingCall.AnswerState == eCallAnswerState.AutoAnswered)
				RemoveSource();
			else if (IncomingCall.AnswerState == eCallAnswerState.Ignored)
				m_CallIgnoredTimer.Reset(REJECTED_LINGER_TIME_MS);

			RefreshIfVisible();
		}
	}
}