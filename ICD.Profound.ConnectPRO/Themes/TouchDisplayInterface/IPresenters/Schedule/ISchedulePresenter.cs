using System;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Schedule
{
	public interface ISchedulePresenter : ITouchDisplayPresenter<IScheduleView>, IMainPagePresenter
	{
		event EventHandler OnRefreshed;
	}
}