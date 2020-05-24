using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Zoom;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.Zoom
{
	public sealed class ZoomGeneralSettingsLeaf : AbstractSettingsLeaf
	{
		public event EventHandler<BoolEventArgs> OnMuteAllParticipantsAtMeetingStartChanged;
		public event EventHandler<BoolEventArgs> OnMuteMyCameraAtMeetingStartChanged;
		public event EventHandler<BoolEventArgs> OnEnableRecordingChanged;
		public event EventHandler<BoolEventArgs> OnEnableDialOutChanged;

        private readonly List<ZoomRoom> m_ZoomRooms;

		private bool m_MuteAllParticipantsAtMeetingStart;
		private bool m_MuteMyCameraAtMeetingStart;
		private bool m_EnableRecording;
		private bool m_EnableDialOut;

		#region Properties

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
		public ZoomGeneralSettingsLeaf()
		{
			m_ZoomRooms = new List<ZoomRoom>();

			Name = "General";
			Icon = eSettingsIcon.Zoom;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnMuteAllParticipantsAtMeetingStartChanged = null;
			OnMuteMyCameraAtMeetingStartChanged = null;
			OnEnableRecordingChanged = null;
			OnEnableDialOutChanged = null;

			base.Dispose();
		}

		#region Methods

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

        #endregion

		#region Private Methods

		private void Update()
		{
			UpdateMuteAllParticipants();
			UpdateMuteMyCamera();
			UpdateRecordEnable();
			UpdateDialOutEnable();
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

			Update();
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

		#endregion
	}
}
