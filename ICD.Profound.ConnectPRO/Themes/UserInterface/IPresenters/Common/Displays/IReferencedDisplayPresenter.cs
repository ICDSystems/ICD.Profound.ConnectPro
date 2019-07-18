using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public interface IReferencedDisplayPresenter : IUiPresenter<IReferencedDisplayView>
	{
		event EventHandler OnDisplayPressed;
		event EventHandler OnDisplaySpeakerPressed;

		MenuDisplaysPresenterDisplay Model { get; set; }
	}
}
