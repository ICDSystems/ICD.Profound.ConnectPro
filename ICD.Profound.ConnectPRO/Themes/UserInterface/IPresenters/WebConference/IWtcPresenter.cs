using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference
{
	public interface IWtcPresenter<T> : IWtcPresenter, IUiPresenter<T> where T: IUiView
	{
	}

	public interface IWtcPresenter : IUiPresenter
	{
		IConferenceDeviceControl ActiveConferenceControl { set; }
	}
}