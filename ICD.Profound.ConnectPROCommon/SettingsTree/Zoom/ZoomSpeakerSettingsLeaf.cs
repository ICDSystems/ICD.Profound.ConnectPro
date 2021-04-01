using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Components.Audio;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Responses;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.Zoom
{
	public sealed class ZoomSpeakerSettingsLeaf : AbstractSettingsLeaf
	{
		public event EventHandler<StringEventArgs> OnSelectedSpeakerNameChanged;
		public event EventHandler OnSpeakersChanged;

		private ZoomRoom m_ZoomRoom;
		private AudioComponent m_AudioComponent;
		private string m_SelectedSpeakerName;

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

				UpdateSelectedSpeaker();
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
		/// Gets the currently selected speaker name.
		/// </summary>
		public string SelectedSpeakerName
		{
			get { return m_SelectedSpeakerName; }
			private set
			{
				if (value == m_SelectedSpeakerName)
					return;

				m_SelectedSpeakerName = value;

				OnSelectedSpeakerNameChanged.Raise(this, new StringEventArgs(m_SelectedSpeakerName));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ZoomSpeakerSettingsLeaf()
		{
			Name = "Speakers";
			Icon = eSettingsIcon.Speaker;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnSelectedSpeakerNameChanged = null;
			OnSpeakersChanged = null;

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
		/// Gets the available speakers.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<AudioOutputLine> GetSpeakers()
		{
			return m_AudioComponent == null
				? Enumerable.Empty<AudioOutputLine>()
				: m_AudioComponent.GetSpeakers();
		}

		/// <summary>
		/// Selects the speaker with the given name.
		/// </summary>
		/// <param name="name"></param>
		public void SetSelectedSpeaker(string name)
		{
			m_ZoomRoom.DefaultSpeakerName = name;

			AudioOutputLine speaker = m_AudioComponent.GetSpeaker(name);
			if (speaker != null)
				m_AudioComponent.SetAudioOutputDeviceById(speaker.Id);

			SetDirty(true);
		}

		#endregion

		#region Private Methods

		private void UpdateSelectedSpeaker()
		{
			SelectedSpeakerName = m_ZoomRoom == null ? null : m_ZoomRoom.DefaultSpeakerName;
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

			audioComponent.OnSpeakersChanged += AudioComponentOnSpeakersChanged;
		}

		/// <summary>
		/// Unsubscribe from the audio component events.
		/// </summary>
		/// <param name="audioComponent"></param>
		private void Unsubscribe(AudioComponent audioComponent)
		{
			if (audioComponent == null)
				return;

			audioComponent.OnSpeakersChanged -= AudioComponentOnSpeakersChanged;
		}

		/// <summary>
		/// Called when the audio input devices change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void AudioComponentOnSpeakersChanged(object sender, EventArgs eventArgs)
		{
			OnSpeakersChanged.Raise(this);
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

			zoomRoom.OnDefaultSpeakerNameChanged += ZoomRoomOnDefaultSpeakerNameChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="zoomRoom"></param>
		private void Unsubscribe(ZoomRoom zoomRoom)
		{
			if (zoomRoom == null)
				return;

			zoomRoom.OnDefaultSpeakerNameChanged += ZoomRoomOnDefaultSpeakerNameChanged;
		}

		/// <summary>
		/// Called when the selected default speaker id changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void ZoomRoomOnDefaultSpeakerNameChanged(object sender, StringEventArgs stringEventArgs)
		{
			UpdateSelectedSpeaker();
		}

		#endregion
	}
}
