using ICD.Connect.Conferencing.Controls.Layout;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.Camera;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.Camera
{
	public interface ICameraLayoutPresenter : ITouchDisplayPresenter<ICameraLayoutView>
	{
		/// <summary>
		/// Sets the rooms layout control.
		/// </summary>
		/// <param name="control"></param>
		void SetConferenceLayoutControl(IConferenceLayoutControl control);
	}
}
