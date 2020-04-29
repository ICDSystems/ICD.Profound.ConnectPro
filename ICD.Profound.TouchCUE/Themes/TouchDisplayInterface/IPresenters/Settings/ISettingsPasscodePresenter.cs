using System;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Settings
{
	public interface ISettingsPasscodePresenter : ITouchDisplayPresenter<ISettingsPasscodeView>
	{
		/// <summary>
		/// Shows the view and sets the success callback.
		/// </summary>
		/// <param name="successCallback"></param>
		void ShowView(Action<ISettingsPasscodePresenter> successCallback);
	}
}
