using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Hangup
{
	public sealed class VtcHangupPresenter : AbstractPresenter<IVtcHangupView>, IVtcHangupPresenter
	{
		private readonly VtcReferencedHangupPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		private IConferenceManager m_SubscribedConferenceManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcHangupPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_ChildrenFactory = new VtcReferencedHangupPresenterFactory(nav, ItemFactory);
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcHangupView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IConferenceSource[] sources = GetSources().ToArray();
				foreach (IVtcReferencedHangupPresenter presenter in m_ChildrenFactory.BuildChildren(sources))
					presenter.ShowView(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			m_ChildrenFactory.SetRoom(room);
		}

		#region Private Methods

		/// <summary>
		/// Returns the current active sources.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConferenceSource> GetSources()
		{
			IConference conference = m_SubscribedConferenceManager == null
										 ? null
										 : m_SubscribedConferenceManager.ActiveConference;
			return conference == null
					   ? Enumerable.Empty<IConferenceSource>()
					   : conference.GetSources().Where(s => s.GetIsOnline());
		}

		/// <summary>
		/// Generates the given number of views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IVtcReferencedHangupView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
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
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;
			m_SubscribedConferenceManager.OnRecentSourceAdded += ConferenceManagerOnRecentSourceAdded;
			m_SubscribedConferenceManager.OnActiveSourceStatusChanged += ConferenceManagerOnActiveSourceStatusChanged;
			m_SubscribedConferenceManager.OnActiveConferenceChanged += ConferenceManagerOnActiveConferenceChanged;
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

			m_SubscribedConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;
			m_SubscribedConferenceManager.OnRecentSourceAdded -= ConferenceManagerOnRecentSourceAdded;
			m_SubscribedConferenceManager.OnActiveSourceStatusChanged -= ConferenceManagerOnActiveSourceStatusChanged;
			m_SubscribedConferenceManager.OnActiveConferenceChanged -= ConferenceManagerOnActiveConferenceChanged;
		}

		/// <summary>
		/// Called when we enter/leave a call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnInCallChanged(object sender, InCallEventArgs args)
		{
			// Show the view when we enter a call.
			if (args.Data >= eInCall.Audio)
				ShowView(true);
		}

		/// <summary>
		/// Called when an active source status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnActiveSourceStatusChanged(object sender, ConferenceSourceStatusEventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when a new source is added.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnRecentSourceAdded(object sender, ConferenceSourceEventArgs args)
		{
			if (args.Data.GetIsOnline())
				ShowView(true);

			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the active conference changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="conferenceEventArgs"></param>
		private void ConferenceManagerOnActiveConferenceChanged(object sender, ConferenceEventArgs conferenceEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
