using System;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Notifications
{
	public interface IIncomingCallView : ITouchDisplayView
	{
		event EventHandler OnAnswerButtonPressed;

		event EventHandler OnRejectButtonPressed;

		void SetIcon(string icon);

		void SetCallerInfo(string number);

		void SetAnswerButtonMode(eIncomingCallAnswerButtonMode number);

		void SetRejectButtonVisibility(bool visible);

		void PlayRingtone(bool playing);
	}

	public enum eIncomingCallAnswerButtonMode
	{
		Ringing = 0,
		Rejected = 1
	}
}