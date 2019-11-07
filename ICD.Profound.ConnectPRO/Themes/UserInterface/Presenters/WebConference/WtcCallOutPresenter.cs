using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	[PresenterBinding(typeof(IWtcCallOutPresenter))]
	public sealed class WtcCallOutPresenter : AbstractWtcPresenter<IWtcCallOutView>, IWtcCallOutPresenter
	{
		private readonly KeypadStringBuilder m_StringBuilder;
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Gets the zoom traditional control for call out.
		/// </summary>
		[CanBeNull]
		private ZoomRoomTraditionalConferenceControl TraditionalControl
		{
			get { return GetTraditionalConferenceControl(ActiveConferenceControl); }
		}

		private ITraditionalConference ActiveConference
		{
			get
			{
				return TraditionalControl == null
					? null
					: TraditionalControl.GetActiveConference() as ITraditionalConference;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WtcCallOutPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_RefreshSection = new SafeCriticalSection();

			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;
		}

		protected override void Refresh(IWtcCallOutView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				ITraditionalConference active = ActiveConference;
				bool isInCall = active != null;

				string dialText = m_StringBuilder.ToString();
				string callLabel = isInCall ? "END CALL" : "CALL";
				string callStatusLabel = StringUtils.NiceName(active == null ? eConferenceStatus.Disconnected : active.Status);
				bool backEnabled = active == null && dialText.Length > 0;
				bool clearEnabled = active == null && dialText.Length > 0;
				bool callSelected = isInCall;

				view.SetBackButtonEnabled(backEnabled);
				view.SetCallButtonLabel(callLabel);
				view.SetCallStatusLabel(callStatusLabel);
				view.SetCallButtonSelected(callSelected);
				view.SetClearButtonEnabled(clearEnabled);
				view.SetText(dialText);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		/// <summary>
		/// Gets the zoom traditional control for call out from the given conference control.
		/// </summary>
		[CanBeNull]
		private static ZoomRoomTraditionalConferenceControl GetTraditionalConferenceControl(
			[CanBeNull] IWebConferenceDeviceControl control)
		{
			if (control == null)
				return null;

			ZoomRoom device = control.Parent as ZoomRoom;
			return device == null ? null : device.Controls.GetControl<ZoomRoomTraditionalConferenceControl>();
		}

		/// <summary>
		/// Called when the string builder is updated.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StringBuilderOnStringChanged(object sender, StringEventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (IWebConference conference in control.GetConferences())
				Subscribe(conference);

			ZoomRoomTraditionalConferenceControl callOut = GetTraditionalConferenceControl(control);
			if (callOut == null)
				return;

			callOut.OnConferenceAdded += TraditionalControlOnConferenceAdded;
			callOut.OnConferenceRemoved += TraditionalControlOnConferenceRemoved;

			foreach (ITraditionalConference conference in callOut.GetConferences())
				Subscribe(conference);
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			foreach (IWebConference conference in control.GetConferences())
				Unsubscribe(conference);

			ZoomRoomTraditionalConferenceControl callOut = GetTraditionalConferenceControl(control);
			if (callOut == null)
				return;

			callOut.OnConferenceAdded -= TraditionalControlOnConferenceAdded;
			callOut.OnConferenceRemoved -= TraditionalControlOnConferenceRemoved;

			foreach (ITraditionalConference conference in callOut.GetConferences())
				Unsubscribe(conference);
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			RefreshIfVisible();
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			RefreshIfVisible();
		}

		private void TraditionalControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			RefreshIfVisible();
		}

		private void TraditionalControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnParticipantRemoved;
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			conference.OnParticipantAdded -= ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved -= ConferenceOnParticipantRemoved;
			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnParticipantAdded(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnParticipantRemoved(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IWtcCallOutView view)
		{
			base.Subscribe(view);

			view.OnBackButtonPressed += ViewOnBackButtonPressed;
			view.OnCallButtonPressed += ViewOnCallButtonPressed;
			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWtcCallOutView view)
		{
			base.Unsubscribe(view);

			view.OnBackButtonPressed -= ViewOnBackButtonPressed;
			view.OnCallButtonPressed -= ViewOnCallButtonPressed;
			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnKeypadButtonPressed -= ViewOnKeypadButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a keypad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="charEventArgs"></param>
		private void ViewOnKeypadButtonPressed(object sender, CharEventArgs charEventArgs)
		{
			if (ActiveConference == null)
				m_StringBuilder.AppendCharacter(charEventArgs.Data);
			else
				ActiveConference.GetParticipants().ForEach(p => p.SendDtmf(charEventArgs.Data));
		}

		/// <summary>
		/// Called when the user presses the clear button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnClearButtonPressed(object sender, EventArgs eventArgs)
		{
			if (ActiveConference == null)
				m_StringBuilder.Clear();
		}

		/// <summary>
		/// Called when the user presses the call button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCallButtonPressed(object sender, EventArgs eventArgs)
		{
			ZoomRoomTraditionalConferenceControl control = TraditionalControl;
			if (control == null)
				return;

			// Hang up
			ITraditionalConference active = control.GetActiveConference() as ITraditionalConference;
			if (active != null)
			{
				active.Hangup();
				return;
			}

			// Call
			IDialContext dialContext = new PstnDialContext()
			{
				CallType = eCallType.Audio,
				DialString = m_StringBuilder.ToString()
			};
			control.Dial(dialContext);
		}

		/// <summary>
		/// Called when the user presses the back button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnBackButtonPressed(object sender, EventArgs eventArgs)
		{
			m_StringBuilder.Backspace();
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_StringBuilder.Clear();
		}

		#endregion
	}
}