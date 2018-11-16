using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.ActiveCalls
{
	public sealed class VtcActiveCallsPresenter : AbstractUiPresenter<IVtcActiveCallsView>, IVtcActiveCallsPresenter
	{
		private readonly VtcReferencedActiveCallsPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		private IDialingDeviceControl m_SubscribedVideoDialer;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcActiveCallsPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_ChildrenFactory = new VtcReferencedActiveCallsPresenterFactory(nav, ItemFactory);
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
		protected override void Refresh(IVtcActiveCallsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IConferenceSource[] sources = GetSources().ToArray();
				foreach (IVtcReferencedActiveCallsPresenter presenter in m_ChildrenFactory.BuildChildren(sources, p => { }, p => { }))
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
			return m_SubscribedVideoDialer == null
				       ? Enumerable.Empty<IConferenceSource>()
					   : m_SubscribedVideoDialer.GetSources().Where(s => s.GetIsActive());
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
		private IEnumerable<IVtcReferencedActiveCallsView> ItemFactory(ushort count)
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

			m_SubscribedVideoDialer = room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			if (m_SubscribedVideoDialer == null)
				return;

			m_SubscribedVideoDialer.OnSourceAdded += VideoDialerOnSourceAdded;
			m_SubscribedVideoDialer.OnSourceRemoved += VideoDialerOnSourceRemoved;
			m_SubscribedVideoDialer.OnSourceChanged += VideoDialerOnSourceChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedVideoDialer != null)
			{
				m_SubscribedVideoDialer.OnSourceAdded -= VideoDialerOnSourceAdded;
				m_SubscribedVideoDialer.OnSourceRemoved -= VideoDialerOnSourceRemoved;
				m_SubscribedVideoDialer.OnSourceChanged -= VideoDialerOnSourceChanged;
			}

			m_SubscribedVideoDialer = null;
		}

		private void VideoDialerOnSourceAdded(object sender, ConferenceSourceEventArgs e)
		{
			RefreshIfVisible();
		}

		private void VideoDialerOnSourceRemoved(object sender, ConferenceSourceEventArgs e)
		{
			RefreshIfVisible();
		}

		private void VideoDialerOnSourceChanged(object sender, ConferenceSourceEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcActiveCallsView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
			view.OnHangupAllButtonPressed += ViewOnHangupAllButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcActiveCallsView view)
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
