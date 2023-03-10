using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Polycom.Devices.Codec.Controls;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Controls.Power;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	[PresenterBinding(typeof(IVtcBasePresenter))]
	public sealed class VtcBasePresenter : AbstractPopupPresenter<IVtcBaseView>, IVtcBasePresenter
	{
		private readonly IVtcCallListTogglePresenter m_CallListTogglePresenter;
		private readonly IVtcContactsNormalPresenter m_ContactsNormalPresenter;
		private readonly IVtcContactsPolycomPresenter m_ContactsPolycomPresenter;
		private readonly IVtcButtonListPresenter m_ButtonListPresenter;
		private readonly ICameraButtonsPresenter m_CameraButtonsPresenter;
		private readonly IGenericKeyboardPresenter m_KeyboardPresenter;
		private readonly List<IVtcPresenter> m_VtcPresenters;

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

				SetVtcPresentersActiveConferenceControl(value);
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

				m_CallListTogglePresenter.ShowView(m_IsInCall);
				m_ButtonListPresenter.ShowView(m_IsInCall);

				if (m_IsInCall)
					ShowContactsPresenter(false);
				else if (IsViewVisible)
					ShowContactsPresenter(true);
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcBasePresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_CallListTogglePresenter = nav.LazyLoadPresenter<IVtcCallListTogglePresenter>();
			m_CallListTogglePresenter.OnButtonPressed += CallListTogglePresenterOnButtonPressed;

			m_ContactsNormalPresenter = nav.LazyLoadPresenter<IVtcContactsNormalPresenter>();
			m_ContactsNormalPresenter.OnViewVisibilityChanged += ContactsNormalPresenterOnViewVisibilityChanged;

			m_ContactsPolycomPresenter = nav.LazyLoadPresenter<IVtcContactsPolycomPresenter>();
			m_ContactsPolycomPresenter.OnViewVisibilityChanged += ContactsPolycomPresenterOnViewVisibilityChanged;

			m_CameraButtonsPresenter = nav.LazyLoadPresenter<ICameraButtonsPresenter>();
			Subscribe(m_CameraButtonsPresenter);

			m_KeyboardPresenter = nav.LazyLoadPresenter<IGenericKeyboardPresenter>();
			m_ButtonListPresenter = nav.LazyLoadPresenter<IVtcButtonListPresenter>();
			m_VtcPresenters = nav.LazyLoadPresenters<IVtcPresenter>().ToList();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_CameraButtonsPresenter);
		}

		/// <summary>
		/// Sets the device control for the presenter.
		/// </summary>
		/// <param name="control"></param>
		public void SetControl(IDeviceControl control)
		{
			ActiveConferenceControl = control as IConferenceDeviceControl;
		}

		/// <summary>
		/// Returns true if the presenter is able to interact with the given device control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public bool SupportsControl(IDeviceControl control)
		{
			IConferenceDeviceControl dialer = control as IConferenceDeviceControl;
			return dialer != null && dialer.Supports.HasFlag(eCallType.Video);
		}

		#region Private Methods

		private void ContactsPolycomPresenterOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			m_CallListTogglePresenter.SetContactsMode(!boolEventArgs.Data);
		}

		private void ContactsNormalPresenterOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			m_CallListTogglePresenter.SetContactsMode(!boolEventArgs.Data);
		}

		private static IPowerDeviceControl GetVtcPowerControl(IConferenceDeviceControl conferenceControl)
		{
			IDevice conferenceDevice = conferenceControl.Parent;
			return conferenceDevice == null ? null : conferenceDevice.Controls.GetControl<IPowerDeviceControl>();
		}

		/// <summary>
		/// Ensures the codec is awake while the view is visible.
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

		private void ShowContactsPresenter(bool visible)
		{
			if (visible)
			{
				bool polycom = m_SubscribedConferenceControl is PolycomCodecTraditionalConferenceControl;

				m_ContactsNormalPresenter.ShowView(!polycom);
				m_ContactsPolycomPresenter.ShowView(polycom);
			}
			else
			{
				m_ContactsNormalPresenter.ShowView(false);
				m_ContactsPolycomPresenter.ShowView(false);
			}
		}

		private void SetVtcPresentersActiveConferenceControl(IConferenceDeviceControl value)
		{
			foreach (IVtcPresenter presenter in m_VtcPresenters)
				presenter.ActiveConferenceControl = value;

			m_CameraButtonsPresenter.SetActiveConferenceControl(value);
		}

		private void UpdateVisibility()
		{
			IsInCall =
				m_SubscribedConferenceControl != null &&
				m_SubscribedConferenceControl.GetActiveConferences().Any();
		}

		#endregion

		#region Conference Control Callbacks

		private void Subscribe(IConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);

			m_SubscribedPowerControl = GetVtcPowerControl(m_SubscribedConferenceControl);
			if (m_SubscribedPowerControl == null)
				return;

			m_SubscribedPowerControl.OnPowerStateChanged += SubscribedPowerControlOnPowerStateChanged;
		}

		private void Unsubscribe(IConferenceDeviceControl control)
		{
			if (control == null)
				return;

			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);

			if (m_SubscribedPowerControl == null)
				return;

			m_SubscribedPowerControl.OnPowerStateChanged -= SubscribedPowerControlOnPowerStateChanged;
			m_SubscribedPowerControl = null;
		}

		private void ControlOnOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data);

			UpdateVisibility();
		}

		private void ControlOnOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			Unsubscribe(e.Data);

			UpdateVisibility();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnParticipantsChanged;
			conference.OnParticipantRemoved += ConferenceOnParticipantsChanged;
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}
		private void Unsubscribe(IConference conference)
		{
			conference.OnParticipantAdded -= ConferenceOnParticipantsChanged;
			conference.OnParticipantRemoved -= ConferenceOnParticipantsChanged;
			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnParticipantsChanged(object sender, ParticipantEventArgs e)
		{
			UpdateVisibility();
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs e)
		{
			UpdateVisibility();
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

		#region View Callbacks

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			// If the keyboard subpage is open close that instead
			if (m_KeyboardPresenter != null && m_KeyboardPresenter.IsViewVisible)
			{
				m_KeyboardPresenter.ShowView(false);
				ShowContactsPresenter(true);
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

			m_CameraButtonsPresenter.ShowView(false);
			m_ButtonListPresenter.ShowView(false);
			m_KeyboardPresenter.ShowView(false);

			// View became visible
			if (args.Data)
			{
				ShowContactsPresenter(true);
			}
			// View became hidden
			else
			{
				ShowContactsPresenter(false);
				m_CallListTogglePresenter.ShowView(false);

				if (ActiveConferenceControl != null)
				{
					var active = ActiveConferenceControl.GetActiveConferences();
					foreach (IConference conference in active)
						if (conference.SupportsLeaveOrEnd())
							conference.LeaveOrEndConference();
				}

				ActiveConferenceControl = null;

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
				m_ButtonListPresenter.ShowView(true);
		}

		private void CallListTogglePresenterOnButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_CameraButtonsPresenter.IsViewVisible && IsViewVisible)
				m_CameraButtonsPresenter.ShowView(false);

			if (m_ContactsNormalPresenter.IsViewVisible || m_ContactsPolycomPresenter.IsViewVisible)
			{
				ShowContactsPresenter(false);
				m_ButtonListPresenter.ShowView(true);
			}
			else
			{
				m_ButtonListPresenter.ShowView(false);
				ShowContactsPresenter(true);
			}
		}

		#endregion
	}
}
