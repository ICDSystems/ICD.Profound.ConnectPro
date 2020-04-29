using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters
{
	public interface ITouchDisplayPresenter : IPresenter
	{
		/// <summary>
		///     Sets the room for the presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		void SetRoom(IConnectProRoom room);
	}

	public interface ITouchDisplayPresenter<T> : IPresenter<T>, ITouchDisplayPresenter
		where T : ITouchDisplayView
	{
	}
}