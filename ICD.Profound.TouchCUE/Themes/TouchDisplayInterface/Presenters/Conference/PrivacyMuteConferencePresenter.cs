using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IPrivacyMuteConferencePresenter))]
	public sealed class PrivacyMuteConferencePresenter : AbstractTouchDisplayPresenter<IPrivacyMuteConferenceView>, IPrivacyMuteConferencePresenter
	{
		private readonly HeaderButtonModel m_HeaderButton;

		private IConferenceManager m_SubscribedConferenceManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public PrivacyMuteConferencePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
		                                          TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_HeaderButton = new HeaderButtonModel(0, 3, PrivacyMuteCallback)
			{
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.PrivacyMuteOff, eTouchCueColor.White),
				LabelText = "Privacy Mute",
				Mode = eHeaderButtonMode.Green
			};
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

		#region Private Methods

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		private bool GetActive()
		{
			return Room != null &&
			       Room.ConferenceManager != null &&
			       Room.ConferenceManager.PrivacyMuted;
		}

		/// <summary>
		/// Override to handle the button press.
		/// </summary>
		private void PrivacyMuteCallback()
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
			var muteActive = GetActive();
			ShowView(muteActive);
			m_HeaderButton.Mode = muteActive ? eHeaderButtonMode.Red : eHeaderButtonMode.Green;
			m_HeaderButton.Icon =
				TouchCueIcons.GetIcon(muteActive ? eTouchCueIcon.PrivacyMuteOn : eTouchCueIcon.PrivacyMuteOff, eTouchCueColor.White);

			var header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			bool show = Room != null && Room.Dialing.ConferenceActionsAvailable(eInCall.Audio);
			if (show)
				header.AddLeftButton(m_HeaderButton);
			else
				header.RemoveLeftButton(m_HeaderButton);
			header.Refresh();
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

			room.Routing.State.OnDisplaySourceChanged += StateOnDisplaySourceChanged;

			m_SubscribedConferenceManager = room.ConferenceManager;
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnPrivacyMuteStatusChange += ConferenceManagerOnPrivacyMuteStatusChange;
			m_SubscribedConferenceManager.Dialers.OnInCallChanged += SubscribedConferenceManagerOnInCallChanged;
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
			m_SubscribedConferenceManager.Dialers.OnInCallChanged -= SubscribedConferenceManagerOnInCallChanged;

			m_SubscribedConferenceManager = null;
		}

		/// <summary>
		/// Called when the privacy mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ConferenceManagerOnPrivacyMuteStatusChange(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateVisibility();
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
