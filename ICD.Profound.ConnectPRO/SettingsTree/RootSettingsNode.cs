using System;
using System.Collections.Generic;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.SettingsTree.Administrative;
using ICD.Profound.ConnectPRO.SettingsTree.Conferencing;
using ICD.Profound.ConnectPRO.SettingsTree.CUE;
using ICD.Profound.ConnectPRO.SettingsTree.RoomCombine;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public sealed class RootSettingsNode : AbstractSettingsNode
	{
		private readonly List<ISettingsNodeBase> m_Children;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public RootSettingsNode(IConnectProRoom room)
			: base(room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			Name = "Administrative";
			Image = SettingsTreeImages.IMAGE_ADMIN;
			Icon = SettingsTreeIcons.ICON_ADMIN;
			Prompt = "Please choose an administrative option from the menu to the left.";

			m_Children = new List<ISettingsNodeBase>
			{
				new AdministrativeSettingsNode(room),
				new ConferencingSettingsNode(room),
				new CueSettingsNode(room),
				new RoomCombineSettingsNode(room)
			};
		}

		/// <summary>
		/// Gets the child nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<ISettingsNodeBase> GetChildren()
		{
			return m_Children;
		}
	}
}
