using System;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Notifications;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Notifications
{
	public interface IConfirmEndMeetingPresenter : ITouchDisplayPresenter<IConfirmEndMeetingView>
	{
		void Show(string text, Action confirmAction);
	}
}
