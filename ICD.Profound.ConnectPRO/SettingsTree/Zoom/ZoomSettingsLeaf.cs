using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Components.Audio;
using ICD.Connect.Conferencing.Zoom.Components.Call;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Zoom
{
	public sealed class ZoomSettingsLeaf : AbstractSettingsLeaf
	{
		public event EventHandler<BoolEventArgs> OnReduceAudioReverbChanged;
		public event EventHandler<BoolEventArgs> OnAudioProcessingChanged;
		public event EventHandler<BoolEventArgs> OnMuteAllParticipantsAtMeetingStartChanged;

		private List<AudioComponent> m_AudioComponents;
		private List<CallComponent> m_CallComponents; 

		private bool m_ReduceAudioReverb;
		private bool m_AudioProcessing;
		private bool m_MuteAllParticipantsAtMeetingStart;

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
		/// Determines if the node should be visible.
		/// </summary>
		public override bool Visible
		{
			get { return base.Visible && Room.Originators.GetInstancesRecursive<ZoomRoom>().Any(); }
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ZoomSettingsLeaf(IConnectProRoom room)
			: base(room)
		{
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
			m_AudioComponents.ForEach(c => c.SetSapDisabled(audioProcessing));
		}

		/// <summary>
		/// Enables/disables the automatic muting of participants on meeting start.
		/// </summary>
		/// <param name="muteAll"></param>
		public void SetMuteAllParticipantsAtMeetingStart(bool muteAll)
		{
			m_CallComponents.ForEach(c => c.EnableMuteUserOnEntry(muteAll));
		}

		#endregion

		#region Private Methods

		private void Update()
		{
			UpdateAudioReverb();
			UpdateAudioProcessing();
			UpdateMuteAllParticipants();
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
			MuteAllParticipantsAtMeetingStart = m_CallComponents.All(c => c.MuteUserOnEntry);
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

			m_AudioComponents =
				room == null
					? new List<AudioComponent>()
					: room.Originators
					      .GetInstancesRecursive<ZoomRoom>()
					      .Select(z => z.Components.GetComponent<AudioComponent>())
					      .ToList();

			m_CallComponents =
				room == null
					? new List<CallComponent>()
					: room.Originators
					      .GetInstancesRecursive<ZoomRoom>()
					      .Select(z => z.Components.GetComponent<CallComponent>())
					      .ToList();

			m_AudioComponents.ForEach(Subscribe);
			m_CallComponents.ForEach(Subscribe);

			Update();
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_AudioComponents != null)
				m_AudioComponents.ForEach(Unsubscribe);
			m_AudioComponents = null;

			if (m_CallComponents != null)
				m_CallComponents.ForEach(Unsubscribe);
			m_CallComponents = null;

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
		/// Subscribe to the call component events.
		/// </summary>
		/// <param name="callComponent"></param>
		private void Subscribe(CallComponent callComponent)
		{
			callComponent.OnMuteUserOnEntryChanged += CallComponentOnMuteUserOnEntryChanged;
		}

		/// <summary>
		/// Unsubscribe from the call component events.
		/// </summary>
		/// <param name="callComponent"></param>
		private void Unsubscribe(CallComponent callComponent)
		{
			callComponent.OnMuteUserOnEntryChanged -= CallComponentOnMuteUserOnEntryChanged;
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

		#endregion
	}
}
