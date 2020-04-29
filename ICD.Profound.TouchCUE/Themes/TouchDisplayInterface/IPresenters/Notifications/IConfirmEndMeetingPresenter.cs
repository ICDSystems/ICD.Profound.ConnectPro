using System;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Notifications;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Notifications
{
	public interface IConfirmEndMeetingPresenter : ITouchDisplayPresenter<IConfirmEndMeetingView>
	{
		void Show(string text, Action confirmAction);
	}
}
