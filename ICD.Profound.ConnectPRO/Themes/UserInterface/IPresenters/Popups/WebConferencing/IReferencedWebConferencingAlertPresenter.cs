using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;
using ICD.Profound.ConnectPROCommon.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing
{
	public interface IReferencedWebConferencingAlertPresenter : IUiPresenter<IReferencedWebConferencingAlertView>
	{
		event EventHandler OnPressed;

		WebConferencingAppInstructions App { get; set; }
	}
}