using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing
{
	public interface IReferencedWebConferencingAlertView : IView
	{
		event EventHandler OnButtonPressed;

		void SetLabel(string label);

		void SetIconUrl(string url);
	}
}
