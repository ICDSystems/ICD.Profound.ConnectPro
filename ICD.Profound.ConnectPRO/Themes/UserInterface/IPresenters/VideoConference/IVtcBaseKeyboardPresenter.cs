using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcBaseKeyboardPresenter<TView> : IUiPresenter<TView>
		where TView : IVtcBaseKeyboardView
	{
		/// <summary>
		/// Shows the view using the given callback for the dial button.
		/// </summary>
		/// <param name="dialButtonCallback"></param>
		void ShowView(Action<string> dialButtonCallback);
	}
}
