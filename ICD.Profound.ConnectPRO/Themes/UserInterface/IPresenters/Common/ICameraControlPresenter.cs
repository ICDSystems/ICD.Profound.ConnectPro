using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common
{
	public interface ICameraControlPresenter : IUiPresenter<ICameraControlView>
	{
		/// <summary>
		/// Gets the number of cameras.
		/// </summary>
		int CameraCount { get; }

		/// <summary>
		/// Sets the VTC routing control to route camera video to.
		/// </summary>
		/// <param name="value"></param>
		void SetVtcDestinationControl(IVideoConferenceRouteControl value);
	}
}
