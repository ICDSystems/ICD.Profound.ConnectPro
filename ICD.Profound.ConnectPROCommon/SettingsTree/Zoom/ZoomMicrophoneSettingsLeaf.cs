using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Components.Audio;
using ICD.Connect.Conferencing.Zoom.Responses;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.Zoom
{
	public sealed class ZoomMicrophoneSettingsLeaf : AbstractSettingsLeaf
	{
        public event EventHandler<StringEventArgs> OnSelectedMicrophoneNameChanged;
        public event EventHandler OnMicrophonesChanged;

		private ZoomRoom m_ZoomRoom;
		private AudioComponent m_AudioComponent;
		private string m_SelectedMicrophoneName;

		#region Properties

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

		/// <summary>
		/// Gets the wrapped zoom room.
		/// </summary>
		public ZoomRoom ZoomRoom
		{
			get { return m_ZoomRoom; }
			private set
			{
				if (value == m_ZoomRoom)
					return;

				Unsubscribe(m_ZoomRoom);
				m_ZoomRoom = value;
				Subscribe(m_ZoomRoom);

				AudioComponent = m_ZoomRoom == null ? null : m_ZoomRoom.Components.GetComponent<AudioComponent>();

				UpdateSelectedMicrophone();
			}
		}

		/// <summary>
		/// Gets the wrapped audio component.
		/// </summary>
		public AudioComponent AudioComponent
		{
			get { return m_AudioComponent; }
			private set
			{
				if (value == m_AudioComponent)
					return;

				Unsubscribe(m_AudioComponent);
				m_AudioComponent = value;
				Subscribe(m_AudioComponent);
			}
		}

		/// <summary>
		/// Gets the currently selected microphone name.
		/// </summary>
		public string SelectedMicrophoneName
		{
			get { return m_SelectedMicrophoneName; }
			private set
			{
				if (value == m_SelectedMicrophoneName)
					return;

				m_SelectedMicrophoneName = value;

				OnSelectedMicrophoneNameChanged.Raise(this, new StringEventArgs(m_SelectedMicrophoneName));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ZoomMicrophoneSettingsLeaf()
		{
			Name = "Microphones";
			Icon = eSettingsIcon.Zoom;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
            OnSelectedMicrophoneNameChanged = null;
            OnMicrophonesChanged = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the room.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			ZoomRoom =
				room == null
					? null
					: room.Originators.GetInstancesRecursive<ZoomRoom>().FirstOrDefault();
		}

		/// <summary>
		/// Gets the available microphones.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<AudioInputLine> GetMicrophones()
		{
			return m_AudioComponent == null
				? Enumerable.Empty<AudioInputLine>()
				: m_AudioComponent.GetMicrophones();
		}

		/// <summary>
		/// Selects the microphone with the given name.
		/// </summary>
		/// <param name="name"></param>
		public void SetSelectedMicrophone(string name)
		{
			m_ZoomRoom.DefaultMicrophoneName = name;

			AudioInputLine microphone = m_AudioComponent.GetMicrophone(name);
			if (microphone != null)
				m_AudioComponent.SetAudioInputDeviceById(microphone.Id);

			SetDirty(true);
		}

        #endregion

		#region Private Methods

		private void UpdateSelectedMicrophone()
		{
			SelectedMicrophoneName = m_ZoomRoom == null ? null : m_ZoomRoom.DefaultMicrophoneName;
		}

		#endregion

		#region AudioComponent Callbacks

		/// <summary>
		/// Subscribe to the audio component events.
		/// </summary>
		/// <param name="audioComponent"></param>
		private void Subscribe(AudioComponent audioComponent)
		{
			if (audioComponent == null)
				return;

			audioComponent.OnMicrophonesChanged += AudioComponentOnMicrophonesChanged;
		}

		/// <summary>
		/// Unsubscribe from the audio component events.
		/// </summary>
		/// <param name="audioComponent"></param>
		private void Unsubscribe(AudioComponent audioComponent)
		{
			if (audioComponent == null)
				return;

			audioComponent.OnMicrophonesChanged -= AudioComponentOnMicrophonesChanged;
		}

		/// <summary>
		/// Called when the audio input devices change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void AudioComponentOnMicrophonesChanged(object sender, EventArgs eventArgs)
		{
			OnMicrophonesChanged.Raise(this);
		}

		#endregion

		#region ZoomRoom Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="zoomRoom"></param>
		private void Subscribe(ZoomRoom zoomRoom)
		{
			if (zoomRoom == null)
				return;

			zoomRoom.OnDefaultMicrophoneNameChanged += ZoomRoomOnDefaultMicrophoneNameChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="zoomRoom"></param>
		private void Unsubscribe(ZoomRoom zoomRoom)
		{
			if (zoomRoom == null)
				return;

			zoomRoom.OnDefaultMicrophoneNameChanged += ZoomRoomOnDefaultMicrophoneNameChanged;
		}

		/// <summary>
		/// Called when the selected default microphone id changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void ZoomRoomOnDefaultMicrophoneNameChanged(object sender, StringEventArgs stringEventArgs)
		{
			UpdateSelectedMicrophone();
		}

		#endregion
	}
}
