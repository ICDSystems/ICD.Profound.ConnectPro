using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters
{
	/// <summary>
	/// INavigationController provides a way for presenters to access each other.
	/// </summary>
	public interface IConnectProNavigationController : INavigationController
	{
		/// <summary>
		/// Sets the room for the presenters to represent.
		/// </summary>
		/// <param name="room"></param>
		void SetRoom(IConnectProRoom room);
	}
}
