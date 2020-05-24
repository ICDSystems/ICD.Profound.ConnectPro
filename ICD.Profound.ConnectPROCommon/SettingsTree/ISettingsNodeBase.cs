using System;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree
{
	public interface ISettingsNodeBase : IDisposable
	{
		/// <summary>
		/// The room in context for this settings node.
		/// </summary>
		IConnectProRoom Room { get; }

		void SetRoom(IConnectProRoom room);

		/// <summary>
		/// The name of the entry in the settings menu.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The name of the icon shown in the settings menu.
		/// </summary>
		eSettingsIcon Icon { get; }

		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// If true, the user must be logged in to access this part of the settings
		/// </summary>
		bool RequiresLogin { get; }

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
