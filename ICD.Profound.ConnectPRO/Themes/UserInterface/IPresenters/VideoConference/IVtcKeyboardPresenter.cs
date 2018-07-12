using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcKeyboardPresenter : IPresenter<IVtcKeyboardView>
	{
		event EventHandler OnKeypadButtonPressed;

		event EventHandler OnDialButtonPressed;
	}
}
