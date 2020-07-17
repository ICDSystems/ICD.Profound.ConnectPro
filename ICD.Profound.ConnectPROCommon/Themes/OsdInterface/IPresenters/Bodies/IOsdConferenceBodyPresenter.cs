using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Bodies;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Bodies
{
	public interface IOsdConferenceBodyPresenter : IOsdPresenter<IOsdConferenceBodyView>
	{
		IConferenceDeviceControl ActiveConferenceControl { set; }
	}
}
