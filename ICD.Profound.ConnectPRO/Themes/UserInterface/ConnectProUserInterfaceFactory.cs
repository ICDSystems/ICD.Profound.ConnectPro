using ICD.Connect.Panels;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface
{
	public sealed class ConnectProUserInterfaceFactory :
		AbstractConnectProUserInterfaceFactory<ConnectProUserInterface, IPanelDevice>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProUserInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="originator"></param>
		/// <returns></returns>
		protected override ConnectProUserInterface CreateUserInterface(IPanelDevice originator)
		{
			return new ConnectProUserInterface(originator, Theme);
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsOriginator(IRoom room, ConnectProUserInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Panel.Id);
		}
	}
}
