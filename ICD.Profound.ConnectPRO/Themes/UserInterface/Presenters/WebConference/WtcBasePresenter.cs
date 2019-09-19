using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Controls;
using ICD.Connect.Conferencing.Zoom.EventArguments;
using ICD.Connect.Conferencing.Zoom.Responses;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	[PresenterBinding(typeof(IWtcBasePresenter))]
	public sealed class WtcBasePresenter : AbstractPopupPresenter<IWtcBaseView>, IWtcBasePresenter
	{
		private readonly IWtcLeftMenuPresenter m_LeftMenuPresenter;
		private readonly ICameraControlPresenter m_CameraControlPresenter;
		private readonly ICameraActivePresenter m_CameraActivePresenter;
		private readonly List<IWtcPresenter> m_WtcPresenters;
		private readonly SafeTimer m_ConnectingTimer;

		private IPowerDeviceControl m_SubscribedPowerControl;
		private IWebConferenceDeviceControl m_SubscribedConferenceControl;

		private bool m_IsInCall;
		private bool m_AboutToShowActiveCamera;
		private bool m_AboutToShowControlCamera;

		public IWebConferenceDeviceControl ActiveConferenceControl
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
		public WtcBasePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_LeftMenuPresenter = nav.LazyLoadPresenter<IWtcLeftMenuPresenter>();

			m_CameraControlPresenter = nav.LazyLoadPresenter<ICameraControlPresenter>();
			Subscribe(m_CameraControlPresenter);

			m_CameraActivePresenter = nav.LazyLoadPresenter<ICameraActivePresenter>();
			Subscribe(m_CameraActivePresenter);

			m_WtcPresenters = nav.LazyLoadPresenters<IWtcPresenter>().ToList();

			m_ConnectingTimer =
				SafeTimer.Stopped(() => Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>().ShowView(false));
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_CameraControlPresenter);
			Unsubscribe(m_CameraActivePresenter);
		}

		#region Methods

		/// <summary>
		/// Closes the popup.
		/// </summary>
		public override void Close()
		{
			// Close before routing for better UX
			base.Close();

			if (ActiveConferenceControl != null)
			{
				var conference = ActiveConferenceControl.GetActiveConference() as IWebConference;
				if (conference != null)
					conference.LeaveConference();
			}

			ActiveConferenceControl = null;

			if (Room != null)
				Room.Routing.RouteOsd();
		}
		
		public void SetControl(IDeviceControl control)
		{
			ActiveConferenceControl = control as IWebConferenceDeviceControl;
		}

		public bool SupportsControl(IDeviceControl control)
		{
			return control is IWebConferenceDeviceControl;
		}

		#endregion

		#region Private Methods

		private static IPowerDeviceControl GetWtcPowerControl(IConferenceDeviceControl conferenceControl)
		{
			IDevice conferenceDevice = conferenceControl.Parent as IDevice;
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

		private void UpdateVisibility()
		{
			IsInCall =
				m_SubscribedConferenceControl != null &&
				m_SubscribedConferenceControl.GetActiveConference() != null;
		}

		private void SetWtcPresenterConferenceControls(IWebConferenceDeviceControl value)
		{
			foreach (IWtcPresenter presenter in m_WtcPresenters)
				presenter.ActiveConferenceControl = value;

			m_CameraActivePresenter.SetVtcDestinationControl(value == null
				                                                  ? null
				                                                  : value.Parent.Controls.GetControl<IVideoConferenceRouteControl>());
		}

		private void SubmitZoomPassword(string meetingNumber, string password)
		{
			ZoomRoomConferenceControl zoomControl = ActiveConferenceControl as ZoomRoomConferenceControl;
			if (zoomControl == null)
				return;

			ZoomDialContext context = new ZoomDialContext
			{
				CallType = eCallType.Video,
				DialString = meetingNumber,
				Password = password
			};

			zoomControl.Dial(context);
		}

		#endregion

		#region Conference Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		private void Subscribe(IWebConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);

			var zoomControl = control as ZoomRoomConferenceControl;
			if (zoomControl != null)
			{
				zoomControl.OnCallError += ZoomControlOnCallError;
				zoomControl.OnPasswordRequired += ZoomControlOnPasswordRequired;
				zoomControl.OnMicrophoneMuteRequested += ZoomControlOnMicrophoneMuteRequested;
			}

			UpdateVisibility();

			m_SubscribedPowerControl = GetWtcPowerControl(m_SubscribedConferenceControl);
			if (m_SubscribedPowerControl == null)
				return;

			m_SubscribedPowerControl.OnPowerStateChanged += SubscribedPowerControlOnPowerStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(IWebConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);

			var zoomControl = control as ZoomRoomConferenceControl;
			if (zoomControl != null)
			{
				zoomControl.OnCallError -= ZoomControlOnCallError;
				zoomControl.OnPasswordRequired -= ZoomControlOnPasswordRequired;
				zoomControl.OnMicrophoneMuteRequested -= ZoomControlOnMicrophoneMuteRequested;
			}

			UpdateVisibility();

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
			UpdateVisibility();
		}

		/// <summary>
		/// Called when a conference is removed from the control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			UpdateVisibility();
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
				? string.Format("Please re-enter password for Zoom Meeting #{0} - Incorrect password", args.MeetingNumber)
				: string.Format("Please enter password for Zoom Meeting #{0}", args.MeetingNumber);

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
				                PressCallback = p => Room.ConferenceManager.EnablePrivacyMute(eventArgs.Data),
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

			UpdateVisibility();
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
			// If the camera subpage is open close that instead
			if (m_CameraControlPresenter.IsViewVisible && IsViewVisible)
			{
				m_CameraControlPresenter.ShowView(false);
				m_LeftMenuPresenter.ShowView(true);
				return;
			}

            // If the camera subpage is open close that instead
			if (m_CameraActivePresenter.IsViewVisible && IsViewVisible)
			{
				m_CameraActivePresenter.ShowView(false);
				m_LeftMenuPresenter.ShowView(true);
				return;
			}

			// If the keyboard subpage is open close that instead
			var keyboardPresenter = Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>();
			if (keyboardPresenter.IsViewVisible)
			{
				keyboardPresenter.ShowView(false);
				return;
			}

			// If we are in a call we want to confirm before closing
			IConferenceManager manager = Room == null ? null : Room.ConferenceManager;
			bool isInCall = manager != null && manager.IsInCall >= eInCall.Audio;

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
			{
				m_LeftMenuPresenter.ShowView(true);
			}
			// View became hidden
			else
			{
				m_LeftMenuPresenter.ShowView(false);

				if (ActiveConferenceControl != null)
				{
					var active = ActiveConferenceControl.GetActiveConference() as ITraditionalConference;

					if (active != null)
						active.Hangup();
				}

				Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>().ShowView(false);

				if (Room != null)
					Room.FocusSource = null;
			}

			UpdateCodecAwakeState(true);
		}

		#endregion

		#region Subpage Callbacks

		private void Subscribe(ICameraControlPresenter cameraControl)
		{
			cameraControl.OnViewPreVisibilityChanged += CameraControlPresenterOnViewPreVisibilityChanged;
			cameraControl.OnViewVisibilityChanged += CameraPresenterOnViewVisibilityChanged;
		}

		private void Unsubscribe(ICameraControlPresenter cameraControl)
		{
			cameraControl.OnViewPreVisibilityChanged -= CameraControlPresenterOnViewPreVisibilityChanged;
			cameraControl.OnViewVisibilityChanged -= CameraPresenterOnViewVisibilityChanged;
		}

		private void Subscribe(ICameraActivePresenter cameraActive)
		{
			cameraActive.OnViewPreVisibilityChanged += CameraActivePresenterOnViewPreVisibilityChanged;
			cameraActive.OnViewVisibilityChanged += CameraPresenterOnViewVisibilityChanged;
		}

		private void Unsubscribe(ICameraActivePresenter cameraActive)
		{
			cameraActive.OnViewPreVisibilityChanged -= CameraActivePresenterOnViewPreVisibilityChanged;
			cameraActive.OnViewVisibilityChanged -= CameraPresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Checking if the view is about to change to the camera control view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CameraControlPresenterOnViewPreVisibilityChanged(object sender, BoolEventArgs e)
		{
			m_AboutToShowControlCamera = e.Data;
		}

		/// <summary>
		/// Checking if the view is about to change to the camera active view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CameraActivePresenterOnViewPreVisibilityChanged(object sender, BoolEventArgs e)
		{
			m_AboutToShowActiveCamera = e.Data;
		}

		/// <summary>
		/// If the view is about to change the the camera active or camera control view, don't show the left menu view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CameraPresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			bool aboutToShow = m_AboutToShowActiveCamera || m_AboutToShowControlCamera;

			if (!aboutToShow && !m_CameraActivePresenter.IsViewVisible && !m_CameraControlPresenter.IsViewVisible && IsViewVisible)
				m_LeftMenuPresenter.ShowView(true);
		}

		#endregion
	}
}
