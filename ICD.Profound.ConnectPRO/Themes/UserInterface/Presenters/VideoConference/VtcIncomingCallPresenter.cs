using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;
using System;
using ICD.Common.Utils.Extensions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcIncomingCallPresenter : AbstractPresenter<IVtcIncomingCallView>, IVtcIncomingCallPresenter
	{
		/// <summary>
		/// Raised when the user answers the incoming call.
		/// </summary>
		public event EventHandler OnCallAnswered;

		private IConferenceManager m_SubscribedConferenceManager;

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

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCallAnswered = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcIncomingCallView view)
		{
			base.Refresh(view);

			IConferenceSource source = GetSource();
			string info = source == null ? string.Empty : string.Format("{0} - {1}", source.Name, source.Number);

			view.SetCallerInfo(info);
		}

		/// <summary>
		/// Gets the first available incoming conference source.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IConferenceSource GetSource()
		{
			IConference conference = m_SubscribedConferenceManager == null
				                         ? null
				                         : m_SubscribedConferenceManager.ActiveConference;

			IConferenceSource source = conference == null
				                           ? null
				                           : conference.GetSources()
				                                       .FirstOrDefault(s => s.Direction == eConferenceSourceDirection.Incoming &&
				                                                            !s.GetIsAnswered() &&
				                                                            !s.GetIsOnline());
			return source;
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
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnRecentSourceAdded += SubscribedConferenceManagerOnRecentSourceAdded;
			m_SubscribedConferenceManager.OnActiveSourceStatusChanged += SubscribedConferenceManagerOnActiveSourceStatusChanged;
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

			m_SubscribedConferenceManager.OnRecentSourceAdded -= SubscribedConferenceManagerOnRecentSourceAdded;
			m_SubscribedConferenceManager.OnActiveSourceStatusChanged -= SubscribedConferenceManagerOnActiveSourceStatusChanged;
		}

		/// <summary>
		/// Called when a conference source is added to the conference manager.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SubscribedConferenceManagerOnRecentSourceAdded(object sender, ConferenceSourceEventArgs eventArgs)
		{
			UpdateVisibility();
		}

		/// <summary>
		/// Called when an active conference source status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SubscribedConferenceManagerOnActiveSourceStatusChanged(object sender, ConferenceSourceStatusEventArgs eventArgs)
		{
			UpdateVisibility();
		}

		/// <summary>
		/// Updates the visibility of the subpage based if there is an incoming source.
		/// </summary>
		private void UpdateVisibility()
		{
			IConferenceSource source = GetSource();

			ShowView(source != null);

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcIncomingCallView view)
		{
			base.Subscribe(view);

			view.OnAnswerButtonPressed += ViewOnAnswerButtonPressed;
			view.OnIgnoreButtonPressed += ViewOnIgnoreButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcIncomingCallView view)
		{
			base.Unsubscribe(view);

			view.OnAnswerButtonPressed -= ViewOnAnswerButtonPressed;
			view.OnIgnoreButtonPressed -= ViewOnIgnoreButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the ignore button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnIgnoreButtonPressed(object sender, EventArgs e)
		{
			IConferenceSource source = GetSource();

			if (source != null)
				source.Hangup();

			ShowView(false);
		}

		/// <summary>
		/// Called when the user presses the answer button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnAnswerButtonPressed(object sender, EventArgs e)
		{
			IConferenceSource source = GetSource();

			if (source != null)
				source.Answer();

			OnCallAnswered.Raise(this);

			ShowView(false);
		}

		#endregion
	}
}
