using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Components.Audio;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Zoom
{
	public sealed class ZoomSettingsLeaf : AbstractSettingsLeaf
	{
		public event EventHandler<BoolEventArgs> OnReduceAudioReverbChanged;
		public event EventHandler<BoolEventArgs> OnAudioProcessingChanged;
		public event EventHandler<BoolEventArgs> OnMuteAllParticipantsAtMeetingStartChanged;
		public event EventHandler<BoolEventArgs> OnMuteMyCameraAtMeetingStartChanged;

		private readonly List<AudioComponent> m_AudioComponents;
		private readonly List<ZoomRoomConferenceControl> m_ConferenceControls; 

		private bool m_ReduceAudioReverb;
		private bool m_AudioProcessing;
		private bool m_MuteAllParticipantsAtMeetingStart;
		private bool m_MuteMyCameraAtMeetingStart;

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
			m_AudioComponents = new List<AudioComponent>();
			m_ConferenceControls = new List<ZoomRoomConferenceControl>();

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
			m_ConferenceControls.ForEach(c => c.MuteUserOnEntry = muteAll);
		}

		/// <summary>
		/// Enables/Disabled the automatic (from our program) muting of this devices's camera on meeting start
		/// </summary>
		/// <param name="muteCamera"></param>
		public void SetMuteMyCameraAtMeetingStart(bool muteCamera)
		{
			m_ConferenceControls.ForEach(c => c.MuteMyCameraOnStart = muteCamera);
		}

		#endregion

		#region Private Methods

		private void Update()
		{
			UpdateAudioReverb();
			UpdateAudioProcessing();
			UpdateMuteAllParticipants();
			UpdateMuteMyCamera();
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
			MuteAllParticipantsAtMeetingStart = m_ConferenceControls.All(c => c.MuteUserOnEntry);
		}

		private void UpdateMuteMyCamera()
		{
			MuteMyCameraAtMeetingStart = m_ConferenceControls.All(c => c.MuteMyCameraOnStart);
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

			IEnumerable<AudioComponent> audioComponents =
				room == null
					? Enumerable.Empty<AudioComponent>()
					: room.Originators
					      .GetInstancesRecursive<ZoomRoom>()
					      .Select(z => z.Components.GetComponent<AudioComponent>());

			m_AudioComponents.Clear();
			m_AudioComponents.AddRange(audioComponents);
			m_AudioComponents.ForEach(Subscribe);

			IEnumerable<ZoomRoomConferenceControl> conferenceControls =
				room == null
					? Enumerable.Empty<ZoomRoomConferenceControl>()
					: room.Originators
					      .GetInstancesRecursive<ZoomRoom>()
					      .Select(z => z.Controls.GetControl<ZoomRoomConferenceControl>());

			m_ConferenceControls.Clear();
			m_ConferenceControls.AddRange(conferenceControls);
			m_ConferenceControls.ForEach(Subscribe);

			Update();
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			m_AudioComponents.ForEach(Unsubscribe);
			m_AudioComponents.Clear();

			m_ConferenceControls.ForEach(Unsubscribe);
			m_ConferenceControls.Clear();

			Update();
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

		#region CallComponent Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="callComponent"></param>
		private void Subscribe(ZoomRoomConferenceControl callComponent)
		{
			callComponent.OnMuteUserOnEntryChanged += CallComponentOnMuteUserOnEntryChanged;
			callComponent.OnMuteMyCameraOnStartChanged += CallComponentOnMuteMyCameraOnStartChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="callComponent"></param>
		private void Unsubscribe(ZoomRoomConferenceControl callComponent)
		{
			callComponent.OnMuteUserOnEntryChanged -= CallComponentOnMuteUserOnEntryChanged;
			callComponent.OnMuteMyCameraOnStartChanged -= CallComponentOnMuteMyCameraOnStartChanged;
		}

		/// <summary>
		/// Called when the mute on entry state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void CallComponentOnMuteUserOnEntryChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateMuteAllParticipants();
		}

		private void CallComponentOnMuteMyCameraOnStartChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateMuteMyCamera();
		}

		#endregion
	}
}
