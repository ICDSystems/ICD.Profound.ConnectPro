namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Popups
{
	public interface IOsdIncomingCallView : IOsdView
	{
		void SetIcon(string icon);

		void SetSourceName(string name);

		void SetCallerInfo(string number);

		void SetBackgroundMode(eOsdIncomingCallBackgroundMode number);

		void PlayRingtone(bool playing);
	}

	public enum eOsdIncomingCallBackgroundMode
	{
		Ringing = 0,
		Rejected = 1
	}
}