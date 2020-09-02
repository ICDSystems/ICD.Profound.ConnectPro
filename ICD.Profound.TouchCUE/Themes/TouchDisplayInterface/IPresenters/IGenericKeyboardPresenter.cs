using System;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters
{
	public interface IGenericKeyboardPresenter : ITouchDisplayPresenter<IGenericKeyboardView>
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
