using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcButtonListPresenter : IUiPresenter<IVtcButtonListView>
	{
		void ShowMenu(ushort index);
	}
}
