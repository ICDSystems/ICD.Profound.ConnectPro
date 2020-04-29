using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.Conferencing.Zoom.Responses;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IRecordConferencePresenter))]
	public sealed class RecordConferencePresenter : AbstractConferencePresenter<IRecordConferenceView>,
	                                                           IRecordConferencePresenter
	{
		private const string LABEL_RECORD = "Record";
		private const string LABEL_STOP_RECORDING = "Stop Recording";

		private const long BLINK_INTERVAL = 1000 / 2;

		private const string DEFAULT_ERROR =
			"Recording is unavailable.\n\n" +
			"Your active meeting must be in a Pro Zoom User's personal meeting room, not a Zoom Room's.\n\n" +
			"To record this session please grant recording permission to an attendee in the participant list.";

		private const string STOP_RECORDING_ERROR =
			"Unable to stop recording.\n\n" +
			"The meeting is configured for automatic recording of the session";

		private static readonly Dictionary<string, string> s_ErrorMessages =
			new Dictionary<string, string>
			{
				{
					"No recording privilege",
					"Recording is unavailable.\n\n" +
					"Your active meeting must be in a Pro Zoom User's personal meeting room, not a Basic User's.\n\n"
				},
				{"Need email address", DEFAULT_ERROR},
			};

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_BlinkTimer;

		private bool m_BlinkState;
		private bool m_CanRecord;
		private CallComponent m_SubscribedCallComponent;

		/// <summary>
		/// Returns true if the meeting is being recorded.
		/// </summary>
		private bool IsRecording { get { return m_SubscribedCallComponent != null && m_SubscribedCallComponent.CallRecord; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public RecordConferencePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
		                                            TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_BlinkTimer = SafeTimer.Stopped(UpdateState);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IRecordConferenceView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetRecordButtonEnabled(m_CanRecord);
				view.SetStopButtonEnabled(IsRecording);
				view.SetRecordButtonSelected(GetState());
				view.SetRecordAnimation(IsRecording);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void UpdateState()
		{
			if (IsRecording)
				m_BlinkTimer.Reset(BLINK_INTERVAL, BLINK_INTERVAL);
			else
				m_BlinkTimer.Stop();

			m_BlinkState = !m_BlinkState;

			RefreshIfVisible();
		}

		private bool GetState()
		{
			return m_SubscribedCallComponent != null && IsRecording && m_BlinkState;
		}

		private void UpdateCanRecord()
		{
			m_CanRecord = m_SubscribedCallComponent != null && m_CanRecord ||
			              m_SubscribedCallComponent != null && m_SubscribedCallComponent.AmIHost;

			RefreshIfVisible();
		}

		#region Conference Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IConferenceDeviceControl control)
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
		protected override void Unsubscribe(IConferenceDeviceControl control)
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
			string errorMessage = s_ErrorMessages.GetDefault(eventArgs.Data, DEFAULT_ERROR);

			// Hack - A meeting may be set for automatic recording and zoom will not let us stop recording
			if (IsRecording)
				errorMessage = STOP_RECORDING_ERROR;

			// Hide the error message after 15 seconds.
			const long timeout = 15 * 1000;
			
			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
					  .Show(errorMessage, timeout, GenericAlertPresenterButton.Dismiss);
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

		#region View Callbacks

		protected override void Subscribe(IRecordConferenceView view)
		{
			base.Subscribe(view);

			view.OnRecordButtonPressed += ViewOnRecordButtonPressed;
			view.OnStopButtonPressed += ViewOnStopButtonPressed;
		}

		protected override void Unsubscribe(IRecordConferenceView view)
		{
			base.Unsubscribe(view);

			view.OnRecordButtonPressed -= ViewOnRecordButtonPressed;
			view.OnStopButtonPressed -= ViewOnStopButtonPressed;
		}

		private void ViewOnRecordButtonPressed(object sender, EventArgs e)
		{
			if (m_SubscribedCallComponent != null)
				m_SubscribedCallComponent.EnableCallRecord(!IsRecording);
		}

		private void ViewOnStopButtonPressed(object sender, EventArgs e)
		{
			if (m_SubscribedCallComponent != null && IsRecording)
				m_SubscribedCallComponent.EnableCallRecord(false);
		}

		#endregion
	}
}
