using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Participants.EventHelpers;
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
		private readonly TraditionalParticipantEventHelper m_ParticipantEventHelper;

		private ITraditionalParticipant m_Selected;

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
			m_ParticipantEventHelper = new TraditionalParticipantEventHelper(ParticipantOnChange);
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
				IEnumerable<ITraditionalParticipant> sources = GetSources();
				foreach (IVtcReferencedDtmfPresenter presenter in m_Factory.BuildChildren(sources))
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
		private IEnumerable<ITraditionalParticipant> GetSources()
		{
			var conference = GetActiveConference();
			return conference == null
					   ? Enumerable.Empty<ITraditionalParticipant>()
					   : conference.GetParticipants().Where(s => s.GetIsOnline());
		}

		private ITraditionalConference GetActiveConference()
		{
			return ActiveConferenceControl == null
				? null
				: ActiveConferenceControl.GetActiveConference() as ITraditionalConference;
		}

		private IEnumerable<IVtcReferencedDtmfView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		/// <summary>
		/// Sets the given source as selected.
		/// </summary>
		/// <param name="source"></param>
		private void SetSelected(ITraditionalParticipant source)
		{
			if (source == m_Selected)
				return;

			m_Selected = source;

			RefreshIfVisible();
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the active conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(ITraditionalConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += VideoDialerOnConferenceAdded;
			control.OnConferenceRemoved += VideoDialerOnConferenceRemoved;
			
			foreach (ITraditionalConference conference in control.GetConferences())
				Subscribe(conference);
		}

		/// <summary>
		/// Unsubscribe from the active conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(ITraditionalConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= VideoDialerOnConferenceAdded;
			control.OnConferenceRemoved -= VideoDialerOnConferenceRemoved;

			foreach (ITraditionalConference conference in control.GetConferences())
				Unsubscribe(conference);
		}

		private void VideoDialerOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data as ITraditionalConference);
			RefreshIfVisible();
		}

		private void VideoDialerOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			Unsubscribe(e.Data as ITraditionalConference);
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(ITraditionalConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnOnParticipantRemoved;

			foreach (var participant in conference.GetParticipants())
				m_ParticipantEventHelper.Subscribe(participant);
		}

		private void Unsubscribe(ITraditionalConference conference)
		{
			conference.OnParticipantAdded -= ConferenceOnOnParticipantAdded;
			conference.OnParticipantRemoved -= ConferenceOnOnParticipantRemoved;

			foreach (var participant in conference.GetParticipants())
				m_ParticipantEventHelper.Unsubscribe(participant);
		}

		private void ConferenceOnOnParticipantAdded(object sender, ParticipantEventArgs e)
		{
			var participant = e.Data as ITraditionalParticipant;
			m_ParticipantEventHelper.Subscribe(participant);

			// Select the most recent source.
			SetSelected(participant);
		}

		private void ConferenceOnOnParticipantRemoved(object sender, ParticipantEventArgs e)
		{
			var participant = e.Data as ITraditionalParticipant;
			m_ParticipantEventHelper.Unsubscribe(participant);
		}

		private void ParticipantOnChange(ITraditionalParticipant p)
		{
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
			ITraditionalParticipant source = presenter == null ? null : presenter.Source;
			SetSelected(source);
		}

		#endregion
	}
}
