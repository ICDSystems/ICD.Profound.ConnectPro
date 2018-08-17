using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface
{
	public sealed class ConnectProOsdInterfaceFactory :
		AbstractConnectProUserInterfaceFactory<ConnectProOsdInterface, OsdPanelDevice>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProOsdInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="originator"></param>
		/// <returns></returns>
		protected override ConnectProOsdInterface CreateUserInterface(OsdPanelDevice originator)
		{
			return new ConnectProOsdInterface(originator, Theme);
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsOriginator(IRoom room, ConnectProOsdInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Panel.Id);
		}
	}
}
