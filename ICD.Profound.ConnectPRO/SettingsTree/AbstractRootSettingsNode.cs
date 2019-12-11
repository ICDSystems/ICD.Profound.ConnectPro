using System;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Cores;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public abstract class AbstractRootSettingsNode : AbstractStaticSettingsNode, IRootSettingsNode
	{
		/// <summary>
		/// If true, the user must be logged in to access this part of the settings
		/// </summary>
		public override bool RequiresLogin { get { return false; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		protected AbstractRootSettingsNode(IConnectProRoom room)
		{
			Name = "Settings";
			Room = room;
		}

		/// <summary>
		/// If there are any dirty nodes the core is saved back to disk and the nodes are marked as clean.
		/// </summary>
		public void SaveDirtySettings()
		{
			if (Room == null)
				throw new InvalidOperationException("No room assigned to node");

			if (!Dirty)
				return;

			ICoreSettings settings = Room.Core.CopySettings();
			FileOperations.SaveSettings(settings, true);

			SetDirty(false);
		}
	}
}
