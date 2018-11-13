using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference
{
	public interface IWtcPresenter<T> : IWtcPresenter, IPresenter<T> where T: IView
	{
	}

	public interface IWtcPresenter : IPresenter
	{
		IWebConferenceDeviceControl ActiveConferenceControl { set; }
	}
}