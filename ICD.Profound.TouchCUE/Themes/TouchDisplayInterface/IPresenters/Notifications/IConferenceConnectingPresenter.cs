using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Notifications;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Notifications
{
	public interface IConferenceConnectingPresenter : ITouchDisplayPresenter<IConferenceConnectingView>
	{
		void Show(string notificationText);
	}
}