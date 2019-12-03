using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings
{
	public interface ISettingsPasscodePresenter : IUiPresenter<ISettingsPasscodeView>
	{
		/// <summary>
		/// Shows the view and sets the success callback.
		/// </summary>
		/// <param name="successCallback"></param>
		void ShowView(Action<ISettingsPasscodePresenter> successCallback);
	}
}
