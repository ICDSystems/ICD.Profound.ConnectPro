using System;
using ICD.Common.Properties;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public abstract class AbstractSettingsNodeBase : ISettingsNodeBase
	{
		private readonly IConnectProRoom m_Room;

		/// <summary>
		/// The room in context for this settings node.
		/// </summary>
		[NotNull]
		public IConnectProRoom Room { get { return m_Room; } }

		/// <summary>
		/// The name of the entry in the settings menu.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The name of the icon shown in the settings menu.
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		public virtual bool Visible { get { return true; } }

		/// <summary>
		/// Returns true if the system has changed and needs to be saved.
		/// </summary>
		public abstract bool Dirty { get; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		protected AbstractSettingsNodeBase(IConnectProRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			m_Room = room;
			Subscribe(m_Room);

			Name = "Unnamed";
			Icon = SettingsTreeIcons.ICON_ADMIN;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			Unsubscribe(m_Room);
		}

		/// <summary>
		/// Sets the dirty state.
		/// </summary>
		/// <param name="dirty"></param>
		public abstract void SetDirty(bool dirty);

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

		#endregion
	}
}
