using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference
{
	public interface IWtcBasePresenter : IPopupPresenter<IWtcBaseView>
	{
		IWebConferenceDeviceControl ActiveConferenceControl {get;set; }
	}
}