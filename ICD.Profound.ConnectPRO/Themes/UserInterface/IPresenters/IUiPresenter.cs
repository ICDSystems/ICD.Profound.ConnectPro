using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters
{
	public interface IUiPresenter : IPresenter
	{
		/// <summary>
		/// Sets the room for the presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		void SetRoom(IConnectProRoom room);
	}

	public interface IUiPresenter<T> : IUiPresenter, IPresenter<T>
		where T : IUiView
	{
	}
}
