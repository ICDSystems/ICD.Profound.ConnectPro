using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters
{
	public interface IOsdPresenter : IPresenter
	{
		/// <summary>
		/// Sets the room for the presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		void SetRoom(IConnectProRoom room);
	}

	public interface IOsdPresenter<T> : IOsdPresenter, IPresenter<T>
		where T : IOsdView
	{
	}
}
