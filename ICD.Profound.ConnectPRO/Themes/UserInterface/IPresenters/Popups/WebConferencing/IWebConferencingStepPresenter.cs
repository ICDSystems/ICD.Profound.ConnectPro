﻿using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing
{
	public interface IWebConferencingStepPresenter : IUiPresenter<IWebConferencingStepView>
	{
		WebConferencingAppInstructions App { get; set; }
	}
}
