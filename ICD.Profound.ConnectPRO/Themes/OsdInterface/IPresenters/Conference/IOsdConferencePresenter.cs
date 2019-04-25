using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Conference
{
	public interface IOsdConferencePresenter : IOsdPresenter<IOsdConferenceView>
	{
		IConferenceDeviceControl ActiveConferenceControl { set; }
	}
}
