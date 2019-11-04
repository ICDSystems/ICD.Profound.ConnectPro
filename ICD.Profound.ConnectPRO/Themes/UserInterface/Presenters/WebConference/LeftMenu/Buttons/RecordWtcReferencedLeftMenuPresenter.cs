using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(IRecordWtcReferencedLeftMenuPresenter))]
	public sealed class RecordWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter,
	                                                           IRecordWtcReferencedLeftMenuPresenter
	{
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
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected override void HandleButtonPress()
		{
			var zoomControl = ActiveConferenceControl as ZoomRoomConferenceControl;
			if (zoomControl == null)
				return;

			var zoomCallComponent = zoomControl.Parent.Components.GetComponent<CallComponent>();
			if (zoomCallComponent == null)
				return;

			zoomCallComponent.EnableCallRecord(!zoomCallComponent.CallRecord);
		}

		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			var zoomControl = control as ZoomRoomConferenceControl;

			// Hack to set the enabled state of the button
			Enabled = zoomControl != null;

			CallComponent zoomCallComponent =
				zoomControl == null ? null : zoomControl.Parent.Components.GetComponent<CallComponent>();

			if (zoomCallComponent != null)
			{
				zoomCallComponent.OnCallRecordErrorState += ZoomControlOnCallRecordErrorState;
			}
		}

		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			var zoomControl = control as ZoomRoomConferenceControl;

			CallComponent zoomCallComponent =
				zoomControl == null ? null : zoomControl.Parent.Components.GetComponent<CallComponent>();

			if (zoomCallComponent != null)
			{
				zoomCallComponent.OnCallRecordErrorState -= ZoomControlOnCallRecordErrorState;
			}
		}

		private void ZoomControlOnCallRecordErrorState(object sender, StringEventArgs eventArgs)
		{
			if (Room == null)
				return;

			var zoomControl = ActiveConferenceControl as ZoomRoomConferenceControl;
			if (zoomControl == null)
				return;

			var message = eventArgs.Data;
			if (message == null)
				return;

			//Hide the error message after 8 seconds.
			const long timeout = 8000;

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
			          .Show(message, timeout, new GenericAlertPresenterButton
			          {
				          Visible = false,
				          Enabled = false
			          }, GenericAlertPresenterButton.Dismiss);
		}
	}
}
