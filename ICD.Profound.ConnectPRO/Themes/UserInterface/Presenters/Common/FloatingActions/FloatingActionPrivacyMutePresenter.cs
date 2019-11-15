using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;
using ICD.Profound.ConnectPRO.Utils;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions
{
	[PresenterBinding(typeof(IFloatingActionPrivacyMutePresenter))]
	public sealed class FloatingActionPrivacyMutePresenter :
		AbstractFloatingActionPresenter<IFloatingActionPrivacyMuteView>,
		IFloatingActionPrivacyMutePresenter
	{
		private IConferenceManager m_SubscribedConferenceManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public FloatingActionPrivacyMutePresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                          ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			UpdateVisibility();
		}

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		protected override bool GetActive()
		{
			return Room != null &&
			       Room.ConferenceManager != null &&
			       Room.ConferenceManager.PrivacyMuted;
		}

		/// <summary>
		/// Override to get the enabled state for the button.
		/// </summary>
		/// <returns></returns>
		protected override bool GetEnabled()
		{
			return Room != null;
		}

		/// <summary>
		/// Override to handle the button press.
		/// </summary>
		protected override void HandleButtonPress()
		{
			if (Room == null)
				return;

			if (Room.ConferenceManager != null)
				Room.ConferenceManager.TogglePrivacyMute();
		}

		/// <summary>
		/// Updates the visibility of this subpage.
		/// </summary>
		private void UpdateVisibility()
		{
			bool show = Room != null && ConferenceOverrideUtils.ConferenceActionsAvailable(Room, eInCall.Audio);
			ShowView(show);
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

			room.Routing.State.OnDisplaySourceChanged += StateOnDisplaySourceChanged;

			m_SubscribedConferenceManager = room.ConferenceManager;
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnPrivacyMuteStatusChange += ConferenceManagerOnPrivacyMuteStatusChange;
			m_SubscribedConferenceManager.OnInCallChanged += SubscribedConferenceManagerOnInCallChanged;
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

			room.Routing.State.OnDisplaySourceChanged -= StateOnDisplaySourceChanged;

			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnPrivacyMuteStatusChange -= ConferenceManagerOnPrivacyMuteStatusChange;
			m_SubscribedConferenceManager.OnInCallChanged -= SubscribedConferenceManagerOnInCallChanged;

			m_SubscribedConferenceManager = null;
		}

		/// <summary>
		/// Called when the privacy mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ConferenceManagerOnPrivacyMuteStatusChange(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible(false);
		}

		/// <summary>
		/// Called when the room enters/leaves a call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="inCallEventArgs"></param>
		private void SubscribedConferenceManagerOnInCallChanged(object sender, InCallEventArgs inCallEventArgs)
		{
			UpdateVisibility();
		}

		/// <summary>
		/// Called when a source becomes routed/unrouted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StateOnDisplaySourceChanged(object sender, EventArgs eventArgs)
		{
			UpdateVisibility();
		}

		#endregion
	}
}
