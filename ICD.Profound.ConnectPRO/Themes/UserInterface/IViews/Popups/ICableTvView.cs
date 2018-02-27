using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups
{
	public interface ICableTvView : IPopupView
	{
		event EventHandler<UShortEventArgs> OnChannelButtonPressed; 

		event EventHandler OnGuideButtonPressed;
		event EventHandler OnExitButtonPressed;
		event EventHandler OnPowerButtonPressed;

		event EventHandler<CharEventArgs> OnNumberButtonPressed;
		event EventHandler OnClearButtonPressed;
		event EventHandler OnEnterButtonPressed;

		event EventHandler OnUpButtonPressed;
		event EventHandler OnDownButtonPressed;
		event EventHandler OnLeftButtonPressed;
		event EventHandler OnRightButtonPressed;
		event EventHandler OnSelectButtonPressed;

		event EventHandler OnChannelUpButtonPressed;
		event EventHandler OnChannelDownButtonPressed;
		event EventHandler OnPageUpButtonPressed;
		event EventHandler OnPageDownButtonPressed;
	}
}
