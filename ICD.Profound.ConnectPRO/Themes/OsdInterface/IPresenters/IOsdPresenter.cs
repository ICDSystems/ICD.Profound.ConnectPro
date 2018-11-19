using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters
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
