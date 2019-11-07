using ICD.Connect.Conferencing.Controls.Layout;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras
{
	public interface ICameraLayoutPresenter : IUiPresenter<ICameraLayoutView>
	{
		/// <summary>
		/// Sets the rooms layout control.
		/// </summary>
		/// <param name="value"></param>
		void SetDestinationLayoutControl(IConferenceLayoutControl value);
	}
}
