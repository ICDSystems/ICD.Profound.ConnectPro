using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Notifications;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Notifications;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Notifications
{
	[PresenterBinding(typeof(IIncomingCallPresenter))]
	public sealed class IncomingCallPresenter : AbstractTouchDisplayPresenter<IIncomingCallView>, IIncomingCallPresenter
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
		public IncomingCallPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_CallIgnoredTimer = new SafeTimer(RemoveSource, -1L);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IIncomingCallView view)
		{
			base.Refresh(view);

			if (IncomingCall == null)
				return;
			
			string info = IncomingCall.Name == null ? IncomingCall.Number : string.Format("{0} - {1}", IncomingCall.Name, IncomingCall.Number);

			if (IncomingCall.GetIsRingingIncomingCall())
			{
				view.SetIcon("call");
				view.SetCallerInfo(string.Format("Press to answer incoming call from {0}", info));
				view.SetAnswerButtonMode(eIncomingCallAnswerButtonMode.Ringing);
				view.PlayRingtone(true);
				view.SetRejectButtonVisibility(true);
			}
			else if (IncomingCall.AnswerState == eCallAnswerState.Ignored)
			{
				view.SetIcon("hangup");
				view.SetCallerInfo(string.Format("Call from {0} was declined", info));
				view.SetAnswerButtonMode(eIncomingCallAnswerButtonMode.Rejected);
				view.PlayRingtone(false);
				view.SetRejectButtonVisibility(false);
			}
		}

		#region Room Callbacks

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

		#endregion

		#region Incoming Call Callbacks

		private void Subscribe(IIncomingCall source)
		{
			source.OnAnswerStateChanged += SourceOnAnswerStateChanged;
		}

		private void Unsubscribe(IIncomingCall source)
		{
			source.OnAnswerStateChanged -= SourceOnAnswerStateChanged;
		}

		private void SourceOnAnswerStateChanged(object sender, IncomingCallAnswerStateEventArgs e)
		{
			if (IncomingCall.AnswerState == eCallAnswerState.Answered ||
			    IncomingCall.AnswerState == eCallAnswerState.Autoanswered)
				RemoveSource();
			else if (IncomingCall.AnswerState == eCallAnswerState.Ignored)
				m_CallIgnoredTimer.Reset(REJECTED_LINGER_TIME_MS);

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IIncomingCallView view)
		{
			base.Subscribe(view);

			view.OnAnswerButtonPressed += ViewOnAnswerButtonPressed;
			view.OnRejectButtonPressed += ViewOnRejectButtonPressed;
		}

		protected override void Unsubscribe(IIncomingCallView view)
		{
			base.Unsubscribe(view);

			view.OnAnswerButtonPressed -= ViewOnAnswerButtonPressed;
			view.OnRejectButtonPressed -= ViewOnRejectButtonPressed;
		}

		private void ViewOnAnswerButtonPressed(object sender, EventArgs e)
		{
			if (IncomingCall == null)
				return;

			IncomingCall.Answer();
		}

		private void ViewOnRejectButtonPressed(object sender, EventArgs e)
		{
			if (IncomingCall == null)
				return;

			IncomingCall.Reject();
		}

		#endregion
	}
}