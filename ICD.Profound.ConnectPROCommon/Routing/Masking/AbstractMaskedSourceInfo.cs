using System;
using ICD.Common.Properties;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Routing.Masking
{
	public abstract class AbstractMaskedSourceInfo : IMaskedSourceInfo
	{
		private readonly IConnectProRoom m_Room;
		private ISource m_Source;
		private bool? m_Mask;
		private bool? m_MaskOverride;

		#region Properties

		/// <summary>
		/// Gets the room.
		/// </summary>
		[NotNull]
		public IConnectProRoom Room { get { return m_Room; } }

		/// <summary>
		/// Gets/sets the source for this mask.
		/// </summary>
		[CanBeNull]
		public ISource Source
		{
			get { return m_Source; }
			set
			{
				if (m_Source == value)
					return;

				Unsubscribe(m_Source);
				m_Source = value;
				Subscribe(m_Source);
			}
		}

		public bool Mask 
		{ 
			get { return !m_Mask.HasValue || m_Mask.Value; }
			protected set
			{
				if (m_Mask == value)
					return;

				m_Mask = value;

				CheckMaskState();
			}
		}

		public bool? MaskOverride
		{
			get { return m_MaskOverride; }
			set
			{
				if (m_MaskOverride == value)
					return;

				m_MaskOverride = value;

				CheckMaskState();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		protected AbstractMaskedSourceInfo([NotNull] IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			m_Room = room;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			Source = null;
		}

		#region Methods

		/// <summary>
		/// Override to perform the masking operation.
		/// </summary>
		protected virtual void PerformMask()
		{
			Room.Routing.RouteOsd(this);
		}

		/// <summary>
		/// Override to perform the unmasking operation.
		/// </summary>
		protected virtual void PerformUnmask()
		{
			Room.Routing.RouteToAllDisplays(Source, this);
		}

		/// <summary>
		/// Subscribe to the source events.
		/// </summary>
		/// <param name="source"></param>
		protected virtual void Subscribe(ISource source)
		{
		}

		/// <summary>
		/// Unsubscribe from the source events.
		/// </summary>
		/// <param name="source"></param>
		protected virtual void Unsubscribe(ISource source)
		{
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Performs the masking operation based on the current mask state.
		/// </summary>
		private void CheckMaskState()
		{
			if (MaskOverride ?? Mask)
				PerformMask();
			else
				PerformUnmask();
		}

		#endregion
	}
}
