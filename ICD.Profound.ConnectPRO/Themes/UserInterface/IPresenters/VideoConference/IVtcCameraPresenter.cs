using ICD.Connect.Cameras;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcCameraPresenter : IPresenter<IVtcCameraView>
	{
		/// <summary>
		/// Gets/sets the current camera device.
		/// </summary>
		ICameraDevice Camera { get; set; }
	}
}
