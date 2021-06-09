using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.ActiveCalls
{
	[PresenterBinding(typeof(IVtcActiveCallsPresenter))]
	public sealed class VtcActiveCallsPresenter : AbstractVtcPresenter<IVtcActiveCallsView>, IVtcActiveCallsPresenter
	{
		private readonly VtcReferencedActiveCallsPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcActiveCallsPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_ChildrenFactory = new VtcReferencedActiveCallsPresenterFactory(nav, ItemFactory, null, null);
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
				IParticipant[] sources = GetSources().ToArray();
				foreach (IVtcReferencedActiveCallsPresenter presenter in m_ChildrenFactory.BuildChildren(sources))
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
		public IEnumerable<IParticipant> GetSources()
		{
			var conferences = ActiveConferenceControl == null ? null : ActiveConferenceControl.GetActiveConferences().ToArray();
			return conferences == null || !conferences.Any()
				? Enumerable.Empty<IParticipant>()
				: conferences.SelectMany(c => c.GetParticipants()).Where(p => p.GetIsActive());
		}

		/// <summary>
		/// Hangs up all of the active sources.
		/// </summary>
		public void HangupAll()
		{
			if (ActiveConferenceControl == null)
				return;

			var active = ActiveConferenceControl.GetActiveConferences();

			foreach (IConference conference in active)
				conference.Hangup();
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

		#region Conference Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IConferenceDeviceControl control)
		{
			control.OnConferenceAdded += DialerOnConferenceAdded;
			control.OnConferenceRemoved += DialerOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(IConferenceDeviceControl control)
		{
			control.OnConferenceAdded -= DialerOnConferenceAdded;
			control.OnConferenceRemoved -= DialerOnConferenceRemoved;
			
			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);
		}

		private void DialerOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data);
		}

		private void DialerOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			Unsubscribe(e.Data);
		}

		#endregion

		#region Conference Callbacks 

		private void Subscribe(IConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnParticipantRemoved;
		}

		private void Unsubscribe(IConference conference)
		{
			conference.OnParticipantAdded -= ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved -= ConferenceOnParticipantRemoved;
		}

		private void ConferenceOnParticipantAdded(object sender, ParticipantEventArgs participantEventArgs)
		{
			RefreshIfVisible();
		}

		private void ConferenceOnParticipantRemoved(object sender, ParticipantEventArgs participantEventArgs)
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
