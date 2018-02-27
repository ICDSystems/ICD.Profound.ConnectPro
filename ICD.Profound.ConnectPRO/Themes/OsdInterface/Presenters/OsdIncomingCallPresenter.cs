using System.Linq;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	public sealed class OsdIncomingCallPresenter : AbstractOsdPresenter<IOsdIncomingCallView>, IOsdIncomingCallPresenter
	{
		private IConferenceManager m_SubscribedConferenceManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdIncomingCallPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IOsdIncomingCallView view)
		{
			base.Refresh(view);

			view.SetIcon("source_videoConference_white");
			view.SetSourceName("Video Conference");

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

			string info = source == null ? string.Empty : string.Format("{0} - {1}", source.Name, source.Number);

			view.SetCallerInfo(info);
		}

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