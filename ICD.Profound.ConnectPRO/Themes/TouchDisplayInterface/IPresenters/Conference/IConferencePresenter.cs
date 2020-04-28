using ICD.Connect.Conferencing.Controls.Dialing;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference
{
	public interface IConferencePresenter : ITouchDisplayPresenter
	{
		IConferenceDeviceControl ActiveConferenceControl { get; set; }
	}
}