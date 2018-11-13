using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference
{
	public interface IWtcButtonListPresenter : IWtcPresenter<IWtcButtonListView>
	{
		void ShowMenu(ushort index);
	}
}