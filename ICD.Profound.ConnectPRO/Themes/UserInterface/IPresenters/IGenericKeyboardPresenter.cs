using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters
{
	public interface IGenericKeyboardPresenter : IUiPresenter<IGenericKeyboardView>
	{
		/// <summary>
		/// Shows the view using the given callback for the enter button.
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="text"></param>
		/// <param name="enterCallback"></param>
		/// <param name="closeCallback"></param>
		/// <param name="changeCallback"></param>
		void ShowView(string prompt, string text, Action<string> enterCallback, Action<string> closeCallback,
		              Action<string> changeCallback);
	}
}
