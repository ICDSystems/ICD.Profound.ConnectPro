using ICD.Connect.Conferencing.Controls.Dialing;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference
{
	public interface IConferencePresenter : ITouchDisplayPresenter
	{
		IConferenceDeviceControl ActiveConferenceControl { get; set; }
	}
}