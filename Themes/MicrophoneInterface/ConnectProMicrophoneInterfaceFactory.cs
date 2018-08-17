using ICD.Connect.Audio.Shure;
using ICD.Connect.Partitioning.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.MicrophoneInterface
{
	public sealed class ConnectProMicrophoneInterfaceFactory :
		AbstractConnectProUserInterfaceFactory<ConnectProMicrophoneInterface, IShureMxaDevice>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProMicrophoneInterfaceFactory(ConnectProTheme theme)
			: base(theme)
		{
		}

		/// <summary>
		/// Instantiates the user interface for the given originator.
		/// </summary>
		/// <param name="originator"></param>
		/// <returns></returns>
		protected override ConnectProMicrophoneInterface CreateUserInterface(IShureMxaDevice originator)
		{
			return new ConnectProMicrophoneInterface(originator, Theme);
		}

		/// <summary>
		/// Returns true if the room contains the originator in the given ui.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="ui"></param>
		/// <returns></returns>
		protected override bool RoomContainsOriginator(IRoom room, ConnectProMicrophoneInterface ui)
		{
			return room.Originators.ContainsRecursive(ui.Microphone.Id);
		}
	}
}
