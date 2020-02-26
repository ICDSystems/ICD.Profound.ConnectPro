using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference
{
	public interface IConferenceBasePresenter : IPopupPresenter<IConferenceBaseView>, IContextualControlPresenter, IMainPagePresenter
	{
		IConferenceDeviceControl ActiveConferenceControl { get; }
	}
}