using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups
{
	public interface ICableTvView : IPopupView
	{
		event EventHandler<UShortEventArgs> OnMenuButtonPressed;
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

		event EventHandler OnRepeatButtonPressed;
		event EventHandler OnRewindButtonPressed;
		event EventHandler OnFastForwardButtonPressed;
		event EventHandler OnStopButtonPressed;
		event EventHandler OnPlayButtonPressed;
		event EventHandler OnPauseButtonPressed;
		event EventHandler OnRecordButtonPressed;

		event EventHandler OnRedButtonPressed;
		event EventHandler OnGreenButtonPressed;
		event EventHandler OnBlueButtonPressed;
		event EventHandler OnYellowButtonPressed;
	}
}
