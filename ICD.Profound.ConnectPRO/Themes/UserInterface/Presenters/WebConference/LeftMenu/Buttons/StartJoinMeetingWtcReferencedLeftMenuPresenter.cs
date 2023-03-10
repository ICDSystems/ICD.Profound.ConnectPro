using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Controls.Conferencing;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(IStartJoinMeetingWtcReferencedLeftMenuPresenter))]
	public sealed class StartJoinMeetingWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter,
	                                                                     IStartJoinMeetingWtcReferencedLeftMenuPresenter
	{
		private readonly IWtcStartMeetingPresenter m_StartMeetingPresenter;
		private readonly IWtcActiveMeetingPresenter m_ActiveMeetingPresenter;

		private bool IsInWebConference
		{
			get
			{
				return
					ActiveConferenceControl != null &&
					ActiveConferenceControl.GetActiveConferences().Any();
			}
		}

		/// <summary>
		/// Gets the zoom traditional control for call out.
		/// </summary>
		private ZoomRoomConferenceControl ZoomConferenceControl
		{
			get { return GetZoomConferenceControl(ActiveConferenceControl); }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public StartJoinMeetingWtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                                      IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_StartMeetingPresenter = Navigation.LazyLoadPresenter<IWtcStartMeetingPresenter>();
			Subscribe(m_StartMeetingPresenter);

			m_ActiveMeetingPresenter = Navigation.LazyLoadPresenter<IWtcActiveMeetingPresenter>();
			Subscribe(m_ActiveMeetingPresenter);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_StartMeetingPresenter);
			Unsubscribe(m_ActiveMeetingPresenter);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcReferencedLeftMenuView view)
		{
			Icon = "videoConference";
			Enabled = ZoomConferenceControl == null || !ZoomConferenceControl.GetActiveConferences().Any();
			Selected = m_StartMeetingPresenter.IsViewVisible || m_ActiveMeetingPresenter.IsViewVisible;
			Label = IsInWebConference ? "Active Meeting" : "Start/Join Meeting";
			State = IsInWebConference ? eLightState.Green : eLightState.None;

			base.Refresh(view);
		}

		public override void HideSubpages()
		{
			m_StartMeetingPresenter.ShowView(false);
			m_ActiveMeetingPresenter.ShowView(false);
		}

		/// <summary>
		/// Gets the zoom traditional control for call out from the given conference control.
		/// </summary>
		[CanBeNull]
		private static ZoomRoomConferenceControl GetZoomConferenceControl(
			[CanBeNull] IConferenceDeviceControl control)
		{
			if (control == null)
				return null;

			ZoomRoom device = control.Parent as ZoomRoom;
			return device == null ? null : device.Controls.GetControl<ZoomRoomConferenceControl>();
		}

		#region ActiveMeetingPresenter Callbacks

		private void Subscribe(IWtcActiveMeetingPresenter activeMeetingPresenter)
		{
			activeMeetingPresenter.OnViewVisibilityChanged += ActiveMeetingPresenterOnViewVisibilityChanged;
		}

		private void Unsubscribe(IWtcActiveMeetingPresenter activeMeetingPresenter)
		{
			activeMeetingPresenter.OnViewVisibilityChanged -= ActiveMeetingPresenterOnViewVisibilityChanged;
		}

		private void ActiveMeetingPresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region StartMeetingPresenter Callbacks

		private void Subscribe(IWtcStartMeetingPresenter startMeetingPresenter)
		{
			startMeetingPresenter.OnViewVisibilityChanged += StartMeetingPresenterOnViewVisibilityChanged;
		}

		private void Unsubscribe(IWtcStartMeetingPresenter startMeetingPresenter)
		{
			startMeetingPresenter.OnViewVisibilityChanged -= StartMeetingPresenterOnViewVisibilityChanged;
		}

		private void StartMeetingPresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (IConference conference in control.GetConferences())
				Subscribe(conference);

			ZoomRoomConferenceControl callOut = GetZoomConferenceControl(control);
			if (callOut == null)
				return;

			callOut.OnConferenceAdded += TraditionalControlOnConferenceAdded;
			callOut.OnConferenceRemoved += TraditionalControlOnConferenceRemoved;

			foreach (IConference conference in callOut.GetConferences())
				Subscribe(conference);
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(IConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			foreach (IConference conference in control.GetConferences())
				Unsubscribe(conference);

			ZoomRoomConferenceControl callOut = GetZoomConferenceControl(control);
			if (callOut == null)
				return;

			callOut.OnConferenceAdded -= TraditionalControlOnConferenceAdded;
			callOut.OnConferenceRemoved -= TraditionalControlOnConferenceRemoved;

			foreach (IConference conference in callOut.GetConferences())
				Unsubscribe(conference);
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
			RefreshIfVisible();
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			RefreshIfVisible();
		}

		private void TraditionalControlOnConferenceAdded(object sender, ConferenceEventArgs args)
		{
			Subscribe(args.Data);
			RefreshIfVisible();
		}

		private void TraditionalControlOnConferenceRemoved(object sender, ConferenceEventArgs args)
		{
			Unsubscribe(args.Data);
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

		/// <summary>
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected override void HandleButtonPress()
		{
			if (IsInWebConference)
				Navigation.NavigateTo<IWtcActiveMeetingPresenter>();
			else
				Navigation.NavigateTo<IWtcStartMeetingPresenter>();
		}
	}
}
