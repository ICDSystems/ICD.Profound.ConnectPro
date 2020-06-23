using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Commercial;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.TouchFreeConferencing
{
	public sealed class TouchFreeSettingsLeaf : AbstractSettingsLeaf
	{
		public event EventHandler<IntEventArgs> OnCountdownSecondsChanged;
		public event EventHandler<BoolEventArgs> OnTouchFreeEnabledChanged;
		public event EventHandler<SourceEventArgs> OnDefaultSourceChanged;

		private bool m_TouchFreeEnabled;
		private int m_CountDownSeconds;
		private ISource m_DefaultSource;

		/// <summary>
		/// Constructor.
		/// </summary>
		public TouchFreeSettingsLeaf()
		{
			Name = "Touch Free Conferencing";
			Icon = eSettingsIcon.TouchFree;
		}

		#region Properties

		public bool TouchFreeEnabled
		{ 
			get { return m_TouchFreeEnabled; }
			private set
			{
				if (value == m_TouchFreeEnabled)
					return;

				m_TouchFreeEnabled = value;

				OnTouchFreeEnabledChanged.Raise(this, new BoolEventArgs(m_TouchFreeEnabled));
			}
		}

		public int CountDownSeconds
		{ 
			get { return m_CountDownSeconds; }
			private set
			{
				if (value == m_CountDownSeconds)
					return;

				m_CountDownSeconds = value;

				OnCountdownSecondsChanged.Raise(this, new IntEventArgs(m_CountDownSeconds));
			}
		}

		public ISource DefaultSource
		{
			get { return m_DefaultSource; }
			private set
			{
				if (value == m_DefaultSource)
					return;

				m_DefaultSource = value;

				OnDefaultSourceChanged.Raise(this, new SourceEventArgs(m_DefaultSource));
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCountdownSecondsChanged = null;
			OnTouchFreeEnabledChanged = null;
			OnDefaultSourceChanged = null;

			base.Dispose();
		}

		public void SetCountdownSeconds(int seconds)
		{
			if (Room != null && Room.TouchFree != null)
				Room.TouchFree.CountdownSeconds = seconds;

			SetDirty(true);
		}

		public void SetTouchFreeEnabled(bool enabled)
		{
			if (Room != null && Room.TouchFree != null)
				Room.TouchFree.Enabled = enabled;

			SetDirty(true);
		}

		public void SetDefaultSource(ISource source)
		{
			// TODO - Set on the room
			SetDirty(true);
		}

		public IEnumerable<ISource> GetSources()
		{
			return Room == null
				       ? Enumerable.Empty<ISource>()
				       : Room.Routing.Sources.GetRoomSources();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Override to initialize the node once a room has been assigned.
		/// </summary>
		protected override void Initialize(IConnectProRoom room)
		{
			base.Initialize(room);

			UpdateEnabled();
			UpdateCountdownSeconds();
			UpdateSource();
		}

		private void UpdateEnabled()
		{
			TouchFreeEnabled = Room != null && Room.TouchFree != null && Room.TouchFree.Enabled;
		}

		private void UpdateCountdownSeconds()
		{
			CountDownSeconds =
				Room != null && Room.TouchFree != null
					? Room.TouchFree.CountdownSeconds
					: 0;
		}

		private void UpdateSource()
		{
			throw new NotImplementedException();
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

			TouchFree touchFree = room == null ? null : room.TouchFree;
			if (touchFree == null)
				return;

			touchFree.OnCountDownTimerChanged += TouchFreeOnCountDownTimerChanged;
			touchFree.OnEnabledChanged += TouchFreeOnEnabledChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			TouchFree touchFree = room == null ? null : room.TouchFree;
			if (touchFree == null)
				return;

			touchFree.OnCountDownTimerChanged -= TouchFreeOnCountDownTimerChanged;
			touchFree.OnEnabledChanged -= TouchFreeOnEnabledChanged;
		}

		private void TouchFreeOnEnabledChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateEnabled();
		}

		private void TouchFreeOnCountDownTimerChanged(object sender, IntEventArgs intEventArgs)
		{
			UpdateCountdownSeconds();
		}

		#endregion
	}
}