using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.AudioConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference
{
	public interface IAtcBasePresenter : IPopupPresenter<IAtcBaseView>, IContextualControlPresenter
	{
		IConferenceDeviceControl ActiveConferenceControl { get;set; }
	}
}
