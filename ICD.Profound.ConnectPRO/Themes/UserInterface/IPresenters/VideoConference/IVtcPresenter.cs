using ICD.Connect.Conferencing.Controls.Dialing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcPresenter : IUiPresenter
	{
		IConferenceDeviceControl ActiveConferenceControl { set; }
	}
}