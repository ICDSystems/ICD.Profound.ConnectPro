using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Hangup;
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
			base.Dispose();

			m_ChildrenFactory.Dispose();
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

		/// <summary>
		/// Returns the current active sources.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IConferenceSource> GetSources()
		{
			IConference conference = m_SubscribedConferenceManager == null
				                         ? null
				                         : m_SubscribedConferenceManager.ActiveConference;
			return conference == null
				       ? Enumerable.Empty<IConferenceSource>()
				       : conference.GetSources().Where(CanHangup);
		}

		/// <summary>
		/// Hangs up all of the active sources.
		/// </summary>
		public void HangupAll()
		{
			foreach (IConferenceSource source in GetSources())
				source.Hangup();
		}

		#region Private Methods

		/// <summary>
		/// Generates the given number of views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IVtcReferencedHangupView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		private bool CanHangup(IConferenceSource source)
		{
			switch (source.Status)
			{
				case eConferenceSourceStatus.Undefined:
				case eConferenceSourceStatus.Disconnecting:
				case eConferenceSourceStatus.Disconnected:
				case eConferenceSourceStatus.Idle:
					return false;

				case eConferenceSourceStatus.Dialing:
				case eConferenceSourceStatus.Connecting:
				case eConferenceSourceStatus.Ringing:
				case eConferenceSourceStatus.Connected:
				case eConferenceSourceStatus.OnHold:
				case eConferenceSourceStatus.EarlyMedia:
				case eConferenceSourceStatus.Preserved:
				case eConferenceSourceStatus.RemotePreserved:
					return true;

				default:
					throw new ArgumentOutOfRangeException();
			}
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
			if (!GetSources().Any())
				ShowView(false);

			RefreshIfVisible();
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

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcHangupView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
			view.OnHangupAllButtonPressed += ViewOnHangupAllButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcHangupView view)
		{
			base.Unsubscribe(view);

			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
			view.OnHangupAllButtonPressed -= ViewOnHangupAllButtonPressed;
		}

		private void ViewOnHangupAllButtonPressed(object sender, EventArgs eventArgs)
		{
			HangupAll();
		}

		private void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		#endregion
	}
}
