using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing
{
	public interface IReferencedWebConferencingAlertView : IUiView
	{
		event EventHandler OnButtonPressed;

		void SetLabel(string label);

		void SetIconUrl(string url);
	}
}
