using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public interface IMenuCombinedSimpleModePresenter : IUiPresenter<IMenuCombinedSimpleModeView>, IDisplaysPresenter
	{
		event EventHandler OnAdvancedModePressed;
	}
}
