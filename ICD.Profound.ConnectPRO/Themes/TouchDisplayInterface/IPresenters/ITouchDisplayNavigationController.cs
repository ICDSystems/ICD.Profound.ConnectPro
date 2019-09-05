using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters
{
	public interface ITouchDisplayNavigationController : INavigationController
	{
		void SetRoom(IConnectProRoom room);
	}
}