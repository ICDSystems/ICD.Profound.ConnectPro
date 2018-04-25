using ICD.Connect.Cameras.Devices;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcCameraPresenter : IPresenter<IVtcCameraView>
	{
		/// <summary>
		/// Gets/sets the current camera.
		/// </summary>
		ICameraDevice Camera { get; set; }
	}
}
