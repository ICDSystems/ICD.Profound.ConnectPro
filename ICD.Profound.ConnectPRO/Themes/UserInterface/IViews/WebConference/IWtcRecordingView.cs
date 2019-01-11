using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference
{
	public interface IWtcRecordingView : IUiView
	{
		event EventHandler OnStartRecordingButtonPressed;
		event EventHandler OnStopRecordingButtonPressed;

		void SetStartRecordingButtonEnabled(bool enabled);
		void SetStopRecordingButtonEnabled(bool enabled);
	}
}