using System.Collections.Generic;
using System.Linq;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public abstract class AbstractSettingsNode : AbstractSettingsNodeBase, ISettingsNode
	{
		/// <summary>
		/// The path to the image graphic shown when this node is active.
		/// </summary>
		public string Image { get; set; }

		/// <summary>
		/// The help text shown when this node is active.
		/// </summary>
		public string Prompt { get; set; }

		/// <summary>
		/// Returns true if the system has changed and needs to be saved.
		/// </summary>
		public override bool Dirty { get { return GetChildren().Any(c => c.Dirty); } }

		/// <summary>
		/// Determines if the node should be visible.
		/// </summary>
		public override bool Visible { get { return base.Visible && GetChildren().Any(c => c.Visible); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractSettingsNode()
		{
			Image = SettingsTreeImages.IMAGE_THINKER;
			Prompt = "Please choose a settings option from the menu to the left.";
		}

		/// <summary>
		/// Gets the child nodes.
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<ISettingsNodeBase> GetChildren();

		/// <summary>
		/// Sets the dirty state for all child nodes.
		/// </summary>
		/// <param name="dirty"></param>
		public override void SetDirty(bool dirty)
		{
			foreach (ISettingsNodeBase child in GetChildren())
				child.SetDirty(dirty);
		}

		/// <summary>
		/// Override to initialize the node once a room has been assigned.
		/// </summary>
		protected override void Initialize(IConnectProRoom room)
		{
			base.Initialize(room);

			foreach (ISettingsNodeBase child in GetChildren())
				child.Room = room;
		}
	}
}
