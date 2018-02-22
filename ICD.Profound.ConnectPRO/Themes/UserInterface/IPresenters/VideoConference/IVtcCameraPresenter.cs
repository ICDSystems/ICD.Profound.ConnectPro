using ICD.Connect.Cameras.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcCameraPresenter : IPresenter<IVtcCameraView>
	{
		/// <summary>
		/// Gets/sets the current camera control.
		/// </summary>
		ICameraDeviceControl Camera { get; set; }
	}
}
