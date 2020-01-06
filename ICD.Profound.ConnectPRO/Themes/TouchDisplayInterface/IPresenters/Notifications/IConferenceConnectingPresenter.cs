using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Notifications;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Notifications
{
	public interface IConferenceConnectingPresenter : ITouchDisplayPresenter<IConferenceConnectingView>
	{
		void Show(string notificationText);
	}
}