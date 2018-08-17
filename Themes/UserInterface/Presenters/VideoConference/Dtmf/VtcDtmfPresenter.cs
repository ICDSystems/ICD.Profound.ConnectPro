using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Controls.Dialing;
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
		private IDialingDeviceControl m_VideoDialingControl;

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
				IEnumerable<IConferenceSource> sources = GetSources();
				foreach (IVtcReferencedDtmfPresenter presenter in m_Factory.BuildChildren(sources, Subscribe, Unsubscribe))
				{
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
			return m_VideoDialingControl == null
					   ? Enumerable.Empty<IConferenceSource>()
					   : m_VideoDialingControl.GetSources().Where(s => s.GetIsOnline());
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

			m_VideoDialingControl = room == null ? null : room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			if (m_VideoDialingControl == null)
				return;

			m_VideoDialingControl.OnSourceAdded += VideoDialerOnSourceAdded;
			m_VideoDialingControl.OnSourceRemoved += VideoDialerOnSourceRemoved;
			m_VideoDialingControl.OnSourceChanged += VideoDialerOnSourceChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_VideoDialingControl != null)
			{
				m_VideoDialingControl.OnSourceAdded -= VideoDialerOnSourceAdded;
				m_VideoDialingControl.OnSourceRemoved -= VideoDialerOnSourceRemoved;
				m_VideoDialingControl.OnSourceChanged -= VideoDialerOnSourceChanged;
			}

			m_VideoDialingControl = null;
		}

		private void VideoDialerOnSourceRemoved(object sender, ConferenceSourceEventArgs e)
		{
			RefreshIfVisible();
		}

		private void VideoDialerOnSourceChanged(object sender, ConferenceSourceEventArgs e)
		{
			RefreshIfVisible();
		}

		private void VideoDialerOnSourceAdded(object sender, ConferenceSourceEventArgs eventArgs)
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
