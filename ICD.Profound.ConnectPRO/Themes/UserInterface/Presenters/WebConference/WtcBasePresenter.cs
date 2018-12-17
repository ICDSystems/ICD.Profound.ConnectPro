using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Controls;
using ICD.Connect.Conferencing.Zoom.Responses;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public sealed class WtcBasePresenter : AbstractPopupPresenter<IWtcBaseView>, IWtcBasePresenter
	{
		private readonly IWtcStartMeetingPresenter m_MainPagePresenter;
		private readonly IWtcButtonListPresenter m_ButtonListPresenter;
		private readonly ICameraControlPresenter m_CameraControlPresenter;
		private readonly List<IWtcPresenter> m_WtcPresenters;

		private IPowerDeviceControl m_SubscribedPowerControl;
		private IWebConferenceDeviceControl m_SubscribedConferenceControl;

		public IWebConferenceDeviceControl ActiveConferenceControl
		{
			get { return m_SubscribedConferenceControl; }
			private set
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

				m_MainPagePresenter.ShowView(!value && IsViewVisible);
				m_ButtonListPresenter.ShowView(value && IsViewVisible);
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
			m_MainPagePresenter = nav.LazyLoadPresenter<IWtcStartMeetingPresenter>();
			m_ButtonListPresenter = nav.LazyLoadPresenter<IWtcButtonListPresenter>();

			m_CameraControlPresenter = nav.LazyLoadPresenter<ICameraControlPresenter>();
			m_CameraControlPresenter.OnViewVisibilityChanged += CameraControlPresenterOnViewVisibilityChanged;

			m_WtcPresenters = nav.LazyLoadPresenters<IWtcPresenter>().ToList();
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
				var conference = ActiveConferenceControl.GetActiveConference() as IWebConference;
				if (conference != null)
					conference.LeaveConference();
			}

			ActiveConferenceControl = null;

			if (Room != null)
				Room.Routing.RouteOsd();
		}

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

		private void UpdateVisibility()
		{
			IsInCall =
				m_SubscribedConferenceControl != null &&
				m_SubscribedConferenceControl.GetActiveConference() != null;
		}

		private void SetWtcPresenterConferenceControls(IWebConferenceDeviceControl value)
		{
			foreach (var presenter in m_WtcPresenters)
				presenter.ActiveConferenceControl = value;
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
					? Enumerable.Empty<IWebConferenceDeviceControl>()
					: d.Controls.GetControls<IWebConferenceDeviceControl>())
				.FirstOrDefault(c => c != null);
		}

		#endregion

		#region Conference Control Callbacks

		private void Subscribe(IWebConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);

			var zoomControl = control as ZoomRoomConferenceControl;
			if(zoomControl != null)
				zoomControl.OnCallError += ZoomControlOnOnCallError;

			UpdateVisibility();

			m_SubscribedPowerControl = GetWtcPowerControl(m_SubscribedConferenceControl);
			if (m_SubscribedPowerControl == null)
				return;

			m_SubscribedPowerControl.OnIsPoweredChanged += SubscribedPowerControlOnIsPoweredChanged;
		}

		private void Unsubscribe(IWebConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);

			var zoomControl = control as ZoomRoomConferenceControl;
			if(zoomControl != null)
				zoomControl.OnCallError -= ZoomControlOnOnCallError;
			
			UpdateVisibility();

			if (m_SubscribedPowerControl == null)
				return;

			m_SubscribedPowerControl.OnIsPoweredChanged -= SubscribedPowerControlOnIsPoweredChanged;
			m_SubscribedPowerControl = null;
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			UpdateVisibility();
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			UpdateVisibility();
		}

		private void ZoomControlOnOnCallError(object sender, GenericEventArgs<CallConnectError> e)
		{
			Navigation.LazyLoadPresenter<IGenericAlertPresenter>().Show(e.Data.ErrorMessage);
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

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs conferenceStatusEventArgs)
		{
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
			var cameraPresenter = Navigation.LazyLoadPresenter<ICameraControlPresenter>();
			if(cameraPresenter != null && cameraPresenter.IsViewVisible && IsViewVisible)
			{
				cameraPresenter.ShowView(false);
				m_ButtonListPresenter.ShowView(true);
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

			m_ButtonListPresenter.ShowView(false);

			// View became visible
			if (args.Data)
			{
				m_MainPagePresenter.ShowView(true);
			}
			// View became hidden
			else
			{
				m_MainPagePresenter.ShowView(false);

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
		
		private void CameraControlPresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			if (!e.Data && IsViewVisible)
				Navigation.NavigateTo<IWtcButtonListPresenter>();
		}

		#endregion
	}
}
