using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.Conferencing.Zoom.Responses;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(IRecordWtcReferencedLeftMenuPresenter))]
	public sealed class RecordWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter,
	                                                           IRecordWtcReferencedLeftMenuPresenter
	{
		private const string LABEL_RECORD = "Record";
		private const string LABEL_STOP_RECORDING = "Stop Recording";

		private const long BLINK_INTERVAL = 1000 / 2;

		private readonly SafeTimer m_BlinkTimer;
		private bool m_BlinkState;

		private bool m_CanRecord;

		private CallComponent m_SubscribedCallComponent;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public RecordWtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                            ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_BlinkTimer = SafeTimer.Stopped(UpdateState);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcReferencedLeftMenuView view)
		{
			Label = m_SubscribedCallComponent == null || !m_SubscribedCallComponent.CallRecord ? LABEL_RECORD : LABEL_STOP_RECORDING;
			Icon = "tcRecord";
			Enabled = m_CanRecord;
			State = GetState();

			base.Refresh(view);
		}

		private void UpdateState()
		{
			if (m_SubscribedCallComponent != null && m_SubscribedCallComponent.CallRecord)
				m_BlinkTimer.Reset(BLINK_INTERVAL, BLINK_INTERVAL);
			else
				m_BlinkTimer.Stop();

			m_BlinkState = !m_BlinkState;

			RefreshIfVisible();
		}

		private eLightState? GetState()
		{
			return m_SubscribedCallComponent == null
				? (eLightState?)null
				: m_SubscribedCallComponent.CallRecord
					? m_BlinkState
						? eLightState.Green
						: eLightState.Red
					: eLightState.None;
		}

		private void UpdateCanRecord()
		{
			m_CanRecord = m_SubscribedCallComponent != null && m_CanRecord ||
			              m_SubscribedCallComponent != null && m_SubscribedCallComponent.AmIHost;

			RefreshIfVisible();
		}

		public override void HideSubpages()
		{
		}

		/// <summary>
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected override void HandleButtonPress()
		{
			if (m_SubscribedCallComponent != null)
				m_SubscribedCallComponent.EnableCallRecord(!m_SubscribedCallComponent.CallRecord);
		}

		#region Conference Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			var zoomControl = control as ZoomRoomConferenceControl;
			m_SubscribedCallComponent =
				zoomControl == null ? null : zoomControl.Parent.Components.GetComponent<CallComponent>();
			if (m_SubscribedCallComponent == null)
				return;

			m_SubscribedCallComponent.OnCallRecordChanged += ZoomCallComponentOnCallRecordChanged;
			m_SubscribedCallComponent.OnCallRecordErrorState += ZoomControlOnCallRecordErrorState;
			m_SubscribedCallComponent.OnUpdatedCallRecordInfo += ZoomCallComponentOnUpdatedCallRecordInfo;
			m_SubscribedCallComponent.OnAmIHostChanged += ZoomCallComponentOnAmIHostChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			if (m_SubscribedCallComponent == null)
				return;

			m_SubscribedCallComponent.OnCallRecordChanged -= ZoomCallComponentOnCallRecordChanged;
			m_SubscribedCallComponent.OnCallRecordErrorState -= ZoomControlOnCallRecordErrorState;
			m_SubscribedCallComponent.OnUpdatedCallRecordInfo -= ZoomCallComponentOnUpdatedCallRecordInfo;
			m_SubscribedCallComponent.OnAmIHostChanged -= ZoomCallComponentOnAmIHostChanged;
		}

		/// <summary>
		/// Called when zoom starts/stops recording the call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ZoomCallComponentOnCallRecordChanged(object sender, BoolEventArgs e)
		{
			UpdateState();
		}

		/// <summary>
		/// Called when we get a zoom error for call record.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ZoomControlOnCallRecordErrorState(object sender, StringEventArgs eventArgs)
		{
			string message = "Recording is not available: Booking E-Mail does not match the room E-Mail.\n\n" +
			                 "To record this session please grant recording permission to an attendee in the participant list.";

			// Hide the error message after 15 seconds.
			const long timeout = 15 * 1000;

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
			          .Show(message, timeout, new GenericAlertPresenterButton
			          {
				          Visible = false,
				          Enabled = false
			          }, GenericAlertPresenterButton.Dismiss);
		}

		private void ZoomCallComponentOnUpdatedCallRecordInfo(object sender, GenericEventArgs<UpdateCallRecordInfoEvent> e)
		{
			UpdateCanRecord();
		}

		private void ZoomCallComponentOnAmIHostChanged(object sender, BoolEventArgs e)
		{
			UpdateCanRecord();
		}

		#endregion
	}
}
