using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Dtmf;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Dtmf
{
	public sealed class VtcDtmfPresenter : AbstractPresenter<IVtcDtmfView>, IVtcDtmfPresenter
	{
		private readonly VtcReferencedDtmfPresenterFactory m_Factory;
		private readonly SafeCriticalSection m_RefreshSection;

		private IConferenceSource m_Selected;
		private IConferenceManager m_SubscribedConferenceManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcDtmfPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Factory = new VtcReferencedDtmfPresenterFactory(nav, ItemFactory);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			UnsubscribeChildren();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcDtmfView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				UnsubscribeChildren();

				IEnumerable<IConferenceSource> sources = GetSources();
				foreach (IVtcReferencedDtmfPresenter presenter in m_Factory.BuildChildren(sources))
				{
					Subscribe(presenter);

					presenter.Selected = presenter.Source == m_Selected;
					presenter.ShowView(true);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Gets the online sources.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConferenceSource> GetSources()
		{
			IConference conference = m_SubscribedConferenceManager == null
										 ? null
										 : m_SubscribedConferenceManager.ActiveConference;
			return conference == null
					   ? Enumerable.Empty<IConferenceSource>()
					   : conference.GetSources().Where(s => s.Status == eConferenceSourceStatus.Connected);
		}

		private IEnumerable<IVtcReferencedDtmfView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		/// <summary>
		/// Sets the given source as selected.
		/// </summary>
		/// <param name="source"></param>
		public void SetSelected(IConferenceSource source)
		{
			if (source == m_Selected)
				return;

			m_Selected = source;

			RefreshIfVisible();
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			m_SubscribedConferenceManager = room == null ? null : room.ConferenceManager;
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnRecentSourceAdded += ConferenceManagerOnRecentSourceAdded;
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

			m_SubscribedConferenceManager.OnRecentSourceAdded -= ConferenceManagerOnRecentSourceAdded;
		}

		/// <summary>
		/// Called when a source is added to the conference.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ConferenceManagerOnRecentSourceAdded(object sender, ConferenceSourceEventArgs eventArgs)
		{
			RefreshIfVisible();

			// Select the most recent source.
			SetSelected(eventArgs.Data);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcDtmfView view)
		{
			base.Subscribe(view);

			view.OnToneButtonPressed += ViewOnToneButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcDtmfView view)
		{
			base.Unsubscribe(view);

			view.OnToneButtonPressed -= ViewOnToneButtonPressed;
		}

		private void ViewOnToneButtonPressed(object sender, CharEventArgs charEventArgs)
		{
			if (m_Selected != null)
				m_Selected.SendDtmf(charEventArgs.Data);
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Unsubscribes from all of the child presenters.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (IVtcReferencedDtmfPresenter presenter in m_Factory)
				Unsubscribe(presenter);
		}

		/// <summary>
		/// Subscribe to the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Subscribe(IVtcReferencedDtmfPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed += ChildOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Unsubscribe(IVtcReferencedDtmfPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed -= ChildOnPressed;
		}

		/// <summary>
		/// Called when the user presses the contact presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnPressed(object sender, EventArgs eventArgs)
		{
			IVtcReferencedDtmfPresenter presenter = sender as IVtcReferencedDtmfPresenter;
			IConferenceSource source = presenter == null ? null : presenter.Source;
			SetSelected(source);
		}

		#endregion
	}
}
