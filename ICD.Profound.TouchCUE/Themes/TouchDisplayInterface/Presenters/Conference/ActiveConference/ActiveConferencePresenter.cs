using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Participants;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Components.Call;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.ActiveConference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference.ActiveConference
{
	[PresenterBinding(typeof(IActiveConferencePresenter))]
	public sealed class ActiveConferencePresenter : AbstractConferencePresenter<IActiveConferenceView>, IActiveConferencePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedParticipantPresenterFactory m_PresenterFactory;
		private readonly IParticipantControlsPresenter m_ParticipantControls;

		private IReferencedParticipantPresenter m_SelectedParticipant;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ActiveConferencePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new ReferencedParticipantPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
			m_ParticipantControls = nav.LazyLoadPresenter<IParticipantControlsPresenter>();
		}

		private IReferencedParticipantPresenter SelectedParticipant
		{
			get { return m_SelectedParticipant; }
			set
			{
				if (m_SelectedParticipant == value)
					return;

				if (m_SelectedParticipant != null)
					UnsubscribeSelected(m_SelectedParticipant);

				m_SelectedParticipant = value;

				if (m_SelectedParticipant != null)
					SubscribeSelected(m_SelectedParticipant);

				m_ParticipantControls.Participant =
					m_SelectedParticipant != null && m_SelectedParticipant.Participant is IWebParticipant
						? m_SelectedParticipant.Participant as IWebParticipant
						: null;

				RefreshIfVisible();
			}
		}

		protected override void Refresh(IActiveConferenceView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				if (ActiveConferenceControl == null)
					return;

				var activeConference = ActiveConferenceControl.GetActiveConference();

				var webConferenceControl = ActiveConferenceControl as IWebConferenceDeviceControl;

				IEnumerable<IParticipant> unsortedParticipants =
					activeConference == null
						? Enumerable.Empty<IParticipant>()
						: activeConference.GetParticipants();
				List<IParticipant> sortedParticipants = (activeConference is IWebConference
						? unsortedParticipants.OrderByDescending(p => ((IWebParticipant) p).IsHost)
							.ThenByDescending(p => ((IWebParticipant) p).IsSelf)
							.ThenBy(p => p.Name)
						: unsortedParticipants.OrderBy(p => p.Name))
					.ToList();

				foreach (IReferencedParticipantPresenter presenter in m_PresenterFactory.BuildChildren(sortedParticipants))
				{
					presenter.Selected = presenter == SelectedParticipant;
					presenter.ShowView(true);
				}

				// Show "no participants" and invite button if there are no participants
				view.SetNoParticipantsLabelVisibility(sortedParticipants.Count == 0);
				view.SetInviteButtonVisibility(sortedParticipants.Count == 0);

				// This may change if other web conferences have meeting info to display
				ZoomRoom zoomRoom = webConferenceControl == null ? null : webConferenceControl.Parent as ZoomRoom;
				CallComponent component = zoomRoom == null ? null : zoomRoom.Components.GetComponent<CallComponent>();
				view.SetMeetingNumberLabelVisibility(component != null);
				view.SetMeetingNumberLabelText(component != null
					? string.Format("Meeting #: {0}", component.MeetingId)
					: string.Empty);

				// Only hosts can kick/mute people
				bool isHost = webConferenceControl != null && webConferenceControl.AmIHost;
				bool isNotSelf = SelectedParticipant != null 
				                 && SelectedParticipant.Participant is IWebParticipant
				                 && !((IWebParticipant)SelectedParticipant.Participant).IsSelf;
				bool kickMuteEnabled = isHost && isNotSelf;
				m_ParticipantControls.ShowView(kickMuteEnabled);

				// Call lock
				bool locked = webConferenceControl != null && webConferenceControl.CallLock;
				view.SetLockButtonSelected(locked);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private IEnumerable<IReferencedParticipantView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#endregion

		#region Participant Callbacks

		private void Subscribe(IReferencedParticipantPresenter presenter)
		{
			if (presenter != null)
				presenter.OnPressed += PresenterOnOnPressed;
		}

		private void Unsubscribe(IReferencedParticipantPresenter presenter)
		{
			if (presenter != null)
				presenter.OnPressed -= PresenterOnOnPressed;
		}

		private void PresenterOnOnPressed(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IReferencedParticipantPresenter;
			SelectedParticipant = SelectedParticipant == presenter ? null : presenter;
			RefreshIfVisible();
		}

		#endregion

		#region Selected Participant Callbacks

		private void SubscribeSelected(IReferencedParticipantPresenter presenter)
		{
		}

		private void UnsubscribeSelected(IReferencedParticipantPresenter presenter)
		{
		}

		#endregion

		#region Control Callbacks

		protected override void Subscribe(IConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;
			
			foreach (IConference conference in control.GetConferences())
				Subscribe(conference);

			var webConferenceControl = control as IWebConferenceDeviceControl;
			if (webConferenceControl == null)
				return;

			webConferenceControl.OnCameraMuteChanged += ControlOnCameraMuteChanged;
			webConferenceControl.OnAmIHostChanged += ControlOnOnAmIHostChanged;
			webConferenceControl.OnCallLockChanged += ControlOnCallLockChanged;
		}

		protected override void Unsubscribe(IConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;
			
			foreach (IConference conference in control.GetConferences())
				Unsubscribe(conference);

			var webConferenceControl = control as IWebConferenceDeviceControl;
			if (webConferenceControl == null)
				return;

			webConferenceControl.OnCameraMuteChanged -= ControlOnCameraMuteChanged;
			webConferenceControl.OnAmIHostChanged -= ControlOnOnAmIHostChanged;
			webConferenceControl.OnCallLockChanged -= ControlOnCallLockChanged;
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data as IWebConference);
			RefreshIfVisible();
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data as IWebConference);
			RefreshIfVisible();
		}

		private void ControlOnCameraMuteChanged(object sender, BoolEventArgs args)
		{
			RefreshIfVisible();
		}

		private void ControlOnCallLockChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		private void ControlOnOnAmIHostChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			conference.OnParticipantAdded += ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved += ConferenceOnParticipantRemoved;
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			conference.OnParticipantAdded -= ConferenceOnParticipantAdded;
			conference.OnParticipantRemoved -= ConferenceOnParticipantRemoved;
			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			RefreshIfVisible();
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

		protected override void Subscribe(IActiveConferenceView view)
		{
			base.Subscribe(view);
			
			view.OnInviteButtonPressed += ViewOnInviteButtonPressed;
			view.OnLockButtonPressed += ViewOnLockButtonPressed;
		}

		protected override void Unsubscribe(IActiveConferenceView view)
		{
			base.Unsubscribe(view);
			
			view.OnInviteButtonPressed -= ViewOnInviteButtonPressed;
			view.OnLockButtonPressed -= ViewOnLockButtonPressed;
		}

		private void ViewOnInviteButtonPressed(object sender, EventArgs e)
		{
			Navigation.NavigateTo<IContactListPresenter>();
		}

		private void ViewOnLockButtonPressed(object sender, EventArgs eventArgs)
		{
			var webConferenceControl = ActiveConferenceControl as IWebConferenceDeviceControl;
			if (webConferenceControl == null)
				return;

			webConferenceControl.EnableCallLock(!webConferenceControl.CallLock);
		}

		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnPreVisibilityChanged(sender, args);

			SelectedParticipant = null;
			if (!args.Data)
				m_ParticipantControls.ShowView(false);
		}
		
		#endregion
	}
}