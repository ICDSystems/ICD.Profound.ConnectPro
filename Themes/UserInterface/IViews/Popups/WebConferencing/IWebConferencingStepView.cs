using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing
{
	public interface IWebConferencingStepView : IView
	{
		event EventHandler OnCloseButtonPressed;

		event EventHandler OnBackButtonPressed;

		event EventHandler OnForwardButtonPressed;

		void ShowBackButton(bool show);

		void ShowForwardButton(bool show);

		void SetImageUrl(string url);

		void SetText(string text);

		void SetStepNumber(ushort number);
	}
}
