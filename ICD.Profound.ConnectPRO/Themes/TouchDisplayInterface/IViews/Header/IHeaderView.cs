using System;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header
{
	public interface IHeaderView : ITouchDisplayView
	{
		event EventHandler OnStartEndMeetingPressed;

		void SetRoomName(string name);

		void SetTimeLabel(string time);

		void SetStartEndMeetingButtonMode(eStartEndMeetingMode mode);
	}

	public enum eStartEndMeetingMode : ushort
	{
		StartMeeting = 0,
		EndMeeting = 1
	}
}