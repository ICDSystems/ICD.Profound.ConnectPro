using System;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public interface ISettingsNodeBase : IDisposable
	{
		/// <summary>
		/// The room in context for this settings node.
		/// </summary>
		IConnectProRoom Room { get; set; }

		/// <summary>
		/// The name of the entry in the settings menu.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The name of the icon shown in the settings menu.
		/// </summary>
		string Icon { get; }

		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Returns true if the system has changed and needs to be saved.
		/// </summary>
		bool Dirty { get; }

		/// <summary>
		/// Sets the dirty state.
		/// </summary>
		/// <param name="dirty"></param>
		void SetDirty(bool dirty);
	}
}
