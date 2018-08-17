using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common
{
	public interface IPasscodePresenter : IPresenter<IPasscodeView>
	{
		/// <summary>
		/// Shows the view and sets the success callback.
		/// </summary>
		/// <param name="successCallback"></param>
		void ShowView(Action<IPasscodePresenter> successCallback);
	}
}
