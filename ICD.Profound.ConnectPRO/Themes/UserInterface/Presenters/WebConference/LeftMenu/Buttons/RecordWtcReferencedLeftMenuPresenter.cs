using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
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
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcReferencedLeftMenuView view)
		{
			Label = m_SubscribedCallComponent == null || !m_SubscribedCallComponent.CallRecord ? LABEL_RECORD : LABEL_STOP_RECORDING;
			Icon = "tcRecord";
			Enabled = m_SubscribedCallComponent != null;
			State = m_SubscribedCallComponent == null ? (bool?)null : m_SubscribedCallComponent.CallRecord;

			base.Refresh(view);
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
		}

		/// <summary>
		/// Called when zoom starts/stops recording the call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ZoomCallComponentOnCallRecordChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when we get a zoom error for call record.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ZoomControlOnCallRecordErrorState(object sender, StringEventArgs eventArgs)
		{
			string message = "Failed to Start Recording";
			if (!string.IsNullOrEmpty(eventArgs.Data))
				message = string.Format("{0}{1}{2}", message, IcdEnvironment.NewLine, eventArgs.Data);

			// Hide the error message after 8 seconds.
			const long timeout = 8 * 1000;

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
			          .Show(message, timeout, new GenericAlertPresenterButton
			          {
				          Visible = false,
				          Enabled = false
			          }, GenericAlertPresenterButton.Dismiss);
		}

		#endregion
	}
}
