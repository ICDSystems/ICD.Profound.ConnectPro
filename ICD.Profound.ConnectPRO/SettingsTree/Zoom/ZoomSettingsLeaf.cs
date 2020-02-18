using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Components.Audio;
using ICD.Connect.Conferencing.Zoom.Components.Camera;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Windows;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Zoom
{
	public sealed class ZoomSettingsLeaf : AbstractSettingsLeaf
	{
		public event EventHandler<BoolEventArgs> OnReduceAudioReverbChanged;
		public event EventHandler<BoolEventArgs> OnAudioProcessingChanged;
		public event EventHandler<BoolEventArgs> OnMuteAllParticipantsAtMeetingStartChanged;
		public event EventHandler<BoolEventArgs> OnMuteMyCameraAtMeetingStartChanged;
		public event EventHandler<BoolEventArgs> OnEnableRecordingChanged;
		public event EventHandler<BoolEventArgs> OnEnableDialOutChanged;
        public event EventHandler<BoolEventArgs> OnUsbCamerasChanged;
        public event EventHandler<BoolEventArgs> OnUsbIdsChanged;

        private readonly List<ZoomRoom> m_ZoomRooms;
        private readonly List<CameraComponent> m_CameraComponents;
		private readonly List<AudioComponent> m_AudioComponents;

		private bool m_ReduceAudioReverb;
		private bool m_AudioProcessing;
		private bool m_MuteAllParticipantsAtMeetingStart;
		private bool m_MuteMyCameraAtMeetingStart;
		private bool m_EnableRecording;
		private bool m_EnableDialOut;


		#region Properties

		/// <summary>
		/// Gets the reduce audio reverb state.
		/// </summary>
		public bool ReduceAudioReverb
		{
			get { return m_ReduceAudioReverb; }
			private set
			{
				if (value == m_ReduceAudioReverb)
					return;

				m_ReduceAudioReverb = value;

				OnReduceAudioReverbChanged.Raise(this, new BoolEventArgs(m_ReduceAudioReverb));
			}
		}

		/// <summary>
		/// Gets the audio processing state.
		/// </summary>
		public bool AudioProcessing
		{
			get { return m_AudioProcessing; }
			private set
			{
				if (value == m_AudioProcessing)
					return;

				m_AudioProcessing = value;

				OnAudioProcessingChanged.Raise(this, new BoolEventArgs(m_AudioProcessing));
			}
		}

		/// <summary>
		/// Gets the mute all participants state.
		/// </summary>
		public bool MuteAllParticipantsAtMeetingStart
		{
			get { return m_MuteAllParticipantsAtMeetingStart; }
			private set
			{
				if (value == m_MuteAllParticipantsAtMeetingStart)
					return;

				m_MuteAllParticipantsAtMeetingStart = value;

				OnMuteAllParticipantsAtMeetingStartChanged.Raise(this, new BoolEventArgs(m_MuteAllParticipantsAtMeetingStart));
			}
		}

		/// <summary>
		/// Gets the mute my camera state.
		/// </summary>
		public bool MuteMyCameraAtMeetingStart
		{
			get { return m_MuteMyCameraAtMeetingStart; }
			private set
			{
				if (value == m_MuteMyCameraAtMeetingStart)
					return;

				m_MuteMyCameraAtMeetingStart = value;

				OnMuteMyCameraAtMeetingStartChanged.Raise(this, new BoolEventArgs(m_MuteMyCameraAtMeetingStart));
			}

		}

		public bool EnableRecording
		{
			get { return m_EnableRecording; }
			private set
			{
				if (value == m_EnableRecording)
					return;

				m_EnableRecording = value;

				OnEnableRecordingChanged.Raise(this, new BoolEventArgs(m_EnableRecording));
			}
		}

		public bool EnableDialOut
		{
			get { return m_EnableDialOut; }
			private set
			{
				if (value == m_EnableDialOut)
					return;

				m_EnableDialOut = value;

				OnEnableDialOutChanged.Raise(this, new BoolEventArgs(m_EnableDialOut));
			}
		}

		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		public override bool Visible
		{
			get
			{
				return base.Visible &&
				       Room != null &&
				       Room.Originators.GetInstancesRecursive<ZoomRoom>().Any();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ZoomSettingsLeaf()
		{
			m_ZoomRooms = new List<ZoomRoom>();
			m_CameraComponents = new List<CameraComponent>();
			m_AudioComponents = new List<AudioComponent>();
			m_ZoomRooms = new List<ZoomRoom>();

			Name = "Zoom";
			Icon = SettingsTreeIcons.ICON_ZOOM;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnReduceAudioReverbChanged = null;
			OnAudioProcessingChanged = null;
			OnMuteAllParticipantsAtMeetingStartChanged = null;
            OnUsbCamerasChanged = null;
            OnUsbIdsChanged = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Enables/disables audio reverb reduction.
		/// </summary>
		/// <param name="reduceReverb"></param>
		public void SetReduceAudioReverb(bool reduceReverb)
		{
			m_AudioComponents.ForEach(c => c.SetReduceReverb(reduceReverb));
		}

		/// <summary>
		/// Enables/disables audio processing.
		/// </summary>
		/// <param name="audioProcessing"></param>
		public void SetAudioProcessing(bool audioProcessing)
		{
			m_AudioComponents.ForEach(c => c.SetSapDisabled(!audioProcessing));
		}

		/// <summary>
		/// Enables/disables the automatic muting of participants on meeting start.
		/// </summary>
		/// <param name="muteAll"></param>
		public void SetMuteAllParticipantsAtMeetingStart(bool muteAll)
		{
			m_ZoomRooms.ForEach(z => z.MuteParticipantsOnStart = muteAll);
			SetDirty(true);
		}

		/// <summary>
		/// Enables/Disabled the automatic (from our program) muting of this devices's camera on meeting start
		/// </summary>
		/// <param name="muteCamera"></param>
		public void SetMuteMyCameraAtMeetingStart(bool muteCamera)
		{
			m_ZoomRooms.ForEach(z => z.MuteMyCameraOnStart = muteCamera);
			SetDirty(true);
		}

		public void SetEnableRecording(bool enableRecording)
		{
			m_ZoomRooms.ForEach(z => z.RecordEnabled = enableRecording);
			SetDirty(true);
		}

		public void SetEnableDialOut(bool enableDialOut)
		{
			m_ZoomRooms.ForEach(z => z.DialOutEnabled = enableDialOut);
			SetDirty(true);
		}

		/// <summary>
		/// Sets the Usb Id associated with the camera device.
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="usbInfo"></param>
        public void SetUsbIdForCamera([NotNull] IDeviceBase camera, WindowsDevicePathInfo? usbInfo)
        {
			m_ZoomRooms.ForEach(z => z.SetUsbIdForCamera(camera, usbInfo));
			SetDirty(true);
        }

		/// <summary>
		/// Gets teh Usb Id associated with the camera device.
		/// </summary>
		/// <param name="camera"></param>
		/// <returns></returns>
        public WindowsDevicePathInfo? GetUsbIdForCamera([NotNull] IDeviceBase camera)
        {
            WindowsDevicePathInfo info;
            bool found = m_ZoomRooms.SelectMany(z => z.GetUsbCameras())
                                    .Where(kvp => kvp.Key == camera)
                                    .Select(kvp => kvp.Value)
                                    .TryFirst(out info);

            return found ? info : (WindowsDevicePathInfo?)null;

        }

        public string GetUsbDeviceName(WindowsDevicePathInfo usbInfo)
        {
            CameraInfo info;
            bool found = m_CameraComponents.SelectMany(cc => cc.GetCameras())
                                           .Where(c => new WindowsDevicePathInfo(c.UsbId).DeviceId == usbInfo.DeviceId)
                                           .TryFirst(out info);

            return found ? string.Format("{0} ({1})", info.Name, new WindowsDevicePathInfo(info.UsbId).DeviceId) : null;
        }

		public IEnumerable<IDeviceBase> GetCameraDevices()
        {
            return Room == null
                       ? Enumerable.Empty<IDeviceBase>()
                       : Room.Originators
                             .GetInstancesRecursive<ICameraDevice>();
        }

        public IEnumerable<WindowsDevicePathInfo> GetUsbIds()
        {
            return m_ZoomRooms.SelectMany(z => z.GetUsbCameras())
                              .Select(kvp => kvp.Value)
                              .Concat(m_CameraComponents.SelectMany(cc => cc.GetCameras()
                                                        .Select(i => new WindowsDevicePathInfo(i.UsbId))))
                              .Distinct(d => d.DeviceId);
        }

        #endregion

		#region Private Methods

		private void Update()
		{
			UpdateAudioReverb();
			UpdateAudioProcessing();
			UpdateMuteAllParticipants();
			UpdateMuteMyCamera();
			UpdateRecordEnable();
			UpdateDialOutEnable();
			UpdateUsbCameras();
		}

		private void UpdateAudioReverb()
		{
			ReduceAudioReverb = m_AudioComponents.All(c => c.ReduceReverb);
		}

		private void UpdateAudioProcessing()
		{
			AudioProcessing = m_AudioComponents.All(c => !c.IsSapDisabled);
		}

		private void UpdateMuteAllParticipants()
		{
			MuteAllParticipantsAtMeetingStart = m_ZoomRooms.All(z => z.MuteParticipantsOnStart);
		}

		private void UpdateMuteMyCamera()
		{
			MuteMyCameraAtMeetingStart = m_ZoomRooms.All(z => z.MuteMyCameraOnStart);
		}

		private void UpdateRecordEnable()
		{
			EnableRecording = m_ZoomRooms.All(z => z.RecordEnabled);
		}

		private void UpdateDialOutEnable()
		{
			EnableDialOut = m_ZoomRooms.All(z => z.DialOutEnabled);
		}

        private void UpdateUsbCameras()
        {
			OnUsbCamerasChanged.Raise(this, new BoolEventArgs(true));
			OnUsbIdsChanged.Raise(this, new BoolEventArgs(true));
        }

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			IEnumerable<ZoomRoom> zoomRooms =
				room == null
					? Enumerable.Empty<ZoomRoom>()
					: room.Originators.GetInstancesRecursive<ZoomRoom>();

			m_ZoomRooms.Clear();
			m_ZoomRooms.AddRange(zoomRooms);
			m_ZoomRooms.ForEach(Subscribe);

            IEnumerable<CameraComponent> cameraComponents =
                room == null
                    ? Enumerable.Empty<CameraComponent>()
                    : room.Originators
                          .GetInstancesRecursive<ZoomRoom>()
                          .Select(z => z.Components.GetComponent<CameraComponent>());

			m_CameraComponents.Clear();
			m_CameraComponents.AddRange(cameraComponents);
			m_CameraComponents.ForEach(Subscribe);

			IEnumerable<AudioComponent> audioComponents =
				m_ZoomRooms.Select(z => z.Components.GetComponent<AudioComponent>());

			m_AudioComponents.Clear();
			m_AudioComponents.AddRange(audioComponents);
			m_AudioComponents.ForEach(Subscribe);

			Update();
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

            m_ZoomRooms.ForEach(Unsubscribe);
			m_ZoomRooms.Clear();

			m_CameraComponents.ForEach(Unsubscribe);
			m_CameraComponents.Clear();

			m_AudioComponents.ForEach(Unsubscribe);
			m_AudioComponents.Clear();

			m_ZoomRooms.ForEach(Unsubscribe);
			m_ZoomRooms.Clear();

			Update();
		}

		#endregion

        #region Camera Component Callbacks

		/// <summary>
		/// Subscribe to the camera component events.
		/// </summary>
		/// <param name="cameraComponent"></param>
        private void Subscribe(CameraComponent cameraComponent)
        {
			cameraComponent.OnCamerasUpdated += CameraComponentOnCamerasUpdated;
        }

		/// <summary>
		/// Unsubscribe from the camera component events.
		/// </summary>
		/// <param name="cameraComponent"></param>
        private void Unsubscribe(CameraComponent cameraComponent)
        {
            cameraComponent.OnCamerasUpdated -= CameraComponentOnCamerasUpdated;
        }

        private void CameraComponentOnCamerasUpdated(object sender, EventArgs e)
        {
			UpdateUsbCameras();
        }

        #endregion

        #region AudioComponent Callbacks

		/// <summary>
		/// Subscribe to the audio component events.
		/// </summary>
		/// <param name="audioComponent"></param>
		private void Subscribe(AudioComponent audioComponent)
		{
			audioComponent.OnReduceReverbChanged += AudioComponentOnReduceReverbChanged;
			audioComponent.OnSoftwareAudioProcessingChanged += AudioComponentOnSoftwareAudioProcessingChanged;
		}

		/// <summary>
		/// Unsubscribe from the audio component events.
		/// </summary>
		/// <param name="audioComponent"></param>
		private void Unsubscribe(AudioComponent audioComponent)
		{
			audioComponent.OnReduceReverbChanged -= AudioComponentOnReduceReverbChanged;
			audioComponent.OnSoftwareAudioProcessingChanged -= AudioComponentOnSoftwareAudioProcessingChanged;
		}

		/// <summary>
		/// Called when the audio processing state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void AudioComponentOnSoftwareAudioProcessingChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateAudioProcessing();
		}

		/// <summary>
		/// Called when the reduce reverb state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void AudioComponentOnReduceReverbChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateAudioReverb();
		}

		#endregion

		#region ZoomRoom Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="zoomRoom"></param>
		private void Subscribe(ZoomRoom zoomRoom)
		{
			zoomRoom.OnMuteParticipantsOnStartChanged += ZoomRoomOnMuteParticipantOnStartChanged;
			zoomRoom.OnMuteMyCameraOnStartChanged += ZoomRoomOnMuteMyCameraOnStartChanged;
			zoomRoom.OnRecordEnabledChanged += ZoomRoomOnRecordEnabledChanged;
			zoomRoom.OnDialOutEnabledChanged += ZoomRoomOnDialOutEnabledChanged;
            zoomRoom.OnUsbCamerasChanged += ZoomRoomOnUsbCamerasChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="zoomRoom"></param>
		private void Unsubscribe(ZoomRoom zoomRoom)
		{
			zoomRoom.OnMuteParticipantsOnStartChanged -= ZoomRoomOnMuteParticipantOnStartChanged;
			zoomRoom.OnMuteMyCameraOnStartChanged -= ZoomRoomOnMuteMyCameraOnStartChanged;
			zoomRoom.OnRecordEnabledChanged -= ZoomRoomOnRecordEnabledChanged;
			zoomRoom.OnDialOutEnabledChanged -= ZoomRoomOnDialOutEnabledChanged;
            zoomRoom.OnUsbCamerasChanged -= ZoomRoomOnUsbCamerasChanged;
		}

		/// <summary>
		/// Called when the mute on entry state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ZoomRoomOnMuteParticipantOnStartChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateMuteAllParticipants();
		}

		private void ZoomRoomOnMuteMyCameraOnStartChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateMuteMyCamera();
		}

		private void ZoomRoomOnRecordEnabledChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateRecordEnable();
		}

		private void ZoomRoomOnDialOutEnabledChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateDialOutEnable();
		}

        private void ZoomRoomOnUsbCamerasChanged(object sender, EventArgs e)
        {
            UpdateUsbCameras();
        }

		#endregion
	}
}
