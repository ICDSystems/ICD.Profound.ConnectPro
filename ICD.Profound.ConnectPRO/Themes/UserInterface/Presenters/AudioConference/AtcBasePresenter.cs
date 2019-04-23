using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Devices;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.AudioConference
{
	[PresenterBinding(typeof(IAtcBasePresenter))]
	public sealed class AtcBasePresenter : AbstractPopupPresenter<IAtcBaseView>, IAtcBasePresenter
	{
		private readonly KeypadStringBuilder m_Builder;
		private readonly SafeCriticalSection m_RefreshSection;

		private ITraditionalConferenceDeviceControl m_ActiveConferenceControl;

		public ITraditionalConferenceDeviceControl ActiveConferenceControl
		{
			get { return m_ActiveConferenceControl; }
			set
			{
				if (m_ActiveConferenceControl == value)
					return;

				Unsubscribe(m_ActiveConferenceControl);
				m_ActiveConferenceControl = value;
				Subscribe(m_ActiveConferenceControl);
			}
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public AtcBasePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_Builder = new KeypadStringBuilder();
			m_RefreshSection = new SafeCriticalSection();

			m_Builder.OnStringChanged += BuilderOnStringChanged;
		}

		/// <summary>
		/// Called when the dial string updates.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void BuilderOnStringChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IAtcBaseView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IParticipant active = GetActiveSource();
				eParticipantStatus status = active == null ? eParticipantStatus.Disconnected : active.Status;

				string atcNumber = Room == null ? string.Empty : Room.AtcNumber;
				string activeStatus = StringUtils.NiceName(status);
				string dialString = m_Builder.ToString();
				bool inACall = active != null;

				view.SetRoomNumber(atcNumber);
				view.SetDialNumber(string.IsNullOrEmpty(dialString) && active == null ? "Dial Number" : dialString);
				view.SetCallStatus(activeStatus);

				view.SetClearButtonEnabled(dialString.Length > 0 && !inACall);
				view.SetDialButtonEnabled(dialString.Length > 0 && !inACall);
				view.SetHangupButtonEnabled(inACall);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Gets the active conference source.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IParticipant GetActiveSource()
		{
			return
				ActiveConferenceControl == null
					? null
					: ActiveConferenceControl.GetConferences()
						.SelectMany(c => c.GetParticipants())
						.FirstOrDefault(s => s.GetIsActive());
		}

		[CanBeNull]
		private IConference GetActiveConference()
		{
			return ActiveConferenceControl == null
				? null
				: ActiveConferenceControl.GetConferences()
					.FirstOrDefault(c => 
						c.Status == eConferenceStatus.Connected && 
						c.GetParticipants().Any(p => p.GetIsActive()));
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

			room.Routing.OnAudioSourceChanged += RoutingOnAudioSourceChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);
			
			if (room == null)
				return;

			room.Routing.OnAudioSourceChanged -= RoutingOnAudioSourceChanged;
		}

		private void RoutingOnAudioSourceChanged(object sender, EventArgs e)
		{
			ActiveConferenceControl =
				Room == null
					? null
					: Room.Routing
					      .GetCachedActiveAudioSources()
					      .Select(s => Room.Core.Originators.GetChild<IDeviceBase>(s.Device))
					      .SelectMany(d => d.Controls.GetControls<ITraditionalConferenceDeviceControl>())
					      .FirstOrDefault(c => c != null && c.Supports == eCallType.Audio);
		}

		#endregion

		#region Conference Control Callbacks

		private void Subscribe(ITraditionalConferenceDeviceControl dialer)
		{
			if (dialer == null)
				return;

			dialer.OnConferenceAdded += AudioDialerOnConferenceAdded;
			dialer.OnConferenceRemoved += AudioDialerOnConferenceRemoved;

			foreach (var conference in dialer.GetConferences())
				Subscribe(conference);
		}

		private void Unsubscribe(ITraditionalConferenceDeviceControl dialer)
		{
			if (dialer == null)
				return;

			dialer.OnConferenceAdded -= AudioDialerOnConferenceAdded;
			dialer.OnConferenceRemoved -= AudioDialerOnConferenceRemoved;

			foreach (var conference in dialer.GetConferences())
				Unsubscribe(conference);
		}

		private void AudioDialerOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data);
			RefreshIfVisible();
		}

		private void AudioDialerOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			// When the source goes offline we clear the dial string
			if (GetActiveSource() == null)
				m_Builder.Clear();

			Unsubscribe(e.Data);
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			if (conference == null)
				return;

            conference.OnParticipantAdded += ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnParticipantRemoved;
		}

		private void Unsubscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnParticipantAdded -= ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved -= ConferenceOnParticipantRemoved;
		}

		private void ConferenceOnParticipantAdded(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnParticipantRemoved(object sender, ParticipantEventArgs participantEventArgs)
		{
			// When the source goes offline we clear the dial string
			if (GetActiveSource() == null)
				m_Builder.Clear();

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IAtcBaseView view)
		{
			base.Subscribe(view);

			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnDialButtonPressed += ViewOnDialButtonPressed;
			view.OnHangupButtonPressed += ViewOnHangupButtonPressed;
			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IAtcBaseView view)
		{
			base.Unsubscribe(view);

			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnDialButtonPressed -= ViewOnDialButtonPressed;
			view.OnHangupButtonPressed -= ViewOnHangupButtonPressed;
			view.OnKeypadButtonPressed -= ViewOnKeypadButtonPressed;
		}

		private void ViewOnKeypadButtonPressed(object sender, CharEventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			// DTMF
			IConference conference = GetActiveConference();
			IEnumerable<ITraditionalParticipant> participants =
				conference == null
					? Enumerable.Empty<ITraditionalParticipant>()
					: conference.GetParticipants()
					            .Where(s => s.GetIsOnline())
					            .OfType<ITraditionalParticipant>();

			foreach (ITraditionalParticipant source in participants)
				source.SendDtmf(eventArgs.Data);

			m_Builder.AppendCharacter(eventArgs.Data);
		}

		private void ViewOnHangupButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			var activeConference = GetActiveConference();

			var traditionalConference = activeConference as ITraditionalConference;
			if (traditionalConference != null)
				traditionalConference.Hangup();
		}

		private void ViewOnDialButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConferenceControl == null)
				return;

			string dialString = m_Builder.ToString();
			if (string.IsNullOrEmpty(dialString))
				return;

			var dialContext = new GenericDialContext { DialString = dialString, CallType = eCallType.Audio };
			if (ActiveConferenceControl.CanDial(dialContext) != eDialContextSupport.Unsupported)
				ActiveConferenceControl.Dial(dialContext);
		}

		private void ViewOnClearButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Builder.Clear();
		}

		#endregion
	}
}
