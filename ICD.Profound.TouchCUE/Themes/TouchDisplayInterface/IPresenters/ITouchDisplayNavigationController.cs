using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters
{
	public interface ITouchDisplayNavigationController : INavigationController
	{
		void SetRoom(IConnectProRoom room);
	}
}