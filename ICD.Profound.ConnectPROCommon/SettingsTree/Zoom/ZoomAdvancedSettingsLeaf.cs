using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Components.Audio;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.Zoom
{
	public sealed class ZoomAdvancedSettingsLeaf : AbstractSettingsLeaf
	{
		public event EventHandler<BoolEventArgs> OnReduceAudioReverbChanged;
		public event EventHandler<BoolEventArgs> OnAudioProcessingChanged;

		private readonly List<AudioComponent> m_AudioComponents;

		private bool m_ReduceAudioReverb;
		private bool m_AudioProcessing;

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
		public ZoomAdvancedSettingsLeaf()
		{
			m_AudioComponents = new List<AudioComponent>();

			Name = "Advanced";
			Icon = eSettingsIcon.SystemAdvanced;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnReduceAudioReverbChanged = null;
			OnAudioProcessingChanged = null;

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

        #endregion

		#region Private Methods

		private void Update()
		{
			UpdateAudioReverb();
			UpdateAudioProcessing();
		}

		private void UpdateAudioReverb()
		{
			ReduceAudioReverb = m_AudioComponents.All(c => c.ReduceReverb);
		}

		private void UpdateAudioProcessing()
		{
			AudioProcessing = m_AudioComponents.All(c => !c.IsSapDisabled);
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
	}
}
