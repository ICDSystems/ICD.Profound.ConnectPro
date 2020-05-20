using System;
using System.Collections.Generic;
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
using ICD.Connect.Conferencing.Zoom.Components.TraditionalCall;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.Conferencing.Zoom.Responses;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Utils;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(ICallOutConferencePresenter))]
	public sealed class CallOutConferencePresenter : AbstractConferencePresenter<ICallOutConferenceView>, ICallOutConferencePresenter
	{
		private const string DEFAULT_FAIL_MESSAGE = "Call out failed.";
		private const string DISCONNECTED_MESSAGE = "Use the keypad below to call out";

		private static readonly Dictionary<eZoomPhoneCallTerminatedReason, string> s_CallOutFailedMessages =
			new Dictionary<eZoomPhoneCallTerminatedReason, string>
			{
				{
					eZoomPhoneCallTerminatedReason.ByInitAudioDeviceFailed,
					"This Zoom Room is not configured for placing phone calls."
				}
			};

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
		public CallOutConferencePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_RefreshSection = new SafeCriticalSection();

			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;
		}

		protected override void Refresh(ICallOutConferenceView conferenceView)
		{
			base.Refresh(conferenceView);

			m_RefreshSection.Enter();

			try
			{
				ITraditionalConference active = ActiveConference;
				bool isInCall = active != null;

				// TODO
				//if (isInCall)
				//	Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>().ShowView(false);

				string dialText = m_StringBuilder.ToString();
				string callLabel = isInCall ? "END CALL" : "CALL";
				string callStatusLabel = active == null ? DISCONNECTED_MESSAGE : StringUtils.NiceName(active.Status);
				bool backEnabled = active == null && dialText.Length > 0;
				bool clearEnabled = active == null && dialText.Length > 0;
				bool callEnabled = dialText.Length > 0;
				bool callSelected = isInCall;

				conferenceView.SetBackButtonEnabled(backEnabled);
				conferenceView.SetCallButtonLabel(callLabel);
				conferenceView.SetCallStatusLabel(callStatusLabel);
				conferenceView.SetCallButtonEnabled(callEnabled);
				conferenceView.SetCallButtonSelected(callSelected);
				conferenceView.SetClearButtonEnabled(clearEnabled);
				conferenceView.SetText(dialText);
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
			[CanBeNull] IConferenceDeviceControl control)
		{
			if (control == null)
				return null;

			ZoomRoom device = control.Parent as ZoomRoom;
			return device == null ? null : device.Controls.GetControl<ZoomRoomTraditionalConferenceControl>();
		}

		/// <summary>
		/// Gets the zoom traditional call component from the given conference control.
		/// </summary>
		[CanBeNull]
		private TraditionalCallComponent GetCallComponent([CanBeNull] ZoomRoomTraditionalConferenceControl control)
		{
			return control == null ? null : control.Parent.Components.GetComponent<TraditionalCallComponent>();
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

		private void ShowCallFailedDialog(string message)
		{
			// Hide the call out loading spinner.
			// TODO
			//Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>().ShowView(false);

			// Clear the failed call out string as we show it to the user.
			message = string.Format("Call out to {0} failed\n\n{1}", m_StringBuilder.Pop(), message);

			// Hide the error message after 8 seconds.
			const long timeout = 8 * 1000;

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
					  .Show(message, timeout, GenericAlertPresenterButton.Dismiss);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (IConference conference in control.GetConferences())
				Subscribe(conference);

			ZoomRoomTraditionalConferenceControl callOut = GetTraditionalConferenceControl(control);
			if (callOut == null)
				return;

			callOut.OnConferenceAdded += ControlOnConferenceAdded;
			callOut.OnConferenceRemoved += TraditionalControlOnConferenceRemoved;
			callOut.OnCallOutFailed += TraditionalControlOnCallOutFailed;

			foreach (ITraditionalConference conference in callOut.GetConferences())
				Subscribe(conference);

			TraditionalCallComponent callComponent = GetCallComponent(callOut);
			if (callComponent != null)
				callComponent.OnCallFailed += CallComponentOnCallFailed;
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(IConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			foreach (IConference conference in control.GetConferences())
				Unsubscribe(conference);

			ZoomRoomTraditionalConferenceControl callOut = GetTraditionalConferenceControl(control);
			if (callOut == null)
				return;

			callOut.OnConferenceAdded -= ControlOnConferenceAdded;
			callOut.OnConferenceRemoved -= TraditionalControlOnConferenceRemoved;
			callOut.OnCallOutFailed -= TraditionalControlOnCallOutFailed;

			foreach (ITraditionalConference conference in callOut.GetConferences())
				Unsubscribe(conference);

			TraditionalCallComponent callComponent = GetCallComponent(callOut);
			if (callComponent != null)
				callComponent.OnCallFailed -= CallComponentOnCallFailed;
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

		private void TraditionalControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			if (ActiveConference == null)
				m_StringBuilder.Clear();

			Unsubscribe(args.Data);
			RefreshIfVisible();
		}

		private void TraditionalControlOnCallOutFailed(object sender, GenericEventArgs<TraditionalZoomPhoneCallInfo> eventArgs)
		{
			ShowCallFailedDialog(DEFAULT_FAIL_MESSAGE);
		}

		private void CallComponentOnCallFailed(object sender, GenericEventArgs<PhoneCallTerminated> eventArgs)
		{
			string message = s_CallOutFailedMessages.GetDefault(eventArgs.Data.Reason, DEFAULT_FAIL_MESSAGE);
			ShowCallFailedDialog(message);
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
			if (ActiveConference == null)
				m_StringBuilder.Clear();

			RefreshIfVisible();
		}

		private void ConferenceOnParticipantAdded(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnParticipantRemoved(object sender, ParticipantEventArgs participantEventArgs)
		{
			if (ActiveConference == null)
				m_StringBuilder.Clear();

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ICallOutConferenceView view)
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
		protected override void Unsubscribe(ICallOutConferenceView view)
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
			if (ActiveConference == null && !(charEventArgs.Data == '#' || charEventArgs.Data == '*'))
				m_StringBuilder.AppendCharacter(charEventArgs.Data);

			// DTMF
			if (ActiveConference != null)
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

			// TODO
			//Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>()
			//          .ShowView("Starting Zoom Audio Call", 10 * 1000);

			// Call
			IDialContext dialContext = new DialContext
			{
				Protocol = eDialProtocol.Pstn,
				CallType = eCallType.Audio,
				DialString = m_StringBuilder.ToString()
			};
			try
			{
				control.Dial(dialContext);
			}

			// If an empty dial string or a dial string containing # OR * is dialed
			// Zoom fails but doesn't give useful feedback.
			// So we catch the exception here and display useful info on the UI.
			catch (ArgumentOutOfRangeException)
			{
				// TODO
				//Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>()
				//          .ShowView(false);

				string dialString = m_StringBuilder.Pop();
				if (!string.IsNullOrEmpty(dialString))
					dialString = string.Format(" to {0}", dialString);
				string message = string.Format("Call out{0} failed! Please enter a valid dial string.", dialString);

				Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
						  .Show(message, 8 * 1000, GenericAlertPresenterButton.Dismiss);
			}
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