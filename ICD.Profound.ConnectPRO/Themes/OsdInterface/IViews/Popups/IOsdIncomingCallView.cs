namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Popups
{
	public interface IOsdIncomingCallView : IOsdView
	{
		void SetIcon(string icon);

		void SetSourceName(string name);

		void SetCallerInfo(string number);

		void SetBackgroundMode(eOsdIncomingCallBackgroundMode number);
	}

	public enum eOsdIncomingCallBackgroundMode
	{
		Ringing = 0,
		Rejected = 1
	}
}