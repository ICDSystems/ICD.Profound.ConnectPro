using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference
{
	public interface IWtcLeftMenuPresenter : IWtcPresenter<IWtcLeftMenuView>
	{
		void ShowMenu(ushort index);
	}
}