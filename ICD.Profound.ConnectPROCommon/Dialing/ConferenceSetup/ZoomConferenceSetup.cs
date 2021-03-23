using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Components.Camera;
using ICD.Connect.Misc.Yepkit.Devices.YkupSwitcher;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.YkupSwitcherInterface;

namespace ICD.Profound.ConnectPROCommon.Dialing.ConferenceSetup
{
	public sealed class ZoomConferenceSetup : AbstractConferenceSetup
	{
		private const long CONFERENCE_SETUP_TIMEOUT = 10 * 1000;

		private readonly SafeTimer m_CallSetupTimer;

		private CameraComponent m_SubscribedCameraComponent;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="control"></param>
		/// <param name="finishAction"></param>
		public ZoomConferenceSetup([NotNull] IConnectProRoom room, [NotNull] IConferenceDeviceControl control, [NotNull] Action finishAction)
			: base(room, control, finishAction)
		{
			m_CallSetupTimer = SafeTimer.Stopped(Timeout);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			m_CallSetupTimer.Dispose();
			Unsubscribe(m_SubscribedCameraComponent);
		}

		#region Methods

		/// <summary>
		/// Starts the conference setup process.
		/// </summary>
		public override void Start()
		{
			base.Start();

			ZoomRoom zoom = Control.Parent as ZoomRoom;
			if (zoom == null)
				throw new InvalidProgramException("Unexpected control parent");

			// Subscribe to the camera update event so we can confirm that the cameras have been routed
			m_SubscribedCameraComponent = zoom.Components.GetComponent<CameraComponent>();
			Subscribe(m_SubscribedCameraComponent);

			// Route the USB devices
			bool routingUsb = RouteUsb(Room);
			if (!routingUsb)
			{
				Finish();
				return;
			}

			// Wait for a timeout
			m_CallSetupTimer.Reset(CONFERENCE_SETUP_TIMEOUT);
		}

		/// <summary>
		/// Ends the conference setup process and executes the finish action.
		/// </summary>
		public override void Finish()
		{
			// Cleanup
			m_CallSetupTimer.Stop();
			Unsubscribe(m_SubscribedCameraComponent);

			base.Finish();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Routes USB for the given room.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		private static bool RouteUsb([NotNull] IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			bool routingUsb = false;

			// Hack - Need to figure out a better way of tracking Zoom mic/camera routing
			foreach (YkupSwitcherDevice switcher in room.Originators.GetInstancesRecursive<YkupSwitcherDevice>())
			{
				// Hack - It could have been a short period of time since the last switch
				TimeSpan sinceLastSwitch = IcdEnvironment.GetUtcTime() - switcher.LastSwitchTime;
				if (sinceLastSwitch.TotalMilliseconds < CONFERENCE_SETUP_TIMEOUT)
					routingUsb = true;

				if (switcher.Route(ConnectProYkupSwitcherInterface.ZOOM_OUTPUT))
					routingUsb = true;
			}

			return routingUsb;
		}

		/// <summary>
		/// Called when the timer elapses before the USB routing completes.
		/// </summary>
		private void Timeout()
		{
			Room.Logger.Log(eSeverity.Warning, "{0} - Conference setup timed out for {1}", Room, Control);

			Finish();
		}

		#endregion

		#region Camera Component Callbacks

		/// <summary>
		/// Subscribe to the camera component events.
		/// </summary>
		/// <param name="cameraComponent"></param>
		private void Subscribe(CameraComponent cameraComponent)
		{
			cameraComponent.OnCamerasUpdated += SubscribedCameraComponentOnCamerasUpdated;
		}

		/// <summary>
		/// Unsubscribe from the camera component events.
		/// </summary>
		/// <param name="cameraComponent"></param>
		private void Unsubscribe(CameraComponent cameraComponent)
		{
			cameraComponent.OnCamerasUpdated -= SubscribedCameraComponentOnCamerasUpdated;
		}

		/// <summary>
		/// Called when the zoom cameras table changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SubscribedCameraComponentOnCamerasUpdated(object sender, EventArgs eventArgs)
		{
			Room.Logger.Log(eSeverity.Informational, "{0} - Conference setup finished routing USB for {1}", Room, Control);

			// Assume our USB devices were done routing
			Finish();
		}

		#endregion
	}
}
