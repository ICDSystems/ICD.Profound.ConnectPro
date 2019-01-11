using ICD.Connect.Cameras.Devices;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common
{
	public interface ICameraControlPresenter : IUiPresenter<ICameraControlView>
	{
		/// <summary>
		/// Gets/sets the current camera.
		/// </summary>
		ICameraDevice Camera { get; set; }
	}
}
