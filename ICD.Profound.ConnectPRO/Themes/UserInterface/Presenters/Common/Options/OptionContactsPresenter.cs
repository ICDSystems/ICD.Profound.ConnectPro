using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Hangup;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Options;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Options
{
	public sealed class OptionContactsPresenter : AbstractOptionPresenter<IOptionContactsView>, IOptionContactsPresenter
	{
		private IConferenceManager m_SubscribedConferenceManager;
		private readonly IVtcHangupPresenter m_Menu;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OptionContactsPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_Menu = Navigation.LazyLoadPresenter<IVtcHangupPresenter>();
			Subscribe(m_Menu);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_Menu);
		}

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		protected override ushort GetMode()
		{
			bool menuOpen = m_Menu.IsViewVisible;
			return (ushort)(menuOpen ? 1 : 0);
		}

		#region View Callbacks

		/// <summary>
		/// Called when the user presses the option button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			m_Menu.ShowView(!m_Menu.IsViewVisible);
		}

		#endregion

		#region Menu Callbacks

		/// <summary>
		/// Subscribe to the menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Subscribe(IVtcHangupPresenter menu)
		{
			menu.OnViewVisibilityChanged += MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Unsubscribe(IVtcHangupPresenter menu)
		{
			menu.OnViewVisibilityChanged -= MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Called when the menu is shown/hidden.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void MenuOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
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

			m_SubscribedConferenceManager = room.ConferenceManager;

			m_SubscribedConferenceManager.OnInCallChanged += SubscribedConferenceManagerOnInCallChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnInCallChanged -= SubscribedConferenceManagerOnInCallChanged;
		}

		private void SubscribedConferenceManagerOnInCallChanged(object sender, InCallEventArgs inCallEventArgs)
		{
			ShowView(inCallEventArgs.Data >= eInCall.Audio);
		}

		#endregion
	}
}
