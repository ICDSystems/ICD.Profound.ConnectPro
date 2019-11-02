using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras
{
	public interface ICameraControlPresenter : IUiPresenter<ICameraControlView>
	{
		/// <summary>
		/// Gets the number of cameras.
		/// </summary>
		int CameraCount { get; }
	}
}
