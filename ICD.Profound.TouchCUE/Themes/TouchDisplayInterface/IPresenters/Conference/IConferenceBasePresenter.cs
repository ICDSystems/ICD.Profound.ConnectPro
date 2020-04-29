using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference
{
	public interface IConferenceBasePresenter : IPopupPresenter<IConferenceBaseView>, IContextualControlPresenter, IMainPagePresenter
	{
		IConferenceDeviceControl ActiveConferenceControl { get; }
	}
}