using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcKeypadPresenter : IPresenter<IVtcKeypadView>
	{
		event EventHandler OnKeyboardButtonPressed;

		event EventHandler OnDialButtonPressed;
	}
}