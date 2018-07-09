using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common
{
	public interface IPasscodePresenter : IPresenter<IPasscodeView>
	{
		/// <summary>
		/// Raised when the user submits the correct password.
		/// </summary>
		event EventHandler OnSuccess;
	}
}
