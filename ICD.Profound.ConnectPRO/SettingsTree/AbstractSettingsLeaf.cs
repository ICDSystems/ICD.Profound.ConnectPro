using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public abstract class AbstractSettingsLeaf : AbstractSettingsNodeBase, ISettingsLeaf
	{
		private bool m_Dirty;

		/// <summary>
		/// Returns true if the system has changed and needs to be saved.
		/// </summary>
		public override bool Dirty { get { return m_Dirty; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		protected AbstractSettingsLeaf(IConnectProRoom room)
			: base(room)
		{
		}

		/// <summary>
		/// Sets the dirty state for this leaf.
		/// </summary>
		/// <param name="dirty"></param>
		public override void SetDirty(bool dirty)
		{
			m_Dirty = dirty;
		}
	}
}
