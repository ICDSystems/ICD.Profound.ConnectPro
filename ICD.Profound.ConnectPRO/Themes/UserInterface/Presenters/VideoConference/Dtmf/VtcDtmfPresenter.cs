using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Dtmf;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Dtmf
{
	[PresenterBinding(typeof(IVtcDtmfPresenter))]
	public sealed class VtcDtmfPresenter : AbstractVtcPresenter<IVtcDtmfView>, IVtcDtmfPresenter
	{
		private readonly VtcReferencedDtmfPresenterFactory m_Factory;
		private readonly SafeCriticalSection m_RefreshSection;

		private IConference m_Selected;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcDtmfPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Factory = new VtcReferencedDtmfPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
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
				IEnumerable<IConference> conferences = GetActiveConferencesWithDtmfSupport();
				foreach (IVtcReferencedDtmfPresenter presenter in m_Factory.BuildChildren(conferences))
				{
					presenter.Selected = presenter.Conference == m_Selected;
					presenter.ShowView(true);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private IEnumerable<IConference> GetActiveConferencesWithDtmfSupport()
		{
			return ActiveConferenceControl == null
				? Enumerable.Empty<IConference>()
				: ActiveConferenceControl.GetActiveConferences().Where(c => c.SupportedConferenceFeatures.HasFlag(eConferenceFeatures.SendDtmf));
		}

		private IEnumerable<IVtcReferencedDtmfView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		/// <summary>
		/// Sets the given source as selected.
		/// </summary>
		/// <param name="conference"></param>
		private void SetSelected(IConference conference)
		{
			if (conference == m_Selected)
				return;

			m_Selected = conference;

			RefreshIfVisible();
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the active conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += VideoDialerOnConferenceAdded;
			control.OnConferenceRemoved += VideoDialerOnConferenceRemoved;
		}

		/// <summary>
		/// Unsubscribe from the active conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(IConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= VideoDialerOnConferenceAdded;
			control.OnConferenceRemoved -= VideoDialerOnConferenceRemoved;
		}

		private void VideoDialerOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			// Select confernce automatically when added
			m_Selected = e.Data;
			RefreshIfVisible();
		}

		private void VideoDialerOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			// Select most-recent conference if selected conference ends
			if (m_Selected == e.Data)
				m_Selected = GetActiveConferencesWithDtmfSupport().OrderByDescending(c => c.StartTime).FirstOrDefault();

			RefreshIfVisible();
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
			var presenter = sender as IVtcReferencedDtmfPresenter;
			IConference conference = presenter == null ? null : presenter.Conference;
			SetSelected(conference);
		}

		#endregion
	}
}
