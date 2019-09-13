using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference
{
	public interface IStartMeetingPresenter : ITouchDisplayPresenter<IStartMeetingView>
	{
		IWebConferenceDeviceControl ActiveConferenceControl { get; set; }
	}
}