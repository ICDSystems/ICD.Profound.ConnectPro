using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters
{
	/// <summary>
	/// INavigationController provides a way for presenters to access each other.
	/// </summary>
	public interface IOsdNavigationController : INavigationController
	{
		/// <summary>
		/// Sets the room for the presenters to represent.
		/// </summary>
		/// <param name="room"></param>
		void SetRoom(IConnectProRoom room);
	}
}
