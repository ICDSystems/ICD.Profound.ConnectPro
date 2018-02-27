using System.Linq;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcIncomingCallPresenter : AbstractPresenter<IVtcIncomingCallView>, IVtcIncomingCallPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcIncomingCallPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		private IConferenceManager m_SubscribedConferenceManager;

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
			m_SubscribedConferenceManager.OnRecentSourceAdded += SubscribedConferenceManagerOnRecentSourceAdded;
			m_SubscribedConferenceManager.OnActiveSourceStatusChanged += SubscribedConferenceManagerOnActiveSourceStatusChanged;
		}

		private void SubscribedConferenceManagerOnRecentSourceAdded(object sender, ConferenceSourceEventArgs conferenceSourceEventArgs)
		{
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			IConference conference = m_SubscribedConferenceManager == null
							 ? null
							 : m_SubscribedConferenceManager.ActiveConference;
			IConferenceSource source = conference == null
										   ? null
										   : conference.GetSources()
													   .FirstOrDefault(
																	   s =>
																	   s.Direction == eConferenceSourceDirection.Incoming &&
																	   !s.GetIsAnswered());

			ShowView(source != null);

			RefreshIfVisible();
		}

		private void SubscribedConferenceManagerOnActiveSourceStatusChanged(object sender, ConferenceSourceStatusEventArgs conferenceSourceStatusEventArgs)
		{
			UpdateVisibility();
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

			m_SubscribedConferenceManager.OnRecentSourceAdded += SubscribedConferenceManagerOnRecentSourceAdded;
			m_SubscribedConferenceManager.OnActiveSourceStatusChanged -= SubscribedConferenceManagerOnActiveSourceStatusChanged;
		}
	}
}
