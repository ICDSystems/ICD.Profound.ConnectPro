using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Conference;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Conference
{
	public interface IOsdConferencePresenter : IOsdPresenter<IOsdConferenceView>
	{
		IConferenceDeviceControl ActiveConferenceControl { set; }
	}
}
