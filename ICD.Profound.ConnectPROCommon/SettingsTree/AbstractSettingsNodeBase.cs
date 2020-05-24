using ICD.Common.Properties;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree
{
	public abstract class AbstractSettingsNodeBase : ISettingsNodeBase
	{
		private IConnectProRoom m_Room;

		#region Properties

		/// <summary>
		/// The room in context for this settings node.
		/// </summary>
		[CanBeNull]
		public IConnectProRoom Room { get { return m_Room; } }

		/// <summary>
		/// The name of the entry in the settings menu.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The name of the icon shown in the settings menu.
		/// </summary>
		public eSettingsIcon Icon { get; set; }

		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		public virtual bool Visible { get { return Room != null; } }

		/// <summary>
		/// If true, the user must be logged in to access this part of the settings
		/// </summary>
		public virtual bool RequiresLogin { get { return true; } }

		/// <summary>
		/// Returns true if the system has changed and needs to be saved.
		/// </summary>
		public abstract bool Dirty { get; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractSettingsNodeBase()
		{
			Name = "Unnamed";
			Icon = eSettingsIcon.Admin;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			Unsubscribe(m_Room);
		}

		#region Methods

		/// <summary>
		/// Sets the dirty state.
		/// </summary>
		/// <param name="dirty"></param>
		public abstract void SetDirty(bool dirty);

		/// <summary>
		/// Sets the room.
		/// </summary>
		/// <param name="room"></param>
		public virtual void SetRoom([CanBeNull] IConnectProRoom room)
		{
			if (room == m_Room)
				return;

			Unsubscribe(m_Room);
			m_Room = room;
			Subscribe(m_Room);

			Initialize(m_Room);
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Subscribe(IConnectProRoom room)
		{
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Unsubscribe(IConnectProRoom room)
		{
		}

		/// <summary>
		/// Override to initialize the node once a room has been assigned.
		/// </summary>
		protected virtual void Initialize(IConnectProRoom room)
		{
		}

		#endregion
	}
}
