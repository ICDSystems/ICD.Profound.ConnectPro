using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.System;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls;
using ICD.Connect.Conferencing.EventArguments;
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
	public sealed class VtcBasePresenter : AbstractPopupPresenter<IVtcBaseView>, IVtcBasePresenter
	{
		private SystemComponent m_SubscribedCodecSystem;

		private IConferenceManager m_SubscribedConferenceManager;

		private readonly IVtcCallListTogglePresenter m_CallListTogglePresenter;
		private readonly IVtcContactsPresenter m_ContactsPresenter;
		private readonly IVtcButtonListPresenter m_ButtonListPresenter;
		private readonly IVtcCameraPresenter m_CameraPresenter;
		private readonly IVtcKeyboardPresenter m_KeyboardPresenter;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcBasePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_CallListTogglePresenter = nav.LazyLoadPresenter<IVtcCallListTogglePresenter>();
			m_CallListTogglePresenter.OnButtonPressed += CallListTogglePresenterOnButtonPressed;

			m_ContactsPresenter = nav.LazyLoadPresenter<IVtcContactsPresenter>();
			m_ContactsPresenter.OnViewVisibilityChanged += ContactsPresenterOnViewVisibilityChanged;

			m_KeyboardPresenter = nav.LazyLoadPresenter<IVtcKeyboardPresenter>();
			m_ButtonListPresenter = nav.LazyLoadPresenter<IVtcButtonListPresenter>();
			m_CameraPresenter = nav.LazyLoadPresenter<IVtcCameraPresenter>();
		}

		private void ContactsPresenterOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			m_CallListTogglePresenter.SetContactsMode(!boolEventArgs.Data);
		}

		private SystemComponent GetSystemComponent(IConnectProRoom room)
		{
			if (room == null)
				return null;

			IDialingDeviceControl dialer = room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			if (dialer == null)
				return null;

			CiscoCodec codec = dialer.Parent as CiscoCodec;
			return codec == null ? null : codec.Components.GetComponent<SystemComponent>();
		}

		/// <summary>
		/// Ensures the codec is awake while the view is visible.
		/// </summary>
		private void UpdateCodecAwakeState()
		{
			if (m_SubscribedCodecSystem == null)
				return;

			bool visible = IsViewVisible;
			if (visible == m_SubscribedCodecSystem.Awake)
				return;

			if (visible)
				m_SubscribedCodecSystem.Wake();
			else
				m_SubscribedCodecSystem.Standby();
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

			m_SubscribedConferenceManager = room.ConferenceManager;
			m_SubscribedConferenceManager.OnInCallChanged += SubscribedConferenceManagerOnInCallChanged;

			m_SubscribedCodecSystem = GetSystemComponent(room);
			if (m_SubscribedCodecSystem == null)
				return;

			m_SubscribedCodecSystem.OnAwakeStateChanged += SubscribedCodecSystemOnAwakeStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedConferenceManager != null)
				m_SubscribedConferenceManager.OnInCallChanged -= SubscribedConferenceManagerOnInCallChanged;

			if (m_SubscribedCodecSystem == null)
				return;

			m_SubscribedCodecSystem.OnAwakeStateChanged -= SubscribedCodecSystemOnAwakeStateChanged;
		}

		private void SubscribedConferenceManagerOnInCallChanged(object sender, InCallEventArgs inCallEventArgs)
		{
			bool isInCall = inCallEventArgs.Data >= eInCall.Audio;

			m_CallListTogglePresenter.ShowView(isInCall);

			if (isInCall)
			{
				m_ContactsPresenter.ShowView(false);
				m_ButtonListPresenter.ShowView(true);
			}
			else
			{
				m_ButtonListPresenter.ShowView(false);
				if (IsViewVisible)
					m_ContactsPresenter.ShowView(true);
			}
		}

		/// <summary>
		/// Called when the codec awake state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SubscribedCodecSystemOnAwakeStateChanged(object sender, BoolEventArgs eventArgs)
		{
			UpdateCodecAwakeState();
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

			m_CameraPresenter.ShowView(false);
			m_ButtonListPresenter.ShowView(false);
			m_KeyboardPresenter.ShowView(false);

			// View became visible
			if (args.Data)
			{
				m_ContactsPresenter.ShowView(true);
			}
			// View became hidden
			else
			{
				m_ContactsPresenter.ShowView(false);
				m_CallListTogglePresenter.ShowView(false);

				IConferenceManager manager = Room == null ? null : Room.ConferenceManager;
				IConference active = manager == null ? null : manager.ActiveConference;

				if (active != null)
					active.Hangup();

				if (Room != null)
					Room.Routing.UnrouteVtc();
			}

			UpdateCodecAwakeState();
		}

		#endregion

		#region Subpage Callbacks

		private void CallListTogglePresenterOnButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_CameraPresenter.IsViewVisible)
				m_CameraPresenter.ShowView(false);

			if (m_ContactsPresenter.IsViewVisible)
			{
				m_ContactsPresenter.ShowView(false);
				m_ButtonListPresenter.ShowView(true);
			}
			else
			{
				m_ButtonListPresenter.ShowView(false);
				m_ContactsPresenter.ShowView(true);
			}
		}

		#endregion
	}
}
