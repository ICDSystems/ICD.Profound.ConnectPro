using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Components.Call;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Controls.Conferencing;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.EventArguments;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Responses;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Controls.Power;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	[PresenterBinding(typeof(IWtcBasePresenter))]
	public sealed class WtcBasePresenter : AbstractPopupPresenter<IWtcBaseView>, IWtcBasePresenter
	{
		private readonly IWtcLeftMenuPresenter m_LeftMenuPresenter;
		private readonly ICameraButtonsPresenter m_CameraButtonsPresenter;
		private readonly SafeTimer m_ConnectingTimer;

		private IPowerDeviceControl m_SubscribedPowerControl;
		private IConferenceDeviceControl m_SubscribedConferenceControl;

		private bool m_IsInCall;
		private bool m_AboutToShowCameraButtons;

		public IConferenceDeviceControl ActiveConferenceControl
		{
			get { return m_SubscribedConferenceControl; }
			set
			{
				if (value == m_SubscribedConferenceControl)
					return;

				Unsubscribe(m_SubscribedConferenceControl);
				m_SubscribedConferenceControl = value;
				Subscribe(m_SubscribedConferenceControl);

				SetWtcPresenterConferenceControls(value);

				if (m_SubscribedConferenceControl == null)
					ShowView(false);
			}
		}

		private bool IsInCall
		{
			get { return m_IsInCall; }
			set
			{
				if (value == m_IsInCall)
					return;

				m_IsInCall = value;

				if (m_IsInCall)
					ShowView(true);
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WtcBasePresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_LeftMenuPresenter = nav.LazyLoadPresenter<IWtcLeftMenuPresenter>();

			m_CameraButtonsPresenter = nav.LazyLoadPresenter<ICameraButtonsPresenter>();
			Subscribe(m_CameraButtonsPresenter);

			m_ConnectingTimer =
				SafeTimer.Stopped(() => Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>().ShowView(false));
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_CameraButtonsPresenter);
		}

		#region Methods

		public void SetControl(IDeviceControl control)
		{
			ActiveConferenceControl = control as IConferenceDeviceControl;
		}

		public bool SupportsControl(IDeviceControl control)
		{
			return control is IConferenceDeviceControl;
		}

		#endregion

		#region Private Methods

		private static IPowerDeviceControl GetWtcPowerControl(IConferenceDeviceControl conferenceControl)
		{
			IDevice conferenceDevice = conferenceControl.Parent;
			return conferenceDevice == null ? null : conferenceDevice.Controls.GetControl<IPowerDeviceControl>();
		}

		/// <summary>
		/// Ensures the conference unit is awake while the view is visible.
		/// </summary>
		private void UpdateCodecAwakeState(bool forcePowerOff)
		{
			if (Room == null)
				return;

			if (m_SubscribedPowerControl == null)
				return;

			if (IsViewVisible)
				Room.Routing.PowerDevice(m_SubscribedPowerControl.Parent, true);
			else if (forcePowerOff)
				Room.Routing.PowerDevice(m_SubscribedPowerControl.Parent, false);
		}

		private void UpdateIsInCall()
		{
			IsInCall =
				m_SubscribedConferenceControl != null &&
				m_SubscribedConferenceControl.GetActiveConference() != null;
		}

		private void SetWtcPresenterConferenceControls(IConferenceDeviceControl value)
		{
			foreach (IWtcPresenter presenter in Navigation.LazyLoadPresenters<IWtcPresenter>())
				presenter.ActiveConferenceControl = value;

			m_CameraButtonsPresenter.SetActiveConferenceControl(value);
		}

		private void SubmitZoomPassword(string meetingNumber, string password)
		{
			if (Room == null)
				return;

			ZoomRoomConferenceControl zoomControl = ActiveConferenceControl as ZoomRoomConferenceControl;
			if (zoomControl == null)
				return;

			DialContext context = new DialContext
			{
				Protocol = eDialProtocol.Zoom,
				CallType = eCallType.Video,
				DialString = meetingNumber,
				Password = password
			};

			Room.Dialing.Dial(zoomControl, context);
		}

		#endregion

		#region Conference Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		private void Subscribe(IConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);

			ZoomRoomConferenceControl zoomControl = control as ZoomRoomConferenceControl;
			CallComponent zoomCallComponent =
				zoomControl == null ? null : zoomControl.Parent.Components.GetComponent<CallComponent>();
			if (zoomCallComponent != null)
			{
				zoomCallComponent.OnCallError += ZoomControlOnCallError;
				zoomCallComponent.OnPasswordRequired += ZoomControlOnPasswordRequired;
				zoomCallComponent.OnFarEndRequestedMicrophoneMute += ZoomControlOnMicrophoneMuteRequested;
				zoomCallComponent.OnFarEndRequestedVideoUnMute += ZoomControlOnVideoUnMuteRequested;
			}

			UpdateIsInCall();

			m_SubscribedPowerControl = GetWtcPowerControl(m_SubscribedConferenceControl);
			if (m_SubscribedPowerControl == null)
				return;

			m_SubscribedPowerControl.OnPowerStateChanged += SubscribedPowerControlOnPowerStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(IConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);

			ZoomRoomConferenceControl zoomControl = control as ZoomRoomConferenceControl;
			CallComponent zoomCallComponent =
				zoomControl == null ? null : zoomControl.Parent.Components.GetComponent<CallComponent>();
			if (zoomCallComponent != null)
			{
				zoomCallComponent.OnCallError -= ZoomControlOnCallError;
				zoomCallComponent.OnPasswordRequired -= ZoomControlOnPasswordRequired;
				zoomCallComponent.OnFarEndRequestedMicrophoneMute -= ZoomControlOnMicrophoneMuteRequested;
				zoomCallComponent.OnFarEndRequestedVideoUnMute -= ZoomControlOnVideoUnMuteRequested;
			}

			UpdateIsInCall();

			if (m_SubscribedPowerControl == null)
				return;

			m_SubscribedPowerControl.OnPowerStateChanged -= SubscribedPowerControlOnPowerStateChanged;
			m_SubscribedPowerControl = null;
		}

		/// <summary>
		/// Called when a conference is added to the control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			UpdateIsInCall();
		}

		/// <summary>
		/// Called when a conference is removed from the control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			UpdateIsInCall();
		}

		/// <summary>
		/// Called when Zoom reports a call error.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ZoomControlOnCallError(object sender, GenericEventArgs<CallConnectError> args)
		{
			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
			          .Show(args.Data.ErrorMessage, GenericAlertPresenterButton.Dismiss);
		}

		/// <summary>
		/// Called when Zoom requests a password for a meeting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ZoomControlOnPasswordRequired(object sender, MeetingNeedsPasswordEventArgs args)
		{
			if (!args.NeedsPassword)
				return;

			string prompt = args.WrongAndRetry
				? string.Format("Please re-enter password for Zoom Meeting #{0} - Incorrect password", args.MeetingNumberFormatted)
				: string.Format("Please enter password for Zoom Meeting #{0}", args.MeetingNumberFormatted);

			Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>()
			          .ShowView(prompt, null, p => SubmitZoomPassword(args.MeetingNumber, p), null, null);
		}

		/// <summary>
		/// Called when the far end requests a microphone mute state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ZoomControlOnMicrophoneMuteRequested(object sender, BoolEventArgs eventArgs)
		{
			if (Room == null)
				return;

			string message = string.Format("The far end is requesting that you {0} your microphone",
			                               eventArgs.Data ? "mute" : "unmute");

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
			          .Show(message,
			                new GenericAlertPresenterButton
			                {
				                Label = eventArgs.Data ? "Mute" : "Unmute",
				                Enabled = true,
				                Visible = true,
				                PressCallback = p => Room.ConferenceManager.PrivacyMuted = eventArgs.Data,
			                },
			                GenericAlertPresenterButton.Dismiss);
		}

		private void ZoomControlOnVideoUnMuteRequested(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			var zoomControl = ActiveConferenceControl as ZoomRoomConferenceControl;
			if (zoomControl == null)
				return;

			const string message = "The far end is requesting that you Unmute your video";

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
			          .Show(message, new GenericAlertPresenterButton
			                {
				                Label = "Unmute",
				                Enabled = true,
				                Visible = true,
				                PressCallback = p => zoomControl.SetCameraMute(false)
			                },
			                GenericAlertPresenterButton.Dismiss);

		}

		/// <summary>
		/// Called when the codec awake state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SubscribedPowerControlOnPowerStateChanged(object sender, PowerDeviceControlPowerStateApiEventArgs eventArgs)
		{
			UpdateCodecAwakeState(false);
		}

		#endregion

		#region Conference Callbacks

		/// <summary>
		/// Subscribe to the conference events.
		/// </summary>
		/// <param name="conference"></param>
		private void Subscribe(IConference conference)
		{
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference events.
		/// </summary>
		/// <param name="conference"></param>
		private void Unsubscribe(IConference conference)
		{
			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		/// <summary>
		/// Called when a conference status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			IGenericLoadingSpinnerPresenter spinner = Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>();

			switch (args.Data)
			{
				case eConferenceStatus.Connecting:
					spinner.ShowView("Connecting...", 30 * 1000);
					break;
				case eConferenceStatus.Connected:
					m_ConnectingTimer.Reset(1000); // hide connecting page 1 second after connection complete
					Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>().ShowView(false);
					break;
				default:
					spinner.ShowView(false);
					break;
			}

			UpdateIsInCall();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			// If the keyboard subpage is open close that instead
			var keyboardPresenter = Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>();
			if (keyboardPresenter.IsViewVisible)
			{
				keyboardPresenter.ShowView(false);
				return;
			}

			// If we are in a call we want to confirm before closing
			IConferenceManager manager = Room == null ? null : Room.ConferenceManager;
			bool isInCall = manager != null && manager.Dialers.IsInCall >= eInCall.Audio;

			if (isInCall)
				Navigation.NavigateTo<IConfirmLeaveCallPresenter>();
			else
				base.ViewOnCloseButtonPressed(sender, eventArgs);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);
			
			// View became visible
			if (args.Data)
				m_LeftMenuPresenter.ShowView(true);

			UpdateCodecAwakeState(true);
		}

		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			// View became hidden
			if (!args.Data)
			{
				m_LeftMenuPresenter.ShowView(false);

				// End the active conference
				if (ActiveConferenceControl != null)
				{
					IConference conference = ActiveConferenceControl.GetActiveConference();
					if (conference != null)
					{
						if (conference.SupportedConferenceFeatures.HasFlag(eConferenceFeatures.LeaveConference))
							conference.LeaveConference();
						else
							conference.Hangup();
					}
				}

				ActiveConferenceControl = null;

				// Hide all of the WTC presenters
				foreach (IWtcPresenter presenter in Navigation.LazyLoadPresenters<IWtcPresenter>())
					presenter.ShowView(false);

				Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>().ShowView(false);

				if (Room != null)
				{
					Room.FocusSource = null;
					Room.Routing.RouteOsd();
				}
			}

			UpdateCodecAwakeState(true);
		}

		#endregion

		#region Subpage Callbacks

		private void Subscribe(ICameraButtonsPresenter cameraButtons)
		{
			cameraButtons.OnViewPreVisibilityChanged += CameraButtonsOnViewPreVisibilityChanged;
			cameraButtons.OnViewVisibilityChanged += CameraPresenterOnViewVisibilityChanged;
		}

		private void Unsubscribe(ICameraButtonsPresenter cameraButtons)
		{
			cameraButtons.OnViewPreVisibilityChanged -= CameraButtonsOnViewPreVisibilityChanged;
			cameraButtons.OnViewVisibilityChanged -= CameraPresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Checking if the view is about to change to the camera buttons view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CameraButtonsOnViewPreVisibilityChanged(object sender, BoolEventArgs e)
		{
			m_AboutToShowCameraButtons = e.Data;
		}

		/// <summary>
		/// If the view is about to change to the camera buttons view, don't show the left menu view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CameraPresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			if (!m_AboutToShowCameraButtons && !m_CameraButtonsPresenter.IsViewVisible && IsViewVisible)
				m_LeftMenuPresenter.ShowView(true);
		}

		#endregion
	}
}
