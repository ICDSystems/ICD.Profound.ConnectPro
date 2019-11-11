using System;
using ICD.Common.Utils.Extensions;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public abstract class AbstractSettingsLeaf : AbstractSettingsNodeBase, ISettingsLeaf
	{
		/// <summary>
		/// Raised when the settings change.
		/// </summary>
		public event EventHandler OnSettingsChanged;

		private bool m_Dirty;

		/// <summary>
		/// Returns true if the system has changed and needs to be saved.
		/// </summary>
		public override bool Dirty { get { return m_Dirty; } }

		/// <summary>
		/// Sets the dirty state for this leaf.
		/// </summary>
		/// <param name="dirty"></param>
		public override void SetDirty(bool dirty)
		{
			m_Dirty = dirty;

			if (dirty)
				RaiseSettingsChanged();
		}

		/// <summary>
		/// Raises the OnSettingsChanged event.
		/// </summary>
		protected void RaiseSettingsChanged()
		{
			OnSettingsChanged.Raise(this);
		}
	}
}
