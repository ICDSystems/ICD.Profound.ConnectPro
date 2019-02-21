using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Polycom.Devices.Codec.Controls;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	[PresenterBinding(typeof(IVtcBasePresenter))]
	public sealed class VtcBasePresenter : AbstractPopupPresenter<IVtcBaseView>, IVtcBasePresenter
	{
		public event EventHandler OnActiveConferenceControlChanged;

		private readonly IVtcCallListTogglePresenter m_CallListTogglePresenter;
		private readonly IVtcContactsNormalPresenter m_ContactsNormalPresenter;
		private readonly IVtcContactsPolycomPresenter m_ContactsPolycomPresenter;
		private readonly IVtcButtonListPresenter m_ButtonListPresenter;
		private readonly ICameraControlPresenter m_CameraControlPresenter;
		private readonly IVtcKeyboardPresenter m_KeyboardPresenter;
		private readonly IVtcKeypadPresenter m_KeypadPresenter;
		private readonly List<IVtcPresenter> m_VtcPresenters;

		private IPowerDeviceControl m_SubscribedPowerControl;
		private ITraditionalConferenceDeviceControl m_SubscribedConferenceControl;

		public ITraditionalConferenceDeviceControl ActiveConferenceControl
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

		private bool m_IsInCall;

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
		public VtcBasePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_CallListTogglePresenter = nav.LazyLoadPresenter<IVtcCallListTogglePresenter>();
			m_CallListTogglePresenter.OnButtonPressed += CallListTogglePresenterOnButtonPressed;

			m_ContactsNormalPresenter = nav.LazyLoadPresenter<IVtcContactsNormalPresenter>();
			m_ContactsNormalPresenter.OnViewVisibilityChanged += ContactsNormalPresenterOnViewVisibilityChanged;

			m_ContactsPolycomPresenter = nav.LazyLoadPresenter<IVtcContactsPolycomPresenter>();
			m_ContactsPolycomPresenter.OnViewVisibilityChanged += ContactsPolycomPresenterOnViewVisibilityChanged;

			m_CameraControlPresenter = nav.LazyLoadPresenter<ICameraControlPresenter>();
			m_CameraControlPresenter.OnViewVisibilityChanged += CameraControlPresenterOnOnViewVisibilityChanged;

			m_KeyboardPresenter = nav.LazyLoadPresenter<IVtcKeyboardPresenter>();
			m_KeypadPresenter = nav.LazyLoadPresenter<IVtcKeypadPresenter>();

			m_ButtonListPresenter = nav.LazyLoadPresenter<IVtcButtonListPresenter>();

			m_VtcPresenters = nav.LazyLoadPresenters<IVtcPresenter>().ToList();
		}

		/// <summary>
		/// Closes the popup.
		/// </summary>
		public override void Close()
		{
			// Close before routing for better UX
			base.Close();

			if (ActiveConferenceControl != null)
			{
				var conference = ActiveConferenceControl.GetActiveConference() as ITraditionalConference;
				if (conference != null)
					conference.Hangup();
			}
				
			ActiveConferenceControl = null;

			if (Room != null)
				Room.Routing.RouteOsd();
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
			IDevice conferenceDevice = conferenceControl.Parent as IDevice;
			return conferenceDevice == null ? null : conferenceDevice.Controls.GetControl<IPowerDeviceControl>();
		}

		/// <summary>
		/// Ensures the codec is awake while the view is visible.
		/// </summary>
		private void UpdateCodecAwakeState(bool forcePowerOff)
		{
			if (m_SubscribedPowerControl == null)
				return;

			bool visible = IsViewVisible;
			if (visible == m_SubscribedPowerControl.IsPowered)
				return;

			if (visible)
				m_SubscribedPowerControl.PowerOn();
			else if (forcePowerOff)
				m_SubscribedPowerControl.PowerOff();
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

		private void SetVtcPresentersActiveConferenceControl(ITraditionalConferenceDeviceControl value)
		{
			foreach (var presenter in m_VtcPresenters)
				presenter.ActiveConferenceControl = value;
		}

		private void UpdateVisibility()
		{
			IsInCall =
				m_SubscribedConferenceControl != null &&
				m_SubscribedConferenceControl.GetConferences().Any(c => c.Status == eConferenceStatus.Connected && c.GetParticipants()
					                                                        .Any(s => s.GetIsOnline() || s.GetIsActive()));
		}

		#endregion

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

			room.Routing.OnDisplaySourceChanged += RoutingOnOnDisplaySourceChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			ActiveConferenceControl = null;
		}

		private void RoutingOnOnDisplaySourceChanged(object sender, EventArgs eventArgs)
		{
			ActiveConferenceControl = Room.Routing
				.GetCachedActiveVideoSources()
				.SelectMany(kvp => kvp.Value)
				.Select(s => Room.Core.Originators[s.Device] as IDevice)
				.SelectMany(d => d == null 
					? Enumerable.Empty<ITraditionalConferenceDeviceControl>() 
					: d.Controls.GetControls<ITraditionalConferenceDeviceControl>())
				.FirstOrDefault(c => c != null);
		}

		#endregion

		#region Conference Control Callbacks

		private void Subscribe(ITraditionalConferenceDeviceControl control)
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

			m_SubscribedPowerControl.OnIsPoweredChanged += SubscribedPowerControlOnIsPoweredChanged;
		}

		private void Unsubscribe(ITraditionalConferenceDeviceControl control)
		{
			if (control == null)
				return;

			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);

			if (m_SubscribedPowerControl == null)
				return;

			m_SubscribedPowerControl.OnIsPoweredChanged -= SubscribedPowerControlOnIsPoweredChanged;
			m_SubscribedPowerControl = null;
		}

		private void ControlOnOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data as ITraditionalConference);

			UpdateVisibility();
		}

		private void ControlOnOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			Unsubscribe(e.Data as ITraditionalConference);

			UpdateVisibility();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(ITraditionalConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnParticipantsChanged;
			conference.OnParticipantRemoved += ConferenceOnParticipantsChanged;
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}
		private void Unsubscribe(ITraditionalConference conference)
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
		private void SubscribedPowerControlOnIsPoweredChanged(object sender, PowerDeviceControlPowerStateApiEventArgs eventArgs)
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
			// If the camera subpage is open close that instead
			if (m_CameraControlPresenter != null && m_CameraControlPresenter.IsViewVisible && IsViewVisible)
			{
				m_CameraControlPresenter.ShowView(false);
				m_ButtonListPresenter.ShowView(true);
				return;
			}

			// If the keyboard subpage is open close that instead
			if (m_KeyboardPresenter != null && m_KeyboardPresenter.IsViewVisible)
			{
				m_KeyboardPresenter.ShowView(false);
				ShowContactsPresenter(true);
				return;
			}

			// If the keypad subpage is open close that instead
			if (m_KeypadPresenter != null && m_KeypadPresenter.IsViewVisible)
			{
				m_KeypadPresenter.ShowView(false);
				ShowContactsPresenter(true);
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

			m_CameraControlPresenter.ShowView(false);
			m_ButtonListPresenter.ShowView(false);
			m_KeyboardPresenter.ShowView(false);
			m_KeypadPresenter.ShowView(false);

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
					var active = ActiveConferenceControl.GetActiveConference() as ITraditionalConference;

					if (active != null)
						active.Hangup();
				}
			}

			UpdateCodecAwakeState(true);
		}

		#endregion

		#region Subpage Callbacks

		private void CameraControlPresenterOnOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			if (!e.Data && IsViewVisible)
				Navigation.NavigateTo<IVtcButtonListPresenter>();
		}

		private void CallListTogglePresenterOnButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_CameraControlPresenter.IsViewVisible && IsViewVisible)
				m_CameraControlPresenter.ShowView(false);

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
