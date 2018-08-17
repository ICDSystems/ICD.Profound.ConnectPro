using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing
{
	public interface IReferencedWebConferencingAlertPresenter : IPresenter<IReferencedWebConferencingAlertView>
	{
		event EventHandler OnPressed;

		WebConferencingAppInstructions App { get; set; }
	}
}