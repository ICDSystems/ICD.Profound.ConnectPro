using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference
{
	public interface IWtcJoinByIdView : IUiView
	{
		event EventHandler OnJoinMyMeetingButtonPressed;

		event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		event EventHandler OnClearButtonPressed;

		event EventHandler OnBackButtonPressed;

		event EventHandler<StringEventArgs> OnTextEntered;

		void SetText(string text);
	}
}