using System;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header
{
	public interface IHeaderView : ITouchDisplayView
	{
		event EventHandler OnCenterButtonPressed;

		void SetRoomName(string name);

		void SetTimeLabel(string time);

		void SetCenterButtonIcon(string icon);

		void SetCenterButtonText(string text);
	}
}