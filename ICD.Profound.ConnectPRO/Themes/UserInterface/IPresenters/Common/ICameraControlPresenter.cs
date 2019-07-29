using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common
{
	public interface ICameraControlPresenter : IUiPresenter<ICameraControlView>
	{
		/// <summary>
		/// Gets the number of cameras.
		/// </summary>
		int CameraCount { get; }
	}
}
